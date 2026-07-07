using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float forwardSpeed = 8f;
    public float laneChangeSpeed = 10f;
    private float targetX = 0f;

    // Dışarıdan (örneğin PlayerHealth tarafından) karakteri durdurmak için kullanılacak anahtar
    public bool canMove = true;

    [Header("Zıplama & Eğilme")]
    public float jumpForce = 6f;
    private Vector3 normalScale;
    private Vector3 crouchScale;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalScale = transform.localScale;
        crouchScale = new Vector3(normalScale.x, normalScale.y / 2f, normalScale.z);

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!canMove) return;

        // 1. Sürekli İleri Gitme
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // 2. Şerit Değiştirme Hedefini Belirleme
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && targetX < 2.5f) targetX += 2.5f;
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && targetX > -2.5f) targetX -= 2.5f;

        // 3. Hedef Şeride Yumuşak Geçiş
        Vector3 newPos = transform.position;
        newPos.x = Mathf.MoveTowards(newPos.x, targetX, laneChangeSpeed * Time.deltaTime);
        transform.position = newPos;

        // 4. Zemin Kontrolü
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // 5. Zıplama
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // 6. Eğilme
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) transform.localScale = crouchScale;
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)) transform.localScale = normalScale;
    }

    // Yandan çarpınca önceki şeride geri döndür
    public void BounceBack(float normalX)
    {
        if (normalX > 0.3f)        // Sağdan çarptı → sola git
            targetX -= 2.5f;
        else if (normalX < -0.3f)  // Soldan çarptı → sağa git
            targetX += 2.5f;

        // Şerit sınırını aşmasın
        targetX = Mathf.Clamp(targetX, -2.5f, 2.5f);
    }
}
