
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPage : MonoBehaviour
{
    [SerializeField] GameObject screwPart;
    [SerializeField] GameObject batterPart;
    [SerializeField] TextMeshProUGUI screwText;
    [SerializeField] TextMeshProUGUI batteryText;
    [SerializeField] Button unlockButton;
    [SerializeField] GameObject lockedPanel;
    public Transform pagePanelParent;
    int screwRemainValue;
    int batteryRemainValue;
    // Start is called before the first frame update
    void Start()
    {
        unlockButton.onClick.AddListener(UnlockLevelPage);
    }
    public void Init(int num)
    {
        //if already unlocked , disable the panel
        var playerProfile = SaveLoadManager.Instance.playerProfile;
        var unlocked = num <= playerProfile.pageUnlockProgress;
        if(unlocked)
        {
            lockedPanel.SetActive(false);
        }
        else
        {
            //else on start show the update part to unlock text
            //get parts from scriptable object
            var costList = GameManger.Instance.gameConfig.UnlockCosts.GetPageUnlockCost(num);
            if (costList == null)
                Debug.LogError("invalid level page / page unlock progress");
            
            var screwValue = costList[0].amount;
            if (screwValue > 0)
            {
                screwPart.SetActive(true);
                screwRemainValue = screwValue - playerProfile.screws;
                screwRemainValue = screwRemainValue<=0 ? 0 : screwRemainValue;
                screwText.text = screwValue.ToString();
            }

            var batterValue = costList[1].amount;
            if(batterValue > 0)
            {
                batterPart.SetActive(true);
                batteryRemainValue = batterValue - playerProfile.batteries;
                batteryRemainValue = batteryRemainValue<=0 ? 0 : batteryRemainValue;
                batteryText.text = batterValue.ToString();
            }

            if(screwRemainValue+batteryRemainValue<=0)
            {
                //set unlock button interactable
                unlockButton.interactable = true;   
            }
        }
    }

    

    public void UnlockLevelPage()
    {
        //to do unlock vfx 
        lockedPanel.SetActive(false);
        //decrement part from profile
        LevelSelectionScreen.Instance.UnlockNextPage();
    }
    private void OnDestroy()
    {
        unlockButton.onClick.RemoveListener(UnlockLevelPage);

    }
}
