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

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        // ✅ Büyük bloğun ÖN yüzüne çarpma → Ölüm
        if (other.CompareTag("BigObstacleFront"))
        {
            Debug.Log("Büyük bloğun önüne çarptı - ÖLÜM");
            Die();
        }

        // ✅ Büyük bloğun YAN yüzüne çarpma → 1 hasar
        else if (other.CompareTag("BigObstacleSide"))
        {
            Debug.Log("Büyük bloğun yanına çarptı - 1 hasar");
            TakeDamage();
            // Hangi yandan çarptığını bul ve geri sek
            float sideDir = other.transform.position.x - transform.position.x;
            if (playerMovement != null)
                playerMovement.BounceBack(sideDir > 0 ? 1f : -1f);
        }

        // Küçük engel ve duvar
        else if (other.CompareTag("SmallObstacle") || other.CompareTag("Wall"))
        {
            TakeDamage();
            other.gameObject.SetActive(false);
        }
    }

    // Not: BigObstacle artık burada yok — üstüne çıkmak tamamen güvenli
    // Ana collider sadece fizik içindir (üstte durmak)
    void OnCollisionEnter(Collision collision)
    {
        // İleride başka çarpışma tipleri eklenebilir
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
