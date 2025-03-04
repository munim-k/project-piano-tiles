using UnityEngine;
using UnityEngine.UI;

public class Missions : MonoBehaviour
{
    string[] contracts = new string[6] {
        "Test",
        "Test",
        "Test",
        "Test",
        "Test",
        "Test"
    };


    Button[] claimButtons = new Button[6];

    void Start() {
        // Setup contracts for buttons
    }

    public void ClaimLevel1() {
        Debug.Log("Claimed Level 1");
    }

    public void ClaimLevel2() {
        Debug.Log("Claimed Level 2");
    }

    public void ClaimLevel3() {
        Debug.Log("Claimed Level 3");
    }

    public void ClaimLevel4() {
        Debug.Log("Claimed Level 4");
    }

    public void ClaimLevel5() {
        Debug.Log("Claimed Level 5");
    }

    public void ClaimLevel6() {
        Debug.Log("Claimed Level 6");
    }

    public void ClaimFinalReward() {
        Debug.Log("Claimed Final Reward");
    }


}