using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    int score;
    public static ScoreManager Instance;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public int GetScore() {
        return score;
    }

    public void ResetScore() {
        score = 0;
    }
}