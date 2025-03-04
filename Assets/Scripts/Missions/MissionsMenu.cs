using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Thirdweb;

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

    [SerializeField] MissionButton[] claimButtons = new MissionButton[5];

    [SerializeField] bool[] claimed = new bool[6];

    void Start() {
        for (int i = 0; i < 5; i++) {
            claimButtons[i].Setup(claimed[i], LevelManager.Instance.levelsCompleted[i], i);
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

    public async Task ClaimLevel1(string NFT_ADDR) {
        Debug.Log("Claiming Level 1 NFT.......");
        var contract = ThirdwebManager.Instance.SDK.GetContract(NFT_ADDR);
        var balance = await contract.ERC721.BalanceOf(ThirdwebManager.Instance.connectedWalletAddress);
        if (balance == "0") {
            Debug.Log("Minting NFT.......");
            var result = await contract.ERC721.ClaimTo(ThirdwebManager.Instance.connectedWalletAddress, 1);
            claimed[0] = true;
            claimButtons[0].Setup(claimed[0], LevelManager.Instance.levelsCompleted[0], 0);
            Debug.Log("NFT successfully minted");
        } else {
            Debug.Log("NFT already minted");
        }
        
    }

    public void ClaimLevel2(string NFT_ADDR) {
        Debug.Log("Claimed Level 2");
        claimed[1] = true;
        claimButtons[1].Setup(claimed[1], LevelManager.Instance.levelsCompleted[1], 1);
    }

    public void ClaimLevel3(string NFT_ADDR) {
        Debug.Log("Claimed Level 3");
        claimed[2] = true;
        claimButtons[2].Setup(claimed[2], LevelManager.Instance.levelsCompleted[2], 2);
    }

    public void ClaimLevel4(string NFT_ADDR) {
        Debug.Log("Claimed Level 4");
        claimed[3] = true;
        claimButtons[3].Setup(claimed[3], LevelManager.Instance.levelsCompleted[3], 3);
    }

    public void ClaimLevel5(string NFT_ADDR) {
        Debug.Log("Claimed Level 5");
        claimed[4] = true;
        claimButtons[4].Setup(claimed[4], LevelManager.Instance.levelsCompleted[4], 4);
    }

    [SerializeField] Sprite claimButtonSprite;
    [SerializeField] Image claimButtonImg;
    public void ClaimFinalReward() {
        Debug.Log("Claimed Final Reward");
        claimed[5] = true;
        claimButtonImg.sprite = claimButtonSprite;
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