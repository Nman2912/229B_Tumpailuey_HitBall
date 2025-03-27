using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float reflectForce = 15f;

    private Rigidbody rb;
    private GameObject nearbyBall;
    private bool isGrounded;

    [Header("Third Person Camera")]
    public ThirdPersonCamera cameraFollow;

    [Header("Health System")]
    public int maxHealth = 3;
    private int currentHealth;

    public GameObject[] heartIcons; // ใส่หัวใจ 3 ดวงไว้ใน Inspector

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;

        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    void Update()
    {
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0) && nearbyBall != null)
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

        Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            nearbyBall = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            nearbyBall = null;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void ReflectBall()
    {
        if (nearbyBall != null)
        {
            BladeBall bladeBall = nearbyBall.GetComponent<BladeBall>();

            if (bladeBall != null)
            {
                bladeBall.ReflectTowardsEnemy(gameObject, reflectForce);
            }
        }
    }

    public void TakeDamage()
    {
        currentHealth--;
        Debug.Log(gameObject.name + " ถูกบอลชน! หัวใจเหลือ: " + currentHealth);
        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            Debug.Log(gameObject.name + " ตายแล้ว!");
            gameObject.SetActive(false);
        }
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < currentHealth);
        }
    }
}
