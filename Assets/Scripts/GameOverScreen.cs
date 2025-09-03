using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text killsText;

    void Start()
    {
        int kills = GameManager.instance.GetKills();
        killsText.text = "Inimigos derrotados: " + kills;
    }

    public void RestartGame()
    {
        GameManager.instance.ResetKills();
        SceneManager.LoadScene("SampleScene"); // sua cena principal
    }

    public void BackToMenu()
    {
        GameManager.instance.ResetKills();
        SceneManager.LoadScene("MainMenu");
    }
}
