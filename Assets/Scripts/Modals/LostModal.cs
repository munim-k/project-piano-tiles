using UnityEngine;
using UnityEngine.SceneManagement;

public class LostModal : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI pointsText;
    [SerializeField] TMPro.TextMeshProUGUI levelText;

    public void Show(int score, int level)
    {
        pointsText.text = score.ToString();
        levelText.text = "Level: 0" + level;
    }

    public void Restart()
    {
        Debug.Log("Restarting");
        SceneManager.LoadScene("Start");
    }

    public void Skip() {
        // TODO: Implement skip logic
        SceneManager.LoadScene("Start");
    }
}