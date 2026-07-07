using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarları")]
    public int currentHealth = 3;
    public bool isDead = false;

    private MeshRenderer meshRenderer;
    private PlayerMovement playerMovement;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Büyük engele çarpma — yön tespiti burada
    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("BigObstacle"))
        {
            Vector3 normal = collision.contacts[0].normal;

            float upDot = Vector3.Dot(normal, Vector3.up);
            float forwardDot = Vector3.Dot(normal, -transform.forward);

            if (upDot > 0.5f)
            {
                // ✅ Üstüne çıkıldı — hiçbir şey olmaz
                Debug.Log("Üstüne çıkıldı - hasar yok");
            }
            else if (forwardDot > 0.5f)
            {
                // 💀 Önden çarptı — ölüm
                Debug.Log("Önden çarpma - ölüm");
                Die();
            }
            else
            {
                // 💥 Yandan çarptı — 1 can git + geri sek
                Debug.Log("Yandan çarpma - 1 hasar");
                TakeDamage();
                if (playerMovement != null)
                    playerMovement.BounceBack(normal.x);
            }
        }
    }

    // Sadece küçük engel ve Wall — BigObstacle ARTIK BURADA YOK ✅
    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.gameObject.CompareTag("SmallObstacle") || other.gameObject.CompareTag("Wall"))
        {
            TakeDamage();
            Destroy(other.gameObject);
        }
    }

    public void TakeDamage()
    {
        currentHealth -= 1;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageEffect());
        }
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
        for (int i = 0; i < 3; i++)
        {
            meshRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            meshRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
