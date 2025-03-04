using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI levelText;
    [SerializeField] Image lockIcon;
    [SerializeField] TMPro.TextMeshProUGUI priceText;


    public void SetLevel(int level, int price, bool unlocked, Color color)
    {
        levelText.text = "Level " + (level + 1);
        lockIcon.gameObject.SetActive(!unlocked);

        if (!unlocked)
        {
            priceText.text = price.ToString();
        }

        Debug.Log("Setting color: " + color);
        gameObject.GetComponent<Image>().color = color;
    }
}