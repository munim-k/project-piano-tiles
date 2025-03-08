using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using UnityEngine.UI;

enum ConnectOptions{
    METAMASK_EXTENSION,
    QR_CODE,
}

namespace Thirdweb.Unity
{
    public class ConnectWallet : MonoBehaviour
    {
        private string _address;
        private bool WebglForceMetamaskExtension = false;

        [SerializeField] Button _connectButton;

        [DllImport("__Internal")]
        private static extern bool IsMobile();

        private void Start()
        {
            _connectButton.onClick.RemoveAllListeners();
            #if !UNITY_EDITOR && UNITY_WEBGL
            {
            if(IsMobile()){
                _connectButton.onClick.AddListener(() => onClick(GetWalletOptions(ConnectOptions.QR_CODE)));
            }else{
                _connectButton.onClick.AddListener(() => onClick(GetWalletOptions(ConnectOptions.METAMASK_EXTENSION)));
            }
            }
            #else
            {
                _connectButton.onClick.AddListener(() => onClick(GetWalletOptions(ConnectOptions.QR_CODE)));
            }
            #endif
        }

        WalletOptions GetWalletOptions(ConnectOptions connectOptions)
        {
            switch(connectOptions)
            {
                case ConnectOptions.METAMASK_EXTENSION:
                    return new WalletOptions(provider: WalletProvider.MetaMaskWallet, chainId: 1868);
                case ConnectOptions.QR_CODE:
                    var externalWalletProvider = Application.platform == RuntimePlatform.WebGLPlayer && WebglForceMetamaskExtension ? WalletProvider.MetaMaskWallet : WalletProvider.WalletConnectWallet;
                    return new WalletOptions(provider: externalWalletProvider, chainId: 1868);
                default:
                    return new WalletOptions(provider: WalletProvider.MetaMaskWallet, chainId: 1868);
            }
        }

        async void onClick(WalletOptions options)
        {            
            var wallet = await ThirdwebManager.Instance.ConnectWallet(options);
            PostConnect();
        }

        private async void PostConnect()
        {
            SceneManager.LoadScene("Loading");
        }
    }
}