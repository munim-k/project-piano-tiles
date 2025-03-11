using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ACSResponse
{
    public string response;
    public string timestamp;
    public string signature;
    public string nonce;
}

namespace Thirdweb.Unity
{
    public class ACSManager : MonoBehaviour
    {
        [SerializeField] public int acsAmountToTransfer;
        [SerializeField] public int defiID;
        [SerializeField] public string apiSecret;

        [HideInInspector] public string signature;
        [HideInInspector] public string nonce;
        [HideInInspector] public string timestamp;

        public static ACSManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [DllImport("__Internal")]
        private static extern IntPtr ACS_GenerateNonce();

        [DllImport("__Internal")]
        private static extern void ACS_GenerateSignature(string bodyJson, string timestamp, string nonce, string apiSecret);

        [DllImport("__Internal")]
        private static extern void ACS_SendRequest(string nonce, string signature, string timestamp, string bodyJson);

        public string GenerateNonce()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer) return "TestNonce123";
            IntPtr resultPtr = ACS_GenerateNonce();
            return Marshal.PtrToStringAnsi(resultPtr);
        }

        public void GenerateSignature(string itemsJson, string timestamp, string nonce)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                return;
            }

            this.nonce = nonce;
            this.timestamp = timestamp;
            ACS_GenerateSignature(itemsJson, timestamp, nonce, apiSecret);
        }

        public async void ReceiveSignature(string signature)
        {
            this.signature = signature;

            string userAddress = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
            string description = "Test";
            string bodyJSON = $"[{{\"userAddress\":\"{userAddress}\",\"defiId\":{defiID},\"acsAmount\":{acsAmountToTransfer},\"description\":\"{description}\"}}]";

            SendACSRequest(nonce, signature, timestamp, bodyJSON);
        }

        public void SendACSRequest(string nonce, string signature, string timestamp, string bodyJson)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                ACS_SendRequest(nonce, signature, timestamp, bodyJson);
            }
            else
            {
                Debug.Log("🔹 WebGL function ACS_SendRequest is only available in WebGL builds.");
            }
        }

        public void OnACSResponse(string jsonResponse)
        {
            ACSResponse responseObj = JsonUtility.FromJson<ACSResponse>(jsonResponse);
        }
    }
}
