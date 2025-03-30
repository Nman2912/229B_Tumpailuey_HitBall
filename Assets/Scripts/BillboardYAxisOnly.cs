using UnityEngine;

public class BillboardYAxisOnly : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;

        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.y = 0f; // ล็อกไม่ให้หมุนตามกล้องขึ้น/ลง
        Quaternion rotation = Quaternion.LookRotation(-direction.normalized); // หันกลับเข้าหากล้อง
        transform.rotation = rotation;
    }
}
