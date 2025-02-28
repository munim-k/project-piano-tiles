using System.Collections;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI loadingText;
    [SerializeField] float loadingTime = 3f;
    [SerializeField] string sceneToLoad = "Start";

    void Start() {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene() {
        float elapsedTime = 0f;
        string loadingDots = "";

        while (elapsedTime < loadingTime) {
            loadingText.text = "Loading " + loadingDots;
            loadingDots += "..";
            if (loadingDots.Length > 6)
            {
                loadingDots = "";
            }
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }

}
