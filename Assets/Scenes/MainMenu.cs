using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Buton buna bağlanacak
    public void OyunuBaslat()
    {
        // Sahne isminin Build Settings'tekiyle aynı olduğundan emin ol
        SceneManager.LoadScene("SampleScene");
    }

    // Quit butonu buna bağlanacak
    public void OyundanCik()
    {
        // Unity editöründe çalışırken oyun kapanmaz, bu mesajı görmen normal
        Debug.Log("Oyun kapatıldı");
        Application.Quit();
    }
}