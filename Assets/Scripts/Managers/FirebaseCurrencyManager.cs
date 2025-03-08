using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using TMPro;
using UnityEngine;

public class FirebaseCurrencyManager : MonoBehaviour
{
    [SerializeField] int tokens;
    [SerializeField] int stars;

    [SerializeField] TMPro.TextMeshProUGUI coinText;
    [SerializeField] TMPro.TextMeshProUGUI starText;

    public static FirebaseCurrencyManager Instance;

    private void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        FetchCurrencyBalances();
    }

    // INITIALIZATION

    public void FetchCurrencyBalances()
    {
        try
        {
            FirebaseDatabase.GetJSON($"users/{firebaseManager.idToken}", gameObject.name, nameof(SetUserData), nameof(DisplayErrorObject));
        }
        catch (System.Exception e)
        {
            DisplayError(e.Message);
        }
    }

    private void SetCurrencyBalances()
    {
        SetTokens(tokens);
        SetStars(stars);
    }

    public void SetUserData(string data) {
        var userData = StringSerializationAPI.Deserialize(typeof(UserData), data) as UserData;
        tokens = userData.tokens;
        stars = userData.stars;
        coinText.text = tokens.ToString();
        starText.text = stars.ToString();
    }


    // TOKENS
    public int GetCoins()
    {
        return tokens;
    }

    private void SetTokens(int tokens)
    {
        if (tokens >= 0) {
            this.tokens = tokens;
            FirebaseDatabase.PostJSON($"users/{firebaseManager.idToken}/tokens", tokens.ToString(), gameObject.name, "DisplayInfo", "DisplayErrorObject");
        }
    }

    public void AddTokens(int amount)
    {
        tokens += amount;
        coinText.text = tokens.ToString();
        SetCurrencyBalances();
    }

    public void RemoveTokens(int amount)
    {
        tokens -= amount;
        coinText.text = tokens.ToString();
        SetCurrencyBalances();
    }



    // STARS

    public int GetStars()
    {
        return stars;
    }

    private void SetStars(int stars)
    {
        if (stars >= 0) {
            this.stars = stars;
            FirebaseDatabase.PostJSON($"users/{firebaseManager.idToken}/stars", stars.ToString(), gameObject.name, "DisplayInfo", "DisplayErrorObject");
        }
    }
   
    public void AddStars(int amount)
    {
        stars += amount;
        starText.text = stars.ToString();
        SetCurrencyBalances();
    }

    public void RemoveStars(int amount)
    {
        stars -= amount;
        starText.text = stars.ToString();
        SetCurrencyBalances();
    }


    #region Helpers

    public void DisplayInfo(string info)
    {
        Debug.Log(info);
    }

    public void DisplayErrorObject(string error)
    {
        var parsedError = StringSerializationAPI.Deserialize(typeof(FirebaseError), error) as FirebaseError;
        DisplayError(parsedError.message);
    }

    public void DisplayError(string error)
    {
        Debug.LogError(error);
    }
    #endregion
}