using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginManager : MonoBehaviour
{
    public event Action<PlayerProfile> OnSignedIn;
    public event Action<PlayerProfile> OnAvatarUpdate;

    public GameObject enterNameCanvas;
    public TMP_InputField nameInputField;
    public Button acceptButton;
    public TMP_Text playerNameText;

    private PlayerInfo playerInfo;
    private PlayerProfile playerProfile;
    public PlayerProfile PlayerProfile => playerProfile;
    private const string PlayerTokenKey = "PlayerToken";
    private const string PlayerNameKey = "PlayerName";
    private const string PlayerInfoKey = "PlayerInfo";

    async void Start()
    {
        await InitializeUnityServices();

        // Set the target frame rate to 60
        Application.targetFrameRate = 60;

        // Enable VSync
        QualitySettings.vSyncCount = 1;

        Debug.Log("Framerate limited to 60 and VSync enabled.");
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            PlayerAccountService.Instance.SignedIn += SignedIn;
            Debug.Log("Unity Services initialized successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to initialize Unity Services: " + ex.Message);
        }
    }

    private bool CheckStoredCredentials()
    {
        if (PlayerPrefs.HasKey(PlayerTokenKey) && PlayerPrefs.HasKey(PlayerNameKey) && PlayerPrefs.HasKey(PlayerInfoKey))
        {
            string storedToken = PlayerPrefs.GetString(PlayerTokenKey);
            string storedName = PlayerPrefs.GetString(PlayerNameKey);
            string storedInfo = PlayerPrefs.GetString(PlayerInfoKey);
            playerProfile = JsonUtility.FromJson<PlayerProfile>(storedInfo);
            playerProfile.Name = storedName;
            SignInWithStoredToken(storedToken);
            return true;
        }
        return false;
    }

    public async void OnSignInButtonClicked()
    {
        
            await InitSignIn();
        
    }

    public async Task InitSignIn()
    {
        Debug.Log("Initiating sign-in...");
        try
        {
            if (PlayerAccountService.Instance == null)
            {
                Debug.LogError("PlayerAccountService.Instance is null.");
                return;
            }

            Debug.Log("PlayerAccountService.Instance is not null. Starting sign-in...");
            await PlayerAccountService.Instance.StartSignInAsync();
            Debug.Log("Sign-in initiated successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Sign-in initiation failed: " + ex.Message);
        }
    }

    private async void SignedIn()
    {
        try
        {
            if (PlayerAccountService.Instance == null)
            {
                Debug.LogError("PlayerAccountService.Instance is null.");
                return;
            }

            var accessToken = PlayerAccountService.Instance.AccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                Debug.LogError("AccessToken is null or empty.");
                return;
            }

            Debug.Log("AccessToken retrieved successfully. Signing in with Unity...");
            await SignInWithUnityAsync(accessToken);
            Debug.Log("Signed in with Unity successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during sign-in: " + ex.Message);
        }
    }

    private async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            if (AuthenticationService.Instance == null)
            {
                Debug.LogError("AuthenticationService.Instance is null.");
                return;
            }

            Debug.Log("AuthenticationService.Instance is not null. Signing in with Unity...");
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("Unity Authentication successful!");

            playerInfo = AuthenticationService.Instance.PlayerInfo;
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();
            Debug.Log("Player name retrieved: " + name);
            SceneManager.LoadScene("login");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError("Unity Authentication failed: " + ex.Message);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError("Request failed: " + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("An unexpected error occurred: " + ex.Message);
        }
    }

    private void ShowEnterNameCanvas()
    {
        enterNameCanvas.SetActive(true);
        acceptButton.onClick.AddListener(OnAcceptButtonClicked);
    }

    private async void OnAcceptButtonClicked()
    {
        string playerName = nameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            await UpdatePlayerProfileAsync(playerName);
            SavePlayerProfile(PlayerAccountService.Instance.AccessToken, playerName);
            //SceneManager.LoadScene("Start");
        }
    }

    private void SavePlayerProfile(string token, string playerName)
    {
        PlayerPrefs.SetString(PlayerTokenKey, token);
        PlayerPrefs.SetString(PlayerNameKey, playerName);
        PlayerPrefs.SetString(PlayerInfoKey, JsonUtility.ToJson(playerProfile));
        PlayerPrefs.Save();
    }

    private async void SignInWithStoredToken(string token)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(token);
            Debug.Log("Signed in with stored token successfully!");
            SceneManager.LoadScene("login");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to sign in with stored token: " + ex.Message);
        }
    }

    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= SignedIn;
    }

    private async Task UpdatePlayerProfileAsync(string playerName)
    {
        try
        {
            var playerProfile = await GetPlayerProfileAsync();
            playerProfile.Name = playerName;
            await SavePlayerProfileAsync(playerProfile);
            Debug.Log("Player profile updated successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to update player profile: " + ex.Message);
        }
    }

    private async Task<PlayerProfile> GetPlayerProfileAsync()
    {
        return await Task.FromResult(playerProfile);
    }

    private async Task SavePlayerProfileAsync(PlayerProfile profile)
    {
        try
        {
            // Assuming you have a method to update the player profile on the backend
            
            await AuthenticationService.Instance.UpdatePlayerNameAsync(profile.Name);
            Debug.Log("Player profile saved to backend successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save player profile to backend: " + ex.Message);
        }
    }
}

[Serializable]
public struct PlayerProfile
{
    public PlayerInfo playerInfo;
    public string Name;
}
