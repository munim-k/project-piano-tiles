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
    // int currentLevel = 0; // Now stored in FirebaseLevelManager

    [SerializeField] int[] prices = {5, 20, 25, 30, 37, 45};
    [SerializeField] public Color[] colors = {
        new(93, 227, 120, 255), // Green
        new(166, 229, 255, 255), // Light blue
        new(224, 190, 255, 255), // Light purple
        new(224, 238, 249, 255), // White
        new(255, 237, 137, 255), // Yellow
        new(69, 141, 225, 255) // Blue
    };

    void Start()
    {
        int currentLevel = FirebaseLevelManager.Instance.level;
        levelMusicSource = levelMusic.Instance.GetComponent<AudioSource>();
        levelMusicSource.clip = buttonClickSound[currentLevel];
        levelText.text = $"Level 0{currentLevel+1}";
        SpawnLevelButtons(FirebaseLevelManager.LEVELS);
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
                lsb.SetLevel(i, prices[i], FirebaseLevelManager.Instance.levelsUnlocked[i], colors[i]);
            }
            
            // Safely get audio clip
            AudioClip clip = null;
            if (buttonClickSound != null && i < buttonClickSound.Length)
            {
                clip = buttonClickSound[i];
            }
            
            if (FirebaseLevelManager.Instance.levelsUnlocked[i])
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
        if (FirebaseCurrencyManager.Instance.GetStars() >= price) {
            // TODO: Show confirm dialog
            // ShowConfirm(levelIndex, price); 

            ConfirmPurchase(levelIndex, price);
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
        if (confirmUI)
            confirmUI.SetActive(false);
        FirebaseCurrencyManager.Instance.RemoveStars(price);
        FirebaseLevelManager.Instance.UnlockLevel(levelIndex);

        // Update button
        GameObject buttonObj = contentTransform.Find($"Level_{levelIndex}_Button").gameObject;
        LevelSelectButton lsb = buttonObj.GetComponent<LevelSelectButton>();
        lsb.SetLevel(levelIndex, price, true, colors[levelIndex]);

        // Set loading level
        Button button = buttonObj.GetComponent<Button>();
        AudioClip clip = null;
        if (buttonClickSound != null && levelIndex < buttonClickSound.Length)
        {
            clip = buttonClickSound[levelIndex];
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => LoadLevel(levelIndex, clip));
    }

    private void LoadLevel(int levelNumber , AudioClip buttonClickSound)
    {
        Debug.Log(buttonClickSound);
        if (levelMusicSource != null && buttonClickSound != null)
        {
            levelMusicSource.clip = buttonClickSound;
        }

        FirebaseLevelManager.Instance.level = levelNumber;
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
