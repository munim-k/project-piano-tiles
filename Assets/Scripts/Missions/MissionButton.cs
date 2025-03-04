using UnityEngine;
using UnityEngine.UI;

public class MissionButton : MonoBehaviour
{
    [SerializeField] Button claimButton;
    [SerializeField] TMPro.TextMeshProUGUI buttonText;

    [SerializeField] Sprite unlockedButtonSprite;

    [SerializeField] Image unlockedButtonSpriteRenderer;
    [SerializeField] Sprite claimedBackground;

    public void Setup(bool claimed, bool unlocked, int level) {
        ColorBlock block = claimButton.colors;
        block.disabledColor = block.normalColor;
        claimButton.colors = block;

        if (!unlocked) {
            claimButton.interactable = false;
        } else {
            claimButton.interactable = true;
            unlockedButtonSpriteRenderer.sprite = unlockedButtonSprite;

            if (claimed) {
                claimButton.interactable = false;
                buttonText.text = "Claimed";
                gameObject.GetComponent<Image>().sprite = claimedBackground;
            } else {
                claimButton.interactable = true;
                buttonText.text = "Claim";
            }
        }
        

    }
}