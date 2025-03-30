using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;           // ผู้เล่น
    public Vector3 offset = new Vector3(0, 2, -5);  // ตำแหน่งกล้องหลังผู้เล่น
    public float sensitivity = 2f;     // ความไวของเมาส์
    public float distance = 5f;        // ระยะห่างจากผู้เล่น
    public float minY = -20f;          // มุมกล้องต่ำสุด
    public float maxY = 60f;           // มุมกล้องสูงสุด

    private float rotX = 0f;
    private float rotY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // ล็อกเมาส์
    }

    void LateUpdate()
    {
        if (target == null) return;

        rotX += Input.GetAxis("Mouse X") * sensitivity;
        rotY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotY = Mathf.Clamp(rotY, minY, maxY);

        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f); // มองสูงขึ้นเล็กน้อย
    }

    public Quaternion GetCameraYawRotation()
    {
        return Quaternion.Euler(0, rotX, 0);
    }
}
