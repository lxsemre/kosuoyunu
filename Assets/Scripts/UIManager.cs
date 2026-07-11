using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Arayüz Yazıları")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI diamondText;

    [Header("Can Arayüzü")]
    public GameObject[] hearts;
    public GameObject gameOverPanel;

    [Header("Sistem Bağlantıları")]
    public ScoreManager scoreManager;
    public PlayerHealth playerHealth;

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (scoreManager == null)
            scoreManager = FindFirstObjectByType<ScoreManager>();
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    void Update()
    {
        if (scoreManager != null)
        {
            if (scoreText != null) scoreText.text = "Skor: " + Mathf.FloorToInt(scoreManager.score);
            if (goldText != null) goldText.text = "Altın: " + scoreManager.goldCount;
            if (diamondText != null) diamondText.text = "Elmas: " + scoreManager.diamondCount;
        }

        if (playerHealth != null)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] != null) 
                    hearts[i].SetActive(i < playerHealth.currentHealth);
            }

            if (playerHealth.isDead && gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Time.timeScale = 0f;
                if (scoreManager != null) scoreManager.isScoreActive = false;
            }
        }
    }

    public void LoadMenuFromGameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
