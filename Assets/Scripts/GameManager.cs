using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int kills = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // não destrói ao trocar de cena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKill()
    {
        kills++;
    }

    public int GetKills()
    {
        return kills;
    }

    public void ResetKills()
    {
        kills = 0;
    }
}
