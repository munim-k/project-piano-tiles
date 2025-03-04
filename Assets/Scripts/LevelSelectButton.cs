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

        // Directly setting it as parameter color doesnt work for some reason
        gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, color.a);
    }
}