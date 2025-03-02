using UnityEngine;
using UnityEngine.SceneManagement;

public class WonModal : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI pointsText;
    [SerializeField] TMPro.TextMeshProUGUI rewardsText;
    [SerializeField] TMPro.TextMeshProUGUI levelText;

    public void Show(int score, int rewards, int level)
    {
        pointsText.text = "" + score;
        rewardsText.text = "" + rewards;
        levelText.text = "Level: 0" + level;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Start");
    }
}