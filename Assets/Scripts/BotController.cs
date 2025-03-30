using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BotController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float wanderRadius = 10f;
    public float waitTime = 2f;

    [Header("Reflect Settings")]
    public float reflectForce = 15f;
    public float reflectRange = 2.5f;
    [Range(0f, 1f)] public float hitAccuracy = 0.85f;

    [Header("Bot Health")]
    public int maxHP = 3;
    private int currentHP;
    public GameObject botGameOverUI;

    [Header("Bot Health UI")]
    public GameObject[] heartIcons;

    [Header("Win UI")]
    public GameObject playerWinUI;

    private Vector3 wanderTarget;
    private Rigidbody rb;
    private bool isWaiting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PickNewWanderTarget();

        currentHP = maxHP;
        UpdateHearts();

        if (botGameOverUI != null)
            botGameOverUI.SetActive(false);

        if (playerWinUI != null)
            playerWinUI.SetActive(false);
    }

    void Update()
    {
        TryReflectBall();

        if (!isWaiting && Vector3.Distance(transform.position, wanderTarget) > 1f)
        {
            MoveToTarget();
        }
        else if (!isWaiting)
        {
            StartCoroutine(WaitThenWander());
        }
    }

    void PickNewWanderTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        wanderTarget = new Vector3(randomCircle.x, transform.position.y, randomCircle.y) + transform.position;
    }

    void MoveToTarget()
    {
        Vector3 direction = (wanderTarget - transform.position).normalized;
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
    }

    IEnumerator WaitThenWander()
    {
        isWaiting = true;
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(waitTime);
        PickNewWanderTarget();
        isWaiting = false;
    }

    void TryReflectBall()
    {
        GameObject ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null)
        {
            float distance = Vector3.Distance(transform.position, ball.transform.position);
            if (distance <= reflectRange)
            {
                if (Random.value > hitAccuracy)
                {
                    Debug.Log(" Bot ตีพลาด!");
                    return;
                }

                BladeBall bladeBall = ball.GetComponent<BladeBall>();
                if (bladeBall != null)
                {
                    Vector3 direction = (ball.transform.position - transform.position).normalized;
                    direction.y = 0f;

                    bladeBall.ReflectWithDirection(direction, reflectForce);

                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                        bladeBall.SetTarget(player.transform);

                    Debug.Log(" Bot ตีบอลกลับไปหา Player");
                }
            }
        }
    }

    public void TakeDamage()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameEnded)
            return;

        currentHP--;
        UpdateHearts();

        if (currentHP <= 0)
        {
            Debug.Log(" Player ชนะแล้ว!");

            if (GameManager.Instance != null)
                GameManager.Instance.isGameEnded = true;

            if (botGameOverUI != null)
                botGameOverUI.SetActive(true);

            if (playerWinUI != null)
                playerWinUI.SetActive(true);

            this.enabled = false;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;

            StartCoroutine(LoadSceneAfterDelay("EndCredit", 3f));
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] != null)
                heartIcons[i].SetActive(i < currentHP);
        }
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
