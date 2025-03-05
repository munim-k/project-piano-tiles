using UnityEngine;
using UnityEngine.UI;
using Thirdweb.Unity;
using System.Threading.Tasks;

namespace Thirdweb.Unity{
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

    [SerializeField] ulong ActiveChainId = 1868;

    [SerializeField] MissionButton[] claimButtons = new MissionButton[5];

    [SerializeField] bool[] claimed = new bool[6]{false, false, false, false, false, false};

    void Start() {
        //load karwa claimed etc firebase se
        for (int i = 0; i < 5; i++) {
            claimButtons[i].Setup(false, LevelManager.Instance.levelsCompleted[i], i);
        }
        if (LevelManager.Instance.levelsCompleted[5]) {
            claimButtonImg.sprite = claimButtonSprite;
        } else {
            ColorBlock block = claimButtonImg.GetComponent<Button>().colors;
            block.disabledColor = block.normalColor;
            claimButtonImg.GetComponent<Button>().colors = block;
            claimButtonImg.GetComponent<Button>().interactable = false;
        }
    }


    public async void ClaimLevel1(string addr) {
        Debug.Log("Claiming Level 1");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[0] = true;
        claimButtons[0].Setup(claimed[0], LevelManager.Instance.levelsCompleted[0], 0);
    }

    public async void ClaimLevel2(string addr) {
        Debug.Log("Claiming Level 2");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[1] = true;
        claimButtons[1].Setup(claimed[1], LevelManager.Instance.levelsCompleted[1], 1);
    }

    public async void ClaimLevel3(string addr) {
        Debug.Log("Claiming Level 3");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[2] = true;
        claimButtons[2].Setup(claimed[2], LevelManager.Instance.levelsCompleted[2], 2);
    }

    public async void ClaimLevel4(string addr) {
        Debug.Log("Claiming Level 4");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[3] = true;
        claimButtons[3].Setup(claimed[3], LevelManager.Instance.levelsCompleted[3], 3);
    }

    public async void ClaimLevel5(string addr) {
        Debug.Log("Claiming Level 5");
        var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC721_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,1);
        claimed[4] = true;
        claimButtons[4].Setup(claimed[4], LevelManager.Instance.levelsCompleted[4], 4);
    }

    [SerializeField] Sprite claimButtonSprite;
    [SerializeField] Image claimButtonImg;
    public async void ClaimFinalReward(string addr) {
        Debug.Log("Claimed Final Reward");
        claimed[5] = true;
        claimButtonImg.sprite = claimButtonSprite;
         var contract = await ThirdwebManager.Instance.GetContract(addr, ActiveChainId);
        string address = await ThirdwebManager.Instance.GetActiveWallet().GetAddress();
        await contract.DropERC20_Claim(ThirdwebManager.Instance.GetActiveWallet(), address ,"15");
        //yahaan pe acs currency kam karwani hai
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