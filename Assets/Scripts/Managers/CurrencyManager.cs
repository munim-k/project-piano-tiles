using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using System.Threading.Tasks;

public class CurrencyManager : MonoBehaviour
{
  
    int tokens;
    int stars;

    [SerializeField] TMPro.TextMeshProUGUI tokenText;
    [SerializeField] TMPro.TextMeshProUGUI starText;

    public static CurrencyManager Instance;

    private GetBalancesResult balances;

    async void Start()
    {
        await InitializeUnityServices();
        await FetchCurrencyBalances();
        DontDestroyOnLoad(gameObject);
        tokens = PlayerPrefs.GetInt("Tokens", 0);
        stars = PlayerPrefs.GetInt("Gems", 0);

        tokenText.text = tokens.ToString();
        starText.text = stars.ToString();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        
    }
    private async Task InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async Task FetchCurrencyBalances()
    {
        try
        {
            balances = await EconomyService.Instance.PlayerBalances.GetBalancesAsync();
            
            
            foreach (var currency in balances.Balances)
            {
                if (currency.CurrencyId == "stars")
                {
                    stars = (int)currency.Balance;
                    starText.text = stars.ToString();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error fetching currency balances: " + e.Message);
        }
    }
    public async Task AddStars(int amount)
    {
        try
        {
            await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("stars", amount);
            stars += amount;
            starText.text = stars.ToString();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error adding stars: " + e.Message);
        }
    }

    public async Task<bool> SpendStars(int amount)
    {
        if (stars >= amount)
        {
            try
            {
                await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync("stars", amount);
                stars -= amount;
                starText.text = stars.ToString();
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error spending stars: " + e.Message);
            }
        }
        return false;
    }

    public int GetStars()
    {
        return stars;
    }


    public void AddTokens(int amount)
    {
        tokens += amount;
        PlayerPrefs.SetInt("Tokens", tokens);
        tokenText.text = tokens.ToString();
    }

    public void AddGems(int amount)
    {
        stars += amount;
        PlayerPrefs.SetInt("Gems", stars);
        starText.text = stars.ToString();
    }

    public void RemoveTokens(int amount)
    {
        tokens -= amount;
        PlayerPrefs.SetInt("Tokens", tokens);
        tokenText.text = tokens.ToString();
    }

    public void RemoveGems(int amount)
    {
        stars -= amount;
        PlayerPrefs.SetInt("Gems", stars);
        starText.text = stars.ToString();
    }

    public int GetTokens()
    {
        return tokens;
    }

    public int GetGems()
    {
        return stars;
    }

    public void SetTokens(int amount)
    {
        tokens = amount;
        PlayerPrefs.SetInt("Tokens", tokens);
        tokenText.text = tokens.ToString();
    }

    public void SetGems(int amount)
    {
        stars = amount;
        PlayerPrefs.SetInt("Gems", stars);
        starText.text = stars.ToString();
    }

    public bool CheckTokens(int amount) {
        return tokens >= amount;
    }

    public bool CheckGems(int amount) {
        return stars >= amount;
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