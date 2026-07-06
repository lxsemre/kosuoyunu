using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private Rigidbody rb;
    private int currentLane = 0; // -1: Sol, 0: Orta, 1: Sağ

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // İleri gitme
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Şerit değiştirme
        if (Input.GetKeyDown(KeyCode.A) && currentLane > -1)
        {
            currentLane--;
            MoveToLane();
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentLane < 1)
        {
            currentLane++;
            MoveToLane();
        }

        // Zıplama (W tuşu)
        if (Input.GetKeyDown(KeyCode.W) && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void MoveToLane()
    {
        float targetX = currentLane * 2.5f; // Şeritler arası mesafe (2.5 birim)
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
    }
}