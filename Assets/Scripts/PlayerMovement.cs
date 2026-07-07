using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float laneChangeSpeed = 10f;
    private float targetX = 0f;

    // Dışarıdan (örneğin PlayerHealth tarafından) karakteri durdurmak için kullanılacak anahtar
    public bool canMove = true;

    [Header("Puan Bazlı Hızlanma")]
    public float baseSpeed = 8f;               // 0–999 puan arası hız
    public float speedAt1K   = 10f;            // 1.000 puanda hız
    public float speedAt10K  = 13f;            // 10.000 puanda hız
    public float speedBoostPer10K = 2f;        // 10k'dan sonra her 10k'da eklenen ek hız
    public float maxSpeed = 30f;               // Ulaşılabilecek maksimum hız

    // Okunabilir mevcut hız (Inspector'dan takip edilebilir)
    [HideInInspector] public float forwardSpeed;

    [Header("Zıplama & Eğilme")]
    public float jumpForce = 6f;
    public float fastFallForce = 30f;          // Havada iken aşağı tuşuna basınca iniş kuvveti (yüksek = daha sert iniş)
    private bool isFastFalling = false;        // Hızlı iniş aktif mi?

    private Vector3 normalScale;
    private Vector3 crouchScale;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalScale = transform.localScale;
        crouchScale = new Vector3(normalScale.x, normalScale.y / 2f, normalScale.z);

        forwardSpeed = baseSpeed;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!canMove) return;

        // 1. Puan Bazlı Hızlanma
        UpdateSpeedByScore();

        // 2. Sürekli İleri Gitme
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // 3. Şerit Değiştirme Hedefini Belirleme
        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && targetX < 2.5f) targetX += 2.5f;
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && targetX > -2.5f) targetX -= 2.5f;

        // 4. Hedef Şeride Yumuşak Geçiş
        Vector3 newPos = transform.position;
        newPos.x = Mathf.MoveTowards(newPos.x, targetX, laneChangeSpeed * Time.deltaTime);
        transform.position = newPos;

        // 5. Zemin Kontrolü
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // 6. Zıplama
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // 7. Eğilme & Hızlı İniş
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (!isGrounded)
            {
                // ✅ Havada iken: anında sert aşağı çek
                // Direkt velocity ataması — AddForce'tan çok daha anlık
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -fastFallForce, rb.linearVelocity.z);
                isFastFalling = true;
                transform.localScale = crouchScale;
            }
            else
            {
                // Yerde iken: normal eğilme
                transform.localScale = crouchScale;
            }
        }

        // Yere inince hızlı iniş bayrağını sıfırla
        if (isGrounded && isFastFalling)
        {
            isFastFalling = false;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            transform.localScale = normalScale;
        }
    }

    void UpdateSpeedByScore()
    {
        if (ScoreManager.Instance == null) return;

        float score = ScoreManager.Instance.score;
        float targetSpeed;

        if (score < 1000f)
        {
            // Aşama 0: Başlangıç hızı
            targetSpeed = baseSpeed;
        }
        else if (score < 10000f)
        {
            // Aşama 1: 1.000 puanda ilk hız artışı
            targetSpeed = speedAt1K;
        }
        else
        {
            // Aşama 2+: 10.000 puanda büyük artış,
            // Sonraki her 10.000'de giderek daha fazla artar.
            int tenKCount = Mathf.FloorToInt(score / 10000f); // 10k=1, 20k=2, 30k=3 ...
            targetSpeed = speedAt10K + (tenKCount - 1) * speedBoostPer10K;
        }

        // Maksimum hız sınırını aşmasın
        forwardSpeed = Mathf.Min(targetSpeed, maxSpeed);
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
