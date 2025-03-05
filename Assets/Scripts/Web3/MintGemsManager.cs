using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Thirdweb.Unity{
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
    [SerializeField] ulong ActiveChainId = 1868;

    public async void MintGems(string addr)
    {
        Debug.Log("Minting Gems");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        int amount = (gemCount / 5);
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,amount);
        CurrencyManager.Instance.AddGems(gemCount);
        gemCount = 5;
        UpdateGemText();
    }

    void UpdateGemText()
    {
        gemText.text = gemCount.ToString();
    }
}
}
