using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform contentTransform;
    
    public  AudioClip[] buttonClickSound;
    private AudioSource levelMusicSource;

    [SerializeField] TextMeshProUGUI levelText;
    // int currentLevel = 0; // Now stored in LevelManager

    [SerializeField] int[] prices = {5, 20, 25, 30, 37, 45};
    [SerializeField] Color[] colors = {
        new(93, 227, 120, 1), // Green
        new(166, 229, 255, 1), // Light blue
        new(224, 190, 255, 1), // Light purple
        new(224, 238, 249, 1), // White
        new(255, 237, 137, 1), // Yellow
        new(69, 141, 225, 1) // Blue
    };

    void Start()
    {
        int currentLevel = LevelManager.Instance.level;
        levelMusicSource = levelMusic.Instance.GetComponent<AudioSource>();
        levelMusicSource.clip = buttonClickSound[currentLevel];
        levelText.text = $"Level 0{currentLevel+1}";
        SpawnLevelButtons(LevelManager.LEVELS);
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

            LevelSelectButton lsb = buttonObj.GetComponent<LevelSelectButton>();
            if (lsb != null)
            {
                lsb.SetLevel(i, prices[i], LevelManager.Instance.levelsUnlocked[i], colors[i]);
            }
            
            // Safely get audio clip
            AudioClip clip = null;
            if (buttonClickSound != null && i < buttonClickSound.Length)
            {
                clip = buttonClickSound[i];
            }
            
            if (LevelManager.Instance.levelsUnlocked[i])
            {
                button.onClick.AddListener(() => LoadLevel(levelIndex, clip));
            }
            else
            {
                button.onClick.AddListener(() => BuyLevel(levelIndex));
            }
        }
    }

    private void BuyLevel(int levelIndex) {
        int price = prices[levelIndex];
        if (CurrencyManager.Instance.GetGems() >= price) {
            ShowConfirm(levelIndex, price);
        }
    }

    [SerializeField] GameObject confirmUI;
    [SerializeField] Button confirmButton;
    void ShowConfirm(int levelIndex, int price) {
        confirmUI.SetActive(true);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => ConfirmPurchase(levelIndex, price));
    }

    public void CancelPurchase() {
        confirmUI.SetActive(false);
    }

    private void ConfirmPurchase(int levelIndex, int price) {
        CurrencyManager.Instance.RemoveGems(price);
        LevelManager.Instance.levelsUnlocked[levelIndex] = true;
        // LevelManager.Instance.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void LoadLevel(int levelNumber , AudioClip buttonClickSound)
    {
        Debug.Log(buttonClickSound);
        if (levelMusicSource != null && buttonClickSound != null)
        {
            levelMusicSource.clip = buttonClickSound;
        }

        LevelManager.Instance.level = levelNumber;
        int currentLevel = levelNumber + 1;

        levelText.text = $"Level 0{currentLevel}";
        
        // Assuming your scene names are "level1", "level2", etc.
        string sceneName = $"level";
        SceneManager.LoadScene(sceneName);
    }

    public void SetLevel(int level) {
        LoadLevel(level-1, buttonClickSound[level-1]);
    }

    // public void LoadNextLevel() {
    //     currentLevel++;
    //     LoadLevel(currentLevel, buttonClickSound[currentLevel-1]);
    // }

    public void Play() {
        LoadLevel(0, buttonClickSound[0]);
    }
    
}
