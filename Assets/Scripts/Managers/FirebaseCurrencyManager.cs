using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using TMPro;
using UnityEngine;

public class FirebaseCurrencyManager : MonoBehaviour
{
    [SerializeField] int coins;
    [SerializeField] int stars;

    [SerializeField] TMPro.TextMeshProUGUI coinText;
    [SerializeField] TMPro.TextMeshProUGUI starText;

    public static FirebaseCurrencyManager Instance;

    private void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
        coins = 100;
        coinText.text = coins.ToString();
        starText.text = stars.ToString();
        
        SetCurrencyBalances();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // public void GetJSON() =>
    //         FirebaseDatabase.GetJSON(pathInputField.text, gameObject.name, "DisplayData", "DisplayErrorObject");

    // public void PostJSON() => FirebaseDatabase.PostJSON(pathInputField.text, valueInputField.text, gameObject.name,
    //     "DisplayInfo", "DisplayErrorObject");

    // public void PushJSON() => FirebaseDatabase.PushJSON(pathInputField.text, valueInputField.text, gameObject.name,
    //     "DisplayInfo", "DisplayErrorObject");

    private void SetCurrencyBalances()
    {
        FirebaseDatabase.PostJSON("stars", stars.ToString(), gameObject.name, "DisplayInfo", "DisplayErrorObject");
        FirebaseDatabase.PostJSON("coins", coins.ToString(), gameObject.name, "DisplayInfo", "DisplayErrorObject");
    }
    public void FetchCurrencyBalances()
    {
        try
        {
            FirebaseDatabase.GetJSON("stars", gameObject.name, "DisplayData", "DisplayErrorObject");
            FirebaseDatabase.GetJSON("coins", gameObject.name, "DisplayData", "DisplayErrorObject");
        }
        catch (System.Exception e)
        {
            DisplayError(e.Message);
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coinText.text = coins.ToString();
        PlayerPrefs.SetInt("Coins", coins);
    }

    public void AddStars(int amount)
    {
        stars += amount;
        starText.text = stars.ToString();
        PlayerPrefs.SetInt("Stars", stars);
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
        coinText.text = coins.ToString();
        PlayerPrefs.SetInt("Coins", coins);
    }

    public void RemoveStars(int amount)
    {
        stars -= amount;
        starText.text = stars.ToString();
        PlayerPrefs.SetInt("Stars", stars);
    }

    public int GetCoins()
    {
        return coins;
    }

    public int GetStars()
    {
        return stars;
    }


    #region Helpers
    public void DisplayData(string data)
        {
            Debug.Log(data);
            coinText.text = data;
        }

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
