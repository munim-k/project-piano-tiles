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
        [HideInInspector] public long timestamp;

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
        private static extern string ACS_GenerateNonce();

        [DllImport("__Internal")]
        private static extern void ACS_GenerateSignature(string itemsJson, long timestamp, string nonce, string apiSecret);

        [DllImport("__Internal")]
        private static extern void ACS_SendRequest(string nonce, string signature, string timestamp, string bodyJson);

        public string GenerateNonce()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer) return "TestNonce123";
            return ACS_GenerateNonce();
        }

        public void GenerateSignature(string itemsJson, long timestamp, string nonce)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                Debug.Log("Simulated Signature Call in Editor");
                return;
            }

            this.nonce = nonce;
            this.timestamp = timestamp;
            ACS_GenerateSignature(itemsJson, timestamp, nonce, apiSecret);
        }

        // ✅ JavaScript calls this when the signature is ready
        public async void ReceiveSignature(string signature)
        {
            this.signature = signature;
            Debug.Log($"✅ Received Signature from JavaScript: {signature}");

            string userAddress = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
            string description = "Test";
            string bodyJSON = $"[{{\"userAddress\":\"{userAddress}\",\"defiId\":{defiID},\"acsAmount\":{acsAmountToTransfer},\"description\":\"{description}\"}}]";

            // ✅ Ensure request is only sent after receiving the signature
            SendACSRequest(nonce, signature, timestamp.ToString(), bodyJSON);
        }

        public void SendACSRequest(string nonce, string signature, string timestamp, string bodyJson)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Debug.Log("🔹 Sending API request to JavaScript...");
                ACS_SendRequest(nonce, signature, timestamp, bodyJson);
            }
            else
            {
                Debug.Log("🔹 WebGL function ACS_SendRequest is only available in WebGL builds.");
            }
        }

        public void OnACSResponse(string jsonResponse)
        {
            Debug.Log($"✅ API Response received: {jsonResponse}");

            ACSResponse responseObj = JsonUtility.FromJson<ACSResponse>(jsonResponse);

            Debug.Log($"✅ Response: {responseObj.response}");
            Debug.Log($"✅ Timestamp: {responseObj.timestamp}");
            Debug.Log($"✅ Signature: {responseObj.signature}");
            Debug.Log($"✅ Nonce: {responseObj.nonce}");
        }
    }
}
