using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Numerics;

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
        //var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        //string senderAddress = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        //string toAddress = "0x4f4f2DfEc5a93b1E13ac64d0622f0F1D1B2A6CB2";
        BigInteger amount = 1000000000000000000;
        amount *= 40;
        amount *= gemCount/5;
        Web3PaymentThirdweb web3Payment = new Web3PaymentThirdweb();
        web3Payment.ApproveASTR(amount);
        BigInteger allowedAmount = await web3Payment.GetAllowance();
        if (allowedAmount < amount)
        {
            Debug.Log("Approving ASTR Tokens");
            web3Payment.ApproveASTR(amount);
        }
        Debug.Log("Paying for Gems");
        web3Payment.PayForGems(gemCount);
        //await contract.DropERC20_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,amount);
        //await contract.ERC20_Transfer(ThirdwebManager.Instance.GetActiveWallet(), toAddress, amount);
        //contract.erc20
        FirebaseCurrencyManager.Instance.AddStars(gemCount);
        gemCount = 5;
        UpdateGemText();
    }

    void UpdateGemText()
    {
        gemText.text = gemCount.ToString();
    }
}
}
