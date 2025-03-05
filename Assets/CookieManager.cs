using System.Runtime.InteropServices;
using UnityEngine;

public class CookieManager : MonoBehaviour
{
    private static CookieManager instance;
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SetCookie(string key, string value, int days);

    [DllImport("__Internal")]
    private static extern string GetCookie(string key);
#endif

    public static CookieManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("CookieManager");
                instance = obj.AddComponent<CookieManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SaveProgress(string key, string value)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetCookie(key, value, 30);
#else
        PlayerPrefs.SetString(key, value); // Fallback for testing in the editor
        PlayerPrefs.Save();
#endif
    }

    public string LoadProgress(string key)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return GetCookie(key);
#else
        return PlayerPrefs.GetString(key, "");
#endif
    }
}
