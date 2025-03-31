using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BladeBall : MonoBehaviour
{
    [Header("Ball Movement")]
    public float speed = 10f;
    public float speedIncrement = 0.5f;
    public float maxSpeed = 25f;
    public float curveStrength = 5f;
    public float spinSpeed = 10f; //  ความเร็วการหมุนรอบตัวเอง

    [Header("Target")]
    public Transform target;

    [Header("Hit Effect")]
    public GameObject hitEffectPrefab;

    private Rigidbody rb;
    private bool isReflected = false;
    private Vector3 initialReflectDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        RandomizeInitialTarget();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        if (!isReflected)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f;
            rb.linearVelocity = direction * speed;
        }
        else
        {
            Vector3 toTarget = (target.position - transform.position).normalized;
            toTarget.y = 0f;

            Vector3 curvedDir = Vector3.Slerp(initialReflectDir, toTarget, Time.fixedDeltaTime * curveStrength);
            rb.linearVelocity = curvedDir.normalized * speed;
            initialReflectDir = curvedDir.normalized;
        }

        
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            Vector3 spinAxis = Vector3.Cross(Vector3.up, rb.linearVelocity.normalized);
            rb.angularVelocity = spinAxis * spinSpeed;
        }

        // ป้องกันบอลจมพื้น
        if (transform.position.y < 0.1f)
        {
            Vector3 pos = transform.position;
            pos.y = 0.1f;
            transform.position = pos;
        }
    }

    public void ReflectWithDirection(Vector3 direction, float force)
    {
        speed = Mathf.Min(speed + speedIncrement, maxSpeed);

        if (direction.magnitude < 0.1f)
        {
            direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            Debug.LogWarning(" ทิศทางสะท้อนต่ำเกิน → ใช้สุ่มแทน");
        }

        isReflected = true;
        initialReflectDir = direction.normalized;
        rb.linearVelocity = initialReflectDir * speed;

        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        StartCoroutine(ResumeChaseAfterDelay(1.5f));
    }

    IEnumerator ResumeChaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isReflected = false;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ResetBall(Vector3 startPosition)
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = startPosition;
        isReflected = false;
        speed = 10f;
        StartCoroutine(StartChaseAfterDelay(1f));
    }

    IEnumerator StartChaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RandomizeInitialTarget();
    }

    public void RandomizeInitialTarget()
    {
        List<GameObject> candidates = new List<GameObject>();
        candidates.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        candidates.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        if (candidates.Count > 0)
        {
            GameObject chosen = candidates[Random.Range(0, candidates.Count)];
            SetTarget(chosen.transform);
            Debug.Log(" สุ่มเป้าหมายเริ่ม: " + chosen.name);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                player.TakeDamage();
        }

        if (other.CompareTag("Enemy"))
        {
            BotController bot = other.GetComponent<BotController>();
            if (bot != null)
                bot.TakeDamage();
        }
    }
}
