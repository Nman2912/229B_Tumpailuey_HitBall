using UnityEngine;
using System.Collections;

public class SwordHitbox : MonoBehaviour
{
    [Header("Sword Settings")]
    public float reflectForce = 15f;
    public float attackWindow = 0.3f; // เวลาที่เปิดให้ตีได้หลังคลิก

    private bool canAttack = false;

    void Update()
    {
        // คลิกซ้ายเพื่อเปิดการโจมตี
        if (Input.GetMouseButtonDown(0))
        {
            canAttack = true;
            Debug.Log("🖱️ เปิดตีได้!");
            StartCoroutine(DisableAttackAfterDelay(attackWindow));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("🔍 Trigger เข้ากับ: " + other.name + " | canAttack = " + canAttack);

        if (!canAttack) return;

        if (other.CompareTag("Ball"))
        {
            Debug.Log("✅ บอลโดนดาบขณะโจมตี");

            BladeBall bladeBall = other.GetComponent<BladeBall>();
            if (bladeBall != null)
            {
                Vector3 direction = transform.parent.forward;
                bladeBall.ReflectWithDirection(direction.normalized, reflectForce);
                Debug.Log("⚔️ ตีบอลสำเร็จ!");
            }

            canAttack = false; // ปิดการตีทันทีหลังโดน
        }
    }

    private IEnumerator DisableAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canAttack = false;
        Debug.Log("🛑 ปิดการตี (หมดเวลา)");
    }
}
