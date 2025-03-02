using UnityEngine;
using UniRx;
using TMPro;

public class Score : MonoBehaviour
{
    private TMP_Text text;

    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.text = GameController.Instance.Score.Value.ToString();
    }
}