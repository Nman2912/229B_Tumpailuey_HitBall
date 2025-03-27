using UnityEngine;

public class BotController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float reflectForce = 15f;
    public float attackRange = 2f;

    private Rigidbody rb;
    private GameObject ball;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (ball == null)
        {
            ball = GameObject.FindGameObjectWithTag("Ball");
        }

        if (ball != null)
        {
            MoveTowardBall();

            float distance = Vector3.Distance(transform.position, ball.transform.position);
            if (distance <= attackRange)
            {
                ReflectBall();
            }
        }
    }

    void MoveTowardBall()
    {
        Vector3 direction = (ball.transform.position - transform.position).normalized;
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
    }

    void ReflectBall()
    {
        BladeBall bladeBall = ball.GetComponent<BladeBall>();
        if (bladeBall != null)
        {
            bladeBall.ReflectTowardsEnemy(gameObject, reflectForce);
        }
    }
}
