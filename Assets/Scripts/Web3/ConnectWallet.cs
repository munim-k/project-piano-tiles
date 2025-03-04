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
        private ChainData _currentChainData;

        private void Start()
        {
            _currentChainData = ThirdwebManager.Instance.supportedChains.Find(x => x.identifier == ThirdwebManager.Instance.activeChain);
        }

        public async void onClick()
        {
            var wc = new WalletConnection(provider: WalletProvider.WalletConnect, chainId: BigInteger.Parse(_currentChainData.chainId));
            await Connect(wc);
        }

        private async Task Connect(WalletConnection wc)
        {
            Debug.Log($"Connecting to {wc.provider}...");

            await Task.Delay(500);

            try
            {
                _address = await ThirdwebManager.Instance.SDK.wallet.Connect(wc);
                if (!string.IsNullOrEmpty(_address))
                {
                    PostConnect();
                }
                else
                {
                    Debug.LogError("Wallet Connection Failed!");
                }
            }
            catch (Exception e)
            {
                _address = null;
                Debug.LogError($"Failed to connect: {e}");
            }
        }

        private async void PostConnect()
        {
            Debug.Log($"Connected to {_address}");

            var bal = await ThirdwebManager.Instance.SDK.wallet.GetBalance();
            var balStr = $"{bal.value.ToEth()} {bal.symbol}";
            Debug.Log($"Balance: {balStr}");

            SceneManager.LoadScene("Loading");
        }
    }
}