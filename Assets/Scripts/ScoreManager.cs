using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; 

    [Header("Puanlama Durumu")]
    public float score = 0f;
    public int goldCount = 0;
    public int diamondCount = 0;

    [Header("Ayarlar")]
    public float scorePerSecond = 5f;
    public bool isScoreActive = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isScoreActive) return;
        score += Time.deltaTime * scorePerSecond;
    }

    public void AddGold()
    {
        goldCount += 1;
        score += 10f;
    }

    public void AddDiamond()
    {
        diamondCount += 1;
        score += 100f;
    }
}
