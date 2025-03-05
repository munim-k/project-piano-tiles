using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Thirdweb;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Thirdweb.Unity
{
    public class ConnectWallet : MonoBehaviour
    {
        private string _address;
        //private ChainData _currentChainData;

        private void Start()
        {
            //_currentChainData = ThirdwebManager.Instance.supportedChains.Find(x => x.identifier == ThirdwebManager.Instance.activeChain);
        }

        public async void onClick()
        {
            var options = new WalletOptions(provider: WalletProvider.MetaMaskWallet, chainId: 1868);
            var wallet = await ThirdwebManager.Instance.ConnectWallet(options);
            if(wallet != null)
            {
                Debug.Log("Connected to wallet: " + wallet);
                PostConnect();
            }
        }

        private async void PostConnect()
        {
            Debug.Log($"Connected to {_address}");
            SceneManager.LoadScene("Loading");
        }
    }
}