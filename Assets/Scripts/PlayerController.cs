using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float forwardSpeed = 8f;
    public float laneChangeSpeed = 10f;
    private float targetX = 0f;
    private bool isDead = false;

    [Header("Zıplama & Eğilme")]
    public float jumpForce = 6f;
    private Vector3 normalScale;
    private Vector3 crouchScale;
    private Rigidbody rb;

    [Header("Can Sistemi")]
    public int health = 3;
    public GameObject[] hearts;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalScale = transform.localScale;
        crouchScale = new Vector3(normalScale.x, normalScale.y / 2f, normalScale.z);
    }

    void Update()
    {
        if (isDead) return;

        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && targetX < 2.5f) targetX += 2.5f;
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && targetX > -2.5f) targetX -= 2.5f;

        Vector3 newPos = transform.position;
        newPos.x = Mathf.MoveTowards(newPos.x, targetX, laneChangeSpeed * Time.deltaTime);
        transform.position = newPos;

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) transform.localScale = crouchScale;
        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)) transform.localScale = normalScale;
    }

    // 1. DURUM: KATI ÇARPIŞMALAR (Büyük Engeller)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BigObstacle"))
        {
            health = 0;
            UpdateHearts();

            isDead = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Invoke("LoadMenu", 1f);
        }
    }

    // 2. DURUM: İÇİNDEN GEÇİLENLER (Küçük Engeller ve Duvarlar)
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SmallObstacle") || other.gameObject.CompareTag("Wall"))
        {
            health -= 1;
            UpdateHearts();

            // Küpün içinden geçtiği engeli anında yok et
            Destroy(other.gameObject);

            if (health <= 0)
            {
                isDead = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                Invoke("LoadMenu", 1f);
            }
        }
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < health);
        }
    }
}