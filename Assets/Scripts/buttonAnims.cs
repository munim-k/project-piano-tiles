using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class buttonAnims : MonoBehaviour
{
    private Vector3 originalScale;
    private Button button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale;
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonPress);
        }
        else
        {
            Debug.LogWarning("Button component not found on the GameObject.");
        }
    }

    // Public method to trigger the animation
    public void OnButtonPress()
    {
        AnimateButtonPress();
    }

    void AnimateButtonPress()
    {
        transform.DOScale(new Vector3(3, 3, 3), 0.1f).OnComplete(() =>
        {
            transform.DOScale(originalScale, 0.2f);
        });
    }
}
