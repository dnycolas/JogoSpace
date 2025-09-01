using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int killCount = 0;

    void Awake()
    {
        // garante que s� exista 1 GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // n�o destr�i ao trocar de cena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKill()
    {
        killCount++;
    }

    public int GetKills()
    {
        return killCount;
    }

    public void ResetKills()
    {
        killCount = 0;
    }
}
