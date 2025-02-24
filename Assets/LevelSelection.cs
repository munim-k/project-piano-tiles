using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform contentTransform;
    
    void Start()
    {
        SpawnLevelButtons();
    }

    private void SpawnLevelButtons()
    {
        for (int i = 1; i <= 10; i++)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, contentTransform);
            buttonObj.name = $"Level_{i}_Button";
            // load game level through button click
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
