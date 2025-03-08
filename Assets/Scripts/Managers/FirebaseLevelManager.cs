using UnityEngine;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

public class FirebaseLevelManager : MonoBehaviour
{
    public static FirebaseLevelManager Instance;

    public const int LEVELS = 6;

    public bool[] levelsUnlocked = new bool[LEVELS];
    public bool[] levelsCompleted = new bool[LEVELS];

    public int level;

    public void SaveLevelData(bool[] levelsUnlocked, bool[] levelsCompleted)
    {
        try
        {

            // string levelsUnlockedString = "[";
            // string levelsCompletedString = "[";

            // for (int i = 0; i < levelsUnlocked.Length; i++)
            // {
            //     levelsUnlockedString += levelsUnlocked[i] ? "true" : "false";
            //     levelsCompletedString += levelsCompleted[i] ? "true" : "false";

            //     if (i < levelsUnlocked.Length - 1)
            //     {
            //         levelsUnlockedString += ",";
            //         levelsCompletedString += ",";
            //     }
            // }

            // levelsUnlockedString += "]";
            // levelsCompletedString += "]";

            string levelsUnlockedString = StringSerializationAPI.Serialize(typeof(bool[]), levelsUnlocked);
            string levelsCompletedString = StringSerializationAPI.Serialize(typeof(bool[]), levelsCompleted);

            Debug.Log("Levels Unlocked String: " + levelsUnlockedString);
            Debug.Log("Levels Completed String: " + levelsCompletedString);
            
            FirebaseDatabase.UpdateJSON($"users/{firebaseManager.idToken}/levelsUnlocked", levelsUnlockedString, gameObject.name, null, null);
            FirebaseDatabase.UpdateJSON($"users/{firebaseManager.idToken}/levelsCompleted", levelsCompletedString, gameObject.name, null, null);

            Debug.Log("Level data saved to Firebase.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error saving level data: {ex.Message}");
        }
    }

    private void saveData()
    {
        SaveLevelData(levelsUnlocked, levelsCompleted);
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
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");

        FetchLevelData();
    }

    // Firebase Functions
    void FetchLevelData()
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

    public void SetUserData(string data) {
        var userData = StringSerializationAPI.Deserialize(typeof(UserData), data) as UserData;
        for (int i = 0; i < LEVELS; i++)
        {
            levelsUnlocked[i] = userData.levelsUnlocked[i];
            levelsCompleted[i] = userData.levelsCompleted[i];
        }
    }

    public void UnlockLevel(int level)
    {
        levelsUnlocked[level] = true;
        saveData();
        
    }

    public void CompleteLevel(int level)
    {
        levelsCompleted[level] = true;
        saveData();
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

}