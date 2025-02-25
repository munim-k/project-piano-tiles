using UnityEngine;

public class levelMusic : MonoBehaviour
{
    private static levelMusic _instance;

    public static levelMusic Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<levelMusic>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(levelMusic).ToString());
                    _instance = singleton.AddComponent<levelMusic>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
