using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int currentHealth = 3;
    public bool isDead = false;
    private bool isInvincible = false;

    private MeshRenderer meshRenderer;
    private PlayerMovement playerMovement;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        // Küçük engel ve duvar (Trigger ile çalışmaya devam edebilir)
        if (other.CompareTag("SmallObstacle") || other.CompareTag("Wall"))
        {
            TakeDamage();
            other.gameObject.SetActive(false);
        }
    }

    // Not: BigObstacle artık burada yok — üstüne çıkmak tamamen güvenli
    // Ana collider sadece fizik içindir (üstte durmak)
    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        // "BigObstacle" etiketli büyük objeye çarptıysak
        if (collision.gameObject.CompareTag("BigObstacle"))
        {
            // Çarpışmanın hangi yüzeyden olduğunu matematiksel (normal) olarak alıyoruz
            Vector3 normal = collision.GetContact(0).normal;

            // 1. Y ekseni (Yukarı) -> Üstüne bastık (Zıplarken köşeye takılmaları tolere etmek için 0.1'e düşürdük)
            if (normal.y > 0.1f)
            {
                // Güvenli bölge, üstünde koşmaya devam!
                return;
            }
            // 2. Z ekseni (Ön) -> Bloğun ön yüzüne çarptık
            else if (normal.z < -0.5f)
            {
                // Bloğun sınırlarını (boyutlarını) alıyoruz
                Collider obstacleCollider = collision.collider;
                float obstacleTopY = obstacleCollider.bounds.max.y;           // Bloğun en üst noktası
                float obstacleHeight = obstacleCollider.bounds.size.y;        // Bloğun toplam boyu
                
                // Üstten %10'luk güvenli bölgenin başlangıç Y noktası
                float safeZoneMinY = obstacleTopY - (obstacleHeight * 0.10f);
                
                // Karakterin bloğa temas ettiği tam nokta
                float hitPointY = collision.GetContact(0).point.y;

                if (hitPointY >= safeZoneMinY)
                {
                    // %10'luk üst paya çarptı (Paçayı yırttı, tırmanabilir)
                    Debug.Log("Bloğun üst %10'luk payına çarptı - GÜVENLİ");
                    return;
                }
                else
                {
                    // %90'lık alt gövdeye tam tosladı
                    Debug.Log("Büyük objeye ÖNDEN (alt %90'a) çarptı -> ÖLÜM");
                    Die();
                }
            }
            // 3. X ekseni (Yanlar) -> Bloğun sol veya sağ yanına çarptık
            else if (Mathf.Abs(normal.x) > 0.5f)
            {
                Debug.Log("Büyük objeye YANDAN çarptı -> -1 KALP");
                
                // Eğer hasar alabildiysek (ölümsüz değilsek) sekelim
                bool tookDamage = TakeDamage();
                
                if (tookDamage && playerMovement != null)
                {
                    // Unity'nin kendi çılgın fiziksel sekmesini iptal et (sadece Y hızını koru)
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                    }

                    // Bizim yumuşak sekme kodumuzu çalıştır
                    playerMovement.BounceBack(normal.x);
                }
            }
        }
    }

    public bool TakeDamage()
    {
        if (isInvincible) return false; // Zaten hasar aldıysa (yanıp sönüyorsa) tekrar hasar alma

        currentHealth -= 1;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageEffect());
        }
        
        return true;
    }

    private void Die()
    {
        if (isDead) return;

        currentHealth = 0;
        isDead = true;

        if (playerMovement != null)
            playerMovement.canMove = false;

        Debug.Log("Karakter Öldü!");
    }

    private IEnumerator DamageEffect()
    {
        if (meshRenderer == null) yield break;
        
        isInvincible = true; // Hasar almazlığı başlat
        
        for (int i = 0; i < 3; i++)
        {
            meshRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            meshRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        
        isInvincible = false; // Hasar almazlığı bitir
    }
}
