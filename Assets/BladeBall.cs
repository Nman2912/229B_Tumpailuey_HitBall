using UnityEngine;
using System.Collections;

public class BladeBall : MonoBehaviour
{
    public float speed = 10f;
    public float speedIncrement = 2f;
    public float curveStrength = 5f;      // ยิ่งมาก → โค้งแรง
    public float reflectDuration = 1.5f;

    public Transform target;

    private Rigidbody rb;
    private bool isReflected = false;
    private float currentSpeed;
    private Vector3 initialReflectDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = speed;

        if (target == null)
        {
            FindNewTarget();
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            if (!isReflected)
            {
                // ไล่ตามแบบตรง (ปกติ)
                Vector3 direction = (target.position - transform.position).normalized;
                rb.linearVelocity = direction * currentSpeed;
            }
            else
            {
                // เคลื่อนที่แบบโค้งหาเป้าหมาย
                Vector3 toTarget = (target.position - transform.position).normalized;
                Vector3 curvedDir = Vector3.Slerp(initialReflectDir, toTarget, Time.fixedDeltaTime * curveStrength);
                rb.linearVelocity = curvedDir * currentSpeed;

                // อัพเดตทิศทางล่าสุดให้โค้งต่อเนื่อง
                initialReflectDir = curvedDir.normalized;
            }
        }
    }

    public void ReflectTowardsEnemy(GameObject currentPlayer, float force)
    {
        isReflected = true;
        currentSpeed += speedIncrement;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != currentPlayer)
            {
                target = player.transform;

                // เริ่มทิศทางจากหน้าที่ผู้เล่นหัน
                initialReflectDir = currentPlayer.transform.forward;

                StartCoroutine(ResumeChaseAfterDelay(reflectDuration));
                break;
            }
        }

        Debug.Log("Ball Reflected with Curve toward enemy!");
    }

    IEnumerator ResumeChaseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isReflected = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit Player!");
        }
    }

    public void FindNewTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            target = players[Random.Range(0, players.Length)].transform;
        }
    }
}
