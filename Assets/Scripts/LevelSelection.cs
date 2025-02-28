using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform contentTransform;
    
    [SerializeField] private int numberOfLevels = 6;
    public  AudioClip[] buttonClickSound;
    private AudioSource levelMusicSource;

    [SerializeField] TextMeshProUGUI levelText;
    int currentLevel = 1;

    void Start()
    {
        currentLevel = levelMusic.Instance.level + 1;
        levelMusicSource = levelMusic.Instance.GetComponent<AudioSource>();
        levelMusicSource.clip = buttonClickSound[currentLevel-1];
        levelText.text = $"Level 0{currentLevel}";
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
            int levelIndex = i;
            
            // Safely get audio clip
            AudioClip clip = null;
            if (buttonClickSound != null && i < buttonClickSound.Length)
            {
                clip = buttonClickSound[i];
            }
            
            button.onClick.AddListener(() => LoadLevel(levelIndex, clip));
        }
    }

    private void LoadLevel(int levelNumber , AudioClip buttonClickSound)
    {
        Debug.Log(buttonClickSound);
        if (levelMusicSource != null && buttonClickSound != null)
        {
            levelMusicSource.clip = buttonClickSound;
        }

        currentLevel = levelNumber + 1;

        levelText.text = $"Level 0{currentLevel}";

        levelMusic.Instance.level = levelNumber;
        
        // // Assuming your scene names are "level1", "level2", etc.
        // string sceneName = $"level{levelNumber+1}";
        // SceneManager.LoadScene(sceneName);
    }

    public void Play() {
        string sceneName = $"level{currentLevel}";
        SceneManager.LoadScene(sceneName);
    }
    
}
