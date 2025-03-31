using UnityEngine;

public class NewtonForceController : MonoBehaviour
{
    [Header("F = ma Parameters")]
    public float force = 20f;
    public float mass = 5f;
    public Vector3 direction = Vector3.forward;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1f; 
        Vector3 acceleration = direction.normalized * (force / mass);
        rb.AddForce(acceleration, ForceMode.VelocityChange);
    }
}
