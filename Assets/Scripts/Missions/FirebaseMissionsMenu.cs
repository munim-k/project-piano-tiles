using UnityEngine;
using UnityEngine.UI;
using Thirdweb.Unity;
// using System.Threading.Tasks;
using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

namespace Thirdweb.Unity{
public class FirebaseMissionsMenu : MonoBehaviour
{
    string[] contracts = new string[6] {
        "Test",
        "Test",
        "Test",
        "Test",
        "Test",
        "Test"
    };

    [SerializeField] ulong ActiveChainId = 1868;

    [SerializeField] MissionButton[] claimButtons = new MissionButton[5];

    [SerializeField] bool[] claimed = new bool[6]{false, false, false, false, false, false};

    void Start() {
        //load karwa claimed etc firebase se

        Debug.Log("FirebaseMissionsMenu Start");
        FirebaseDatabase.GetJSON($"users/{firebaseManager.idToken}", gameObject.name, nameof(HandleGetClaimed), null);

        // for (int i = 0; i < 5; i++) {
        //     claimButtons[i].Setup(false, FirebaseLevelManager.Instance.levelsCompleted[i], i);
        // }
        // if (FirebaseLevelManager.Instance.levelsCompleted[5]) {
        //     claimButtonImg.sprite = claimButtonSprite;
        // } else {
        //     ColorBlock block = claimButtonImg.GetComponent<Button>().colors;
        //     block.disabledColor = block.normalColor;
        //     claimButtonImg.GetComponent<Button>().colors = block;
        //     claimButtonImg.GetComponent<Button>().interactable = false;
        // }
    }

    void HandleGetClaimed(string data) {
        // Debug.Log("Claimed Data: " + data);
        if (string.IsNullOrEmpty(data)) {
            return;
        }
        try {
            var parsedData = StringSerializationAPI.Deserialize(typeof(UserData), data) as UserData;
            claimed = parsedData.levelsClaimed;
            bool[] levelsCompleted = parsedData.levelsCompleted;
            for (int i = 0; i < 5; i++) {
                claimButtons[i].Setup(claimed[i], levelsCompleted[i], i);
            }
            if (levelsCompleted[5]) {
                claimButtonImg.sprite = claimButtonSprite;
            } else {
                ColorBlock block = claimButtonImg.GetComponent<Button>().colors;
                block.disabledColor = block.normalColor;
                claimButtonImg.GetComponent<Button>().colors = block;
                claimButtonImg.GetComponent<Button>().interactable = false;
            }
        } catch (System.Exception e) {
            Debug.LogError(e.Message);
        }
    }

    void SaveClaimedData() {
        // bool[] data;
        // data = claimed;
        string dataString = StringSerializationAPI.Serialize(typeof(bool[]), claimed);
        FirebaseDatabase.UpdateJSON($"users/{firebaseManager.idToken}/levelsClaimed", dataString, gameObject.name, null, null);
    }


    public async void ClaimLevel1(string addr) {
        Debug.Log("Claiming Level 1");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[0] = true;
        claimButtons[0].Setup(claimed[0], FirebaseLevelManager.Instance.levelsCompleted[0], 0);

        SaveClaimedData();
    }

    public async void ClaimLevel2(string addr) {
        Debug.Log("Claiming Level 2");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[1] = true;
        claimButtons[1].Setup(claimed[1], FirebaseLevelManager.Instance.levelsCompleted[1], 1);

        SaveClaimedData();
    }

    public async void ClaimLevel3(string addr) {
        Debug.Log("Claiming Level 3");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[2] = true;
        claimButtons[2].Setup(claimed[2], FirebaseLevelManager.Instance.levelsCompleted[2], 2);

        SaveClaimedData();
    }

    public async void ClaimLevel4(string addr) {
        Debug.Log("Claiming Level 4");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[3] = true;
        claimButtons[3].Setup(claimed[3], FirebaseLevelManager.Instance.levelsCompleted[3], 3);

        SaveClaimedData();
    }

    public async void ClaimLevel5(string addr) {
        Debug.Log("Claiming Level 5");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[4] = true;
        claimButtons[4].Setup(claimed[4], FirebaseLevelManager.Instance.levelsCompleted[4], 4);

        SaveClaimedData();
    }

    [SerializeField] Sprite claimButtonSprite;
    [SerializeField] Image claimButtonImg;
    public void ClaimFinalReward() {
        //Redirect user to claim ACS points on web url     
        Application.OpenURL("https://www.metakraft.ai/start");   
    }


    [SerializeField] GameObject bottomBarManager;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject missionsMenu;
    [SerializeField] GameObject settingsMenu;

    public void Back() {
        bottomBarManager.SetActive(true);
        mainMenu.SetActive(true);
        settingsMenu.SetActive(true);
        missionsMenu.SetActive(false);
    }


}
}