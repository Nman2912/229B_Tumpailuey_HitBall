using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0f; // ล็อกไม่ให้เงย/ก้มตามกล้อง
        cameraForward.Normalize();

        transform.forward = cameraForward;
    }
}
