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


        if (other.CompareTag("SmallObstacle") || other.CompareTag("Wall"))
        {
            TakeDamage();
            other.gameObject.SetActive(false);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        
        if (collision.gameObject.CompareTag("BigObstacle"))
        {
            Vector3 normal = collision.GetContact(0).normal;
            Collider obstacleCollider = collision.collider;
            float obstacleTopY = obstacleCollider.bounds.max.y;          
            float obstacleHeight = obstacleCollider.bounds.size.y;        
            float safeZoneMinY = obstacleTopY - (obstacleHeight * 0.15f); 
            float hitPointY = collision.GetContact(0).point.y;

            if (normal.y > 0.1f)
            {
                return;
            }
            
            
            if (hitPointY >= safeZoneMinY)
            {
                Debug.Log("Bloğun üst %15'lik payına çarptı (Geçiş / Zıplama) - GÜVENLİ");
                return;
            }

            if (normal.z < -0.5f)
            {
                Debug.Log("Büyük objeye ÖNDEN (alt kısma) çarptı -> ÖLÜM");
                Die();
            }
            else if (Mathf.Abs(normal.x) > 0.5f)
            {
                Debug.Log("Büyük objeye YANDAN çarptı -> -1 KALP");
                
              
                bool tookDamage = TakeDamage();
                
                if (tookDamage && playerMovement != null)
                {
                    
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                    }

                   
                    playerMovement.BounceBack(normal.x);
                }
            }
        }
    }

    public bool TakeDamage()
    {
        if (isInvincible) return false; 

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
        
        isInvincible = true; 
        
        for (int i = 0; i < 3; i++)
        {
            meshRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            meshRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        
        isInvincible = false; 
    }
}
