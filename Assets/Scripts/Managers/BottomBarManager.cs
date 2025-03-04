using UnityEngine;

public class BottomBarManager : MonoBehaviour
{

    public static BottomBarManager Instance;

    void Start() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void MissionButton() {
        Debug.Log("Mission Button Clicked");
        // TODO: Add code to open the mission scene
    }

    public void LevelsButton() {
        Debug.Log("Levels Button Clicked");
        // TODO: Add code to open the levels scene
    }

    public void ReferToWinButton() {
        Debug.Log("Refer to Win Button Clicked");
        // TODO: Add code to open the refer to win scene
    }

    public void ShvanAIButton() {
        Debug.Log("Shvan AI Button Clicked");
        // TODO: Add code to open the shvan AI scene
    }
}