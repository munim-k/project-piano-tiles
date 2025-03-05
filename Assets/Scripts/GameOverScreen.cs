using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class GameOverScreen : MonoBehaviour
{
    private CanvasElementVisibility visibility;

    [SerializeField] WonModal wonModal;
    [SerializeField] LostModal lostModal;

    void Start()
    {
        visibility = GetComponent<CanvasElementVisibility>();
        GameController.Instance.ShowGameOverScreen.Where((value) => value).Subscribe((value) =>
        {
            visibility.Visible = true;

            int level = LevelManager.Instance.level + 1;
            int score = GameController.Instance.Score.Value;

            if (GameController.Instance.PlayerWon)
            {
                wonModal.gameObject.SetActive(true);
                wonModal.Show(score, score, level);
                LevelManager.Instance.levelsCompleted[level] = true;
            }
            else
            {
                lostModal.gameObject.SetActive(true);
                lostModal.Show(score, level);
            }
        }).AddTo(this);
    }
}
