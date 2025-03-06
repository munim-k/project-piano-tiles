using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class firebaseManager : MonoBehaviour
{
    private void Start()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
            return;
        }
        
        FirebaseAuth.OnAuthStateChanged(gameObject.name, "DisplayUserInfo", "DisplayInfo");
    }
    

    public void SignInWithGoogle() 
    {
        FirebaseAuth.SignInWithGoogle(gameObject.name, "DisplayInfo", "DisplayErrorObject");
        SceneManager.LoadScene("wallet");
    }
        
    public void SignInWithFacebook() =>
        FirebaseAuth.SignInWithFacebook(gameObject.name, "DisplayInfo", "DisplayErrorObject");

    public void DisplayData(string data)
        {
            Debug.Log(data);
        }

        public void DisplayInfo(string info)
        {
            Debug.Log(info);
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
