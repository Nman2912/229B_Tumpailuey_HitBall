using UnityEngine;

public class BotWanderAndReflect : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float wanderRadius = 10f;
    public float waitTime = 2f;

    [Header("Reflect Settings")]
    public float reflectForce = 15f;
    public float reflectRange = 3f;

    private Rigidbody rb;
    private Vector3 targetPoint;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private GameObject ball;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        PickNewTargetPoint();
    }

    void Update()
    {
        if (ball == null)
        {
            ball = GameObject.FindGameObjectWithTag("Ball");
        }

        if (ball != null && Vector3.Distance(transform.position, ball.transform.position) <= reflectRange)
        {
            BladeBall bladeBall = ball.GetComponent<BladeBall>();
            if (bladeBall != null)
            {
                // ✅ หาทิศทางไปยัง Player ฝั่งตรงข้าม
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    if (player != gameObject)
                    {
                        Vector3 toTarget = (player.transform.position - transform.position).normalized;
                        bladeBall.ReflectWithDirection(toTarget, reflectForce);
                        Debug.Log("Bot ตีบอลไปหาฝั่งตรงข้าม!");
                        break;
                    }
                }
            }
        }

        WanderMovement();
    }

    void WanderMovement()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                PickNewTargetPoint();
            }
            else
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }
        else
        {
            Vector3 direction = (targetPoint - transform.position);
            direction.y = 0f;

            if (direction.magnitude < 1f)
            {
                isWaiting = true;
                waitTimer = 0f;
            }
            else
            {
                Vector3 moveDir = direction.normalized;
                rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

                if (moveDir != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
                }
            }
        }
    }

    void PickNewTargetPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        targetPoint = transform.position + randomOffset;
    }
}
