using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    void Awake() {
        DontDestroyOnLoad(gameObject);

        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
}