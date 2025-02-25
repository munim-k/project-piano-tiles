using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    public void SelectSong(int songIndex) {
        LevelManager.Instance.songIndex = songIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
