using UnityEngine;
using System.Runtime.InteropServices;

public class testingJs : MonoBehaviour {

    [DllImport("__Internal")]
    private static extern void Hello();

    [DllImport("__Internal")]
    private static extern void HelloString(string str);

    
    void Start() {
        Hello();
        
        HelloString("This is a string.");
    }
}