using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float reflectForce = 15f;

    private Rigidbody rb;
    private bool isGrounded = true;

    [Header("Third Person Camera")]
    public ThirdPersonCamera cameraFollow;

    [Header("Health System")]
    public int maxHealth = 3;
    public Image[] heartIcons;
    private int currentHealth;

    [Header("Ball Reset Settings")]
    public BladeBall bladeBall;
    public Transform ballResetPoint;

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    private bool canAttack = false;
    private GameObject currentBall;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        Cursor.lockState = CursorLockMode.Locked;

        currentHealth = maxHealth;
        UpdateHearts();

        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    void Update()
    {
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0) && canAttack && currentBall != null)
        {
            ReflectBall();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
            targetAngle += cameraFollow.GetCameraYawRotation().eulerAngles.y;

            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            currentBall = other.gameObject;
            canAttack = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            currentBall = null;
            canAttack = false;
        }
    }

    void ReflectBall()
    {
        if (currentBall == null) return;

        BladeBall ball = currentBall.GetComponent<BladeBall>();
        if (ball != null)
        {
            Vector3 reflectDir = transform.forward;
            ball.ReflectWithDirection(reflectDir, reflectForce);

            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
                ball.SetTarget(enemy.transform);
        }
    }

    public void TakeDamage()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameEnded)
            return;

        currentHealth--;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHearts();

        if (bladeBall != null && ballResetPoint != null)
        {
            bladeBall.ResetBall(ballResetPoint.position);
        }

        if (currentHealth <= 0)
        {
            Debug.Log(" Player แพ้แล้ว!");

            if (GameManager.Instance != null)
                GameManager.Instance.isGameEnded = true;

            if (gameOverUI != null)
                gameOverUI.SetActive(true);

            StartCoroutine(LoadSceneAfterDelay("EndCredit", 3f));
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].gameObject.SetActive(i < currentHealth);
        }
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    void ResetPlayer()
    {
        currentHealth = maxHealth;
        UpdateHearts();

        if (bladeBall != null && ballResetPoint != null)
        {
            bladeBall.ResetBall(ballResetPoint.position);
        }
    }
}
