using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    void Start()
    {
        Debug.Log("ItemCollector BAŞLADI - Player: " + gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bir şeye çarpıldı: " + other.gameObject.name + " | Tag: " + other.gameObject.tag);

        if (other.CompareTag("Gold"))
        {
            Debug.Log("ALTIN TOPLANDI!");
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddGold();
            else
                Debug.LogError("ScoreManager.Instance NULL! ScoreManager sahneye eklenmemiş!");
            other.gameObject.SetActive(false); // Destroy yerine deaktif et — pool ile uyumlu
        }
        else if (other.CompareTag("Diamond"))
        {
            Debug.Log("ELMAS TOPLANDI!");
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddDiamond();
            else
                Debug.LogError("ScoreManager.Instance NULL! ScoreManager sahneye eklenmemiş!");
            other.gameObject.SetActive(false); // Destroy yerine deaktif et — pool ile uyumlu
        }
    }
}
