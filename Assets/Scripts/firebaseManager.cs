using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using FirebaseWebGL;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class firebaseManager : MonoBehaviour
{

    public static firebaseManager Instance;

    private static string authHeader;
    public static string idToken;
    private static string accessToken;


    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            DisplayError("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
            return;
        }
        
        FirebaseAuth.OnAuthStateChanged(gameObject.name, "DisplayUserInfo", "DisplayErrorObject");
    }

    // Handle OnAuthStateChanged. Gets the serialized user object.
    public void DisplayUserInfo(string user)
    {
        if (string.IsNullOrEmpty(user))
        {
            return;
        }
        try {
            var parsedUser = StringSerializationAPI.Deserialize(typeof(FirebaseUser), user) as FirebaseUser;
            Debug.Log(parsedUser);
            idToken = parsedUser.uid;
            DisplayData($"Email: {parsedUser.email}, UserId: {parsedUser.uid}, EmailVerified: {parsedUser.isEmailVerified}");
        } catch (System.Exception e) {
            DisplayError(e.Message);
        }
    }
    

    public void SignInWithGoogle() 
    {
        FirebaseAuth.SignInWithGoogle(gameObject.name, "DisplayInfo", "DisplayErrorObject");
        SceneManager.LoadScene("wallet");
    }
        
    public void SignInWithFacebook() {
        FirebaseAuth.SignInWithFacebook(gameObject.name, "DisplayInfo", "DisplayErrorObject");
        SceneManager.LoadScene("wallet");
    }

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
