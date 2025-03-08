using UnityEngine;
using UnityEngine.SceneManagement;

namespace Thirdweb.Unity
{
    public class ConnectWallet : MonoBehaviour
    {
        private string _address;
        private bool WebglForceMetamaskExtension = false;
        //private ChainData _currentChainData;

        private void Start()
        {
            //_currentChainData = ThirdwebManager.Instance.supportedChains.Find(x => x.identifier == ThirdwebManager.Instance.activeChain);
        }

        public async void onClick()
        {
            CookieManager.Instance.SaveProgress("wallet", "was connecting");
            var options = new WalletOptions(provider: WalletProvider.MetaMaskWallet, chainId: 1868);
            var wallet = await ThirdwebManager.Instance.ConnectWallet(options);
            if(wallet != null)
            {
                Debug.Log("Connected to wallet: " + wallet);
                PostConnect();
            }
        }

        public void onClickWalletConnectWallet(){
            var externalWalletProvider = Application.platform == RuntimePlatform.WebGLPlayer && WebglForceMetamaskExtension ? WalletProvider.MetaMaskWallet : WalletProvider.WalletConnectWallet;
            var options =  new WalletOptions(provider: externalWalletProvider, chainId: 1868);
            var wallet = ThirdwebManager.Instance.ConnectWallet(options);
            if(wallet != null)
            {
                Debug.Log("Connected to wallet: " + wallet);
                PostConnect();
            }
        }

        private void PostConnect()
        {
            Debug.Log($"Connected to {_address}");
            SceneManager.LoadScene("Loading");
        }
    }
}