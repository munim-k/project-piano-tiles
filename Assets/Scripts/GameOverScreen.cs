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

            int level = GameObject.FindAnyObjectByType<levelMusic>().level;
            int score = GameController.Instance.Score.Value;

            Debug.Log("GameController: Game Over. Score: " + score + ", Level: " + level);

            if (GameController.Instance.PlayerWon)
            {
                wonModal.gameObject.SetActive(true);
                wonModal.Show(score, score, level);
            }
            else
            {
                lostModal.gameObject.SetActive(true);
                lostModal.Show(score, level);
            }
        }).AddTo(this);
    }
}
