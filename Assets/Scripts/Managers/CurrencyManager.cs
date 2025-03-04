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
        //await InitializeUnityServices();
        await FetchCurrencyBalances();
        DontDestroyOnLoad(gameObject);
        
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
  
    private async Task FetchCurrencyBalances()
    {
        try
        {
            balances = await EconomyService.Instance.PlayerBalances.GetBalancesAsync();
            
            
            foreach (var currency in balances.Balances)
            {
                if (currency.CurrencyId == "STARS")
                {
                    stars = (int)currency.Balance;
                    starText.text = stars.ToString();
                }
                if(currency.CurrencyId == "COINS")
                {
                    tokens = (int)currency.Balance;
                    tokenText.text = tokens.ToString();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error fetching currency balances: " + e.Message);
        }
    }
    public async Task AddGems(int amount)
    {
        try
        {
            await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("STARS", amount);
            stars += amount;
            starText.text = stars.ToString();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error adding stars: " + e.Message);
        }
    }
    public async Task AddTokens(int amount)
    {
        try
        {
            await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("COINS", amount);
            tokens += amount;
            tokenText.text = tokens.ToString();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error adding tokens: " + e.Message);
        }
    }

    public async Task<bool> SpendStars(int amount)
    {
        if (stars >= amount)
        {
            try
            {
                await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync("STARS", amount);
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
    public async Task<bool> SpendTokens(int amount)
    {
        if (tokens >= amount)
        {
            try
            {
                await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync("COINS", amount);
                tokens -= amount;
                tokenText.text = tokens.ToString();
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error spending tokens: " + e.Message);
            }
        }
        return false;
    }

    
}