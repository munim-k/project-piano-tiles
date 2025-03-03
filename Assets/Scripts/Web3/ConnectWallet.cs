using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Thirdweb.Unity{
public class ConnectWallet : MonoBehaviour
{
    private IThirdwebWallet userAddress;

    public async void onClick(){
        var options = new WalletOptions(provider: WalletProvider.MetaMaskWallet, chainId: 592);
        var wallet = await ThirdwebManager.Instance.ConnectWallet(options);

        if (wallet != null)
        {
            userAddress = wallet;
            SceneManager.LoadScene("Loading"); 
        }
        else
        {
            Debug.LogError("Wallet Connection Failed!");
        }

    }
}
}