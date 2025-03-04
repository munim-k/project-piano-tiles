using UnityEngine;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public const int LEVELS = 6;

    public bool[] levelsUnlocked = new bool[LEVELS];
    public bool[] levelsCompleted = new bool[LEVELS];

    

    public int level;


     public async Task LoadGameData()
    {
        try
        {
        (levelsUnlocked, levelsCompleted) = await LoadLevelData(LEVELS);
        }

        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading level data: {ex.Message}");
        }
        for (int i = 0; i < LEVELS; i++)
        {
            Debug.Log($"Level {i + 1}: Unlocked = {levelsUnlocked[i]}, Completed = {levelsCompleted[i]}");
        }
    }

    public static async Task SaveLevelData(bool[] levelsUnlocked, bool[] levelsCompleted)
    {
        try
        {
            
            List<int> unlockedList = new List<int>();
            List<int> completedList = new List<int>();

            foreach (bool unlocked in levelsUnlocked)
                unlockedList.Add(unlocked ? 1 : 0);

            foreach (bool completed in levelsCompleted)
                completedList.Add(completed ? 1 : 0);

            
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                { "levels_unlocked", unlockedList },
                { "levels_completed", completedList }
            });

            Debug.Log("Level data saved to Cloud Save.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving level data: {ex.Message}");
        }
    }

    public static async Task<(bool[], bool[])> LoadLevelData(int LEVELS)
    {
        try
        {
            var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "levels_unlocked", "levels_completed" });

            bool[] levelsUnlocked = new bool[LEVELS];
            bool[] levelsCompleted = new bool[LEVELS];

            
            if (savedData.TryGetValue("levels_unlocked", out var unlockedObj))
            {
                
                List<int> unlockedList = DeserializeList(unlockedObj);
                for (int i = 0; i < Mathf.Min(unlockedList.Count, LEVELS); i++)
                    if( unlockedList[i] == 1)
                        levelsUnlocked[i] = true;
            }

            
            if (savedData.TryGetValue("levels_completed", out var completedObj))
            {
                
                List<int> completedList = DeserializeList(completedObj);
                
                for (int i = 0; i < Mathf.Min(completedList.Count, LEVELS); i++)
                     if(completedList[i] == 1)
                        levelsCompleted[i] = true;
            }

            Debug.Log("Level data loaded from Cloud Save.");
            return (levelsUnlocked, levelsCompleted);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading level data: {ex.Message}");
            return (new bool[LEVELS], new bool[LEVELS]); 
        }
    }

    private static List<int> DeserializeList(object data)
    {
        try
        {
            if (data is IList<object> objectList)
            {
                List<int> intList = new List<int>();
                foreach (var item in objectList)
                    intList.Add(System.Convert.ToInt32(item));
                return intList;
            }
            else if (data is string jsonString)
            {
                return JsonConvert.DeserializeObject<List<int>>(jsonString) ?? new List<int>();
            }
            else if (data is int singleValue)
            {
                return new List<int> { singleValue };
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error deserializing list: {ex.Message}");
        }

        return new List<int>();
    }


    private async Task saveData()
    {
        await SaveLevelData(levelsUnlocked, levelsCompleted);
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        InitializeGameData();
    }
    private async Task InitializeGameData()
    {
        await LoadGameData();
    }

    public async Task UnlockLevel(int level)
    {
        levelsUnlocked[level] = true;
        await saveData();
        
    }

    public async Task CompleteLevel(int level)
    {
        levelsCompleted[level] = true;
        await saveData();
    }

   

}