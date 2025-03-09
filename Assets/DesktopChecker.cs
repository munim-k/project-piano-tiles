using System.Runtime.InteropServices;
using UnityEngine;

public class DesktopChecker : MonoBehaviour
{
        public GameObject loginButton;

        [DllImport("__Internal")]
        private static extern bool IsMobile();

        private void Start()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            {
            if(IsMobile()){
                //enable popup for sending to desktop here
            }else{
                loginButton.SetActive(true);
            }
            }
            #else
            {
                loginButton.SetActive(true);
            }
            #endif
        }
}
