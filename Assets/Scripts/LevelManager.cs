using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int songIndex = 0;

    public static LevelManager Instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
