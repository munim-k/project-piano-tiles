using System.Threading.Tasks;
using UnityEngine;
using Thirdweb;
using System.Numerics;

namespace Thirdweb.Unity{
public class Web3PaymentThirdweb : MonoBehaviour
{
    private ThirdwebContract tokenContract;  // ASTR Token Contract
    private ThirdwebContract paymentContract;  // PayForGems Contract

    

    private string contractAddress = "0x5466eaBAEC6537f041f9CcFbd36f1Dfe737F3A82";  // PayForGems Contract
    private string tokenAddress = "0x2CAE934a1e84F693fbb78CA5ED3B0A6893259441";  // ASTR Token Contract

    private string playerWalletAddress;

    async void Start()
    {
        // sdk = new ThirdwebSDK("astar"); // Connect to Astar Network
        tokenContract = await ThirdwebManager.Instance.GetContract(tokenAddress,1868);
        paymentContract = await ThirdwebManager.Instance.GetContract(contractAddress, 1868);
        playerWalletAddress = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
    }

    // Connect Wallet
    // public async void ConnectWallet()
    // {
    //     playerWalletAddress = await sdk.wallet.Connect();
    //     Debug.Log("Wallet Connected: " + playerWalletAddress);
    // }

    // Get ASTR Token Allowance
    public async Task<BigInteger> GetAllowance()
    {
        var allowance = await tokenContract.ERC20_Allowance(playerWalletAddress, contractAddress);
        Debug.Log("Allowance: " + allowance);
        return allowance;
    }

    // Approve Spending of ASTR Tokens
    public async void ApproveASTR(BigInteger amount)
    {
        try
        {
            await tokenContract.ERC20_Approve(ThirdwebManager.Instance.GetActiveWallet(),contractAddress, amount);
            Debug.Log("Approval Successful: " + amount + " ASTR approved.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Approval Failed: " + e.Message);
        }
    }

    // Pay for Gems
    public async void PayForGems(int gemAmount)
    {
        if ((gemAmount % 5) != 0)
        {
            Debug.LogError("Gem amount must be in multiples of 5.");
            return;
        }

        try
        {
            var result = await ThirdwebContract.Write(ThirdwebManager.Instance.GetActiveWallet(), paymentContract, "pay", new BigInteger(gemAmount));
            Debug.Log("Transaction Sent: " + result.TransactionHash);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Transaction Failed: " + e.Message);
        }
    }

    // Check if transaction is mined
    // public async Task<bool> IsTransactionMined(string txHash)
    // {
    //     while (true)
    //     {
    //         var receipt = await ThirdwebManager.Instance.GetActiveWallet().GetTransaction(txHash);
    //         if (receipt != null)
    //         {
    //             Debug.Log("Transaction Confirmed!");
    //             return true;
    //         }

    //         await Task.Delay(3000); // Wait 3 seconds before checking again
    //     }
    // }

    // Get ASTR Balance
    public async Task<BigInteger> GetASTRBalance()
    {
        var balance = await tokenContract.ERC20_BalanceOf(playerWalletAddress);
        Debug.Log("ASTR Balance: " + balance);
        return balance;
    }
}
}
