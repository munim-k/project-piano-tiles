using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform contentTransform;
    
    [SerializeField] private int numberOfLevels = 10;
    public  AudioClip[] buttonClickSound;
    private AudioSource levelMusic;

    void Start()
    {
        levelMusic = GameObject.Find("levelMusic").GetComponent<AudioSource>();
        SpawnLevelButtons(numberOfLevels);
    }

    private void SpawnLevelButtons(int levels)
    {
        for (int i = 0; i < levels; i++)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, contentTransform);
            buttonObj.name = $"Level_{i}_Button";
            
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = $"Level {i + 1}";
            }

            Button button = buttonObj.GetComponent<Button>();
            
            
            // Safely get audio clip
            AudioClip clip = null;
            if (buttonClickSound != null && i < buttonClickSound.Length)
            {
                clip = buttonClickSound[i];
            }
            
            button.onClick.AddListener(() => LoadLevel( clip));
        }
    }

    private void LoadLevel( AudioClip buttonClickSound)
    {
        if (levelMusic != null && buttonClickSound != null)
        {
            levelMusic.clip = buttonClickSound;
        }
        
        // Assuming your scene names are "Level1", "Level2", etc.
        string sceneName = $"level";
        SceneManager.LoadScene(sceneName);
    }
    
}
