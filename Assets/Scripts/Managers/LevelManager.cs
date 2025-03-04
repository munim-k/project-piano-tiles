using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public const int LEVELS = 6;

    public bool[] levelsUnlocked = new bool[LEVELS];
    public bool[] levelsCompleted = new bool[LEVELS];

    public int level;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void UnlockLevel(int level)
    {
        levelsUnlocked[level] = true;
        
    }

    public void CompleteLevel(int level)
    {
        levelsCompleted[level] = true;
    }

}