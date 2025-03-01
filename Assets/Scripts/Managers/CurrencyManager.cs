using UnityEngine;

public class CurrencyManager : MonoBehaviour
{

    int tokens;
    int gems;

    [SerializeField] TMPro.TextMeshProUGUI tokenText;
    [SerializeField] TMPro.TextMeshProUGUI gemText;

    public static CurrencyManager Instance;


    void Start()
    {
        DontDestroyOnLoad(gameObject);
        tokens = PlayerPrefs.GetInt("Tokens", 0);
        gems = PlayerPrefs.GetInt("Gems", 0);

        tokenText.text = tokens.ToString();
        gemText.text = gems.ToString();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddTokens(int amount)
    {
        tokens += amount;
        PlayerPrefs.SetInt("Tokens", tokens);
        tokenText.text = tokens.ToString();
    }

    public void AddGems(int amount)
    {
        gems += amount;
        PlayerPrefs.SetInt("Gems", gems);
        gemText.text = gems.ToString();
    }

    public void RemoveTokens(int amount)
    {
        tokens -= amount;
        PlayerPrefs.SetInt("Tokens", tokens);
        tokenText.text = tokens.ToString();
    }

    public void RemoveGems(int amount)
    {
        gems -= amount;
        PlayerPrefs.SetInt("Gems", gems);
        gemText.text = gems.ToString();
    }

    public int GetTokens()
    {
        return tokens;
    }

    public int GetGems()
    {
        return gems;
    }

    public void SetTokens(int amount)
    {
        tokens = amount;
        PlayerPrefs.SetInt("Tokens", tokens);
        tokenText.text = tokens.ToString();
    }

    public void SetGems(int amount)
    {
        gems = amount;
        PlayerPrefs.SetInt("Gems", gems);
        gemText.text = gems.ToString();
    }

    public bool CheckTokens(int amount) {
        return tokens >= amount;
    }

    public bool CheckGems(int amount) {
        return gems >= amount;
    }

    public bool SpendTokens(int Tokens) {
        if (CheckTokens(Tokens)) {
            RemoveTokens(Tokens);
            return true;
        }
        return false;
    }

    public bool SpendGems(int Gems) {
        if (CheckGems(Gems)) {
            RemoveGems(Gems);
            return true;
        }
        return false;
    }
}