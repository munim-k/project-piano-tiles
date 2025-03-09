using System.Runtime.InteropServices;
using UnityEngine;

public class DesktopChecker : MonoBehaviour
{
        public GameObject loginButton;
        public GameObject goToDesktopPanel;

        [DllImport("__Internal")]
        private static extern bool IsMobile();

        private void Start()
        {
            #if !UNITY_EDITOR && UNITY_WEBGL
            {
            if(IsMobile()){
                goToDesktopPanel.SetActive(true);
            }else{
                loginButton.SetActive(true);
            }
            }
            #else
            {
                goToDesktopPanel.SetActive(true);
            }
            #endif
        }
}
