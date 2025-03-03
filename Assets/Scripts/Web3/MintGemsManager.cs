using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MintGemsManager : MonoBehaviour
{
    public TextMeshProUGUI gemText;
    
    private int gemCount = 5; // Minimum and starting value

    void Start()
    {
        UpdateGemText();
    }

    public void IncreaseGems()
    {
        gemCount += 5;
        UpdateGemText();
    }

    public void DecreaseGems()
    {
        if (gemCount > 5)
        {
            gemCount -= 5;
            UpdateGemText();
        }
    }

    public void MintGems()
    {
        Debug.Log("Minting Gems");
        // Functionality to be implemented later
    }

    void UpdateGemText()
    {
        gemText.text = gemCount.ToString();
    }
}
