using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStateUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI upgradeName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI valueChange;

    [SerializeField] TextMeshProUGUI costUI;
    [SerializeField] GameObject upgradeSetup;
    [SerializeField] GameObject doneSetup;
    [SerializeField] Button upgradeButton;
    [SerializeField] GameObject borderObject;
   
    UpgradeStatId upgradesStatId;
    int upgradeIndex;
    CurrencyAmount currencyAmount;
    public void Init(UpgradeStatId upgradesStatId,int upgradeIndex)
    {
        this.upgradesStatId = upgradesStatId;
        this.upgradeIndex = upgradeIndex;
        //get current upgrade stats from gamemanager
        var upgradeStatSO = GameManger.Instance.GetCurrentCharUpgradeStat(upgradesStatId);
        var progressIndex = SaveLoadManager.Instance.GetCharUpgradeProgress((int)upgradesStatId);

        icon.sprite = upgradeStatSO.icon;
        upgradeName.text = upgradeStatSO.name+" "+upgradeIndex;
        description.text = upgradeStatSO.description;

        valueChange.text = $"{upgradeStatSO.upgrades[this.upgradeIndex-1].value}>>{upgradeStatSO.upgrades[this.upgradeIndex].value}";
        if(progressIndex<this.upgradeIndex)
        {
            currencyAmount = upgradeStatSO.upgrades[this.upgradeIndex].currencyAmount;
            costUI.text = currencyAmount.amount.ToString();
        }
        else
        {
            //show max level
            upgradeSetup.SetActive(false);
            doneSetup.SetActive(true);
        }

    }
    public void CheckPurchased()
    {

    }
    public void UpgradeButton()
    {
        var saveLoadInstance = SaveLoadManager.Instance;
        
        List<CurrencyAmount> currencyList = new List<CurrencyAmount>
        {
            this.currencyAmount,
        };

        if (saveLoadInstance.CanPurchase(currencyList))
        {
            //inc upgrade level
            //modify saveload 
            saveLoadInstance.SetCharUpgradeProgress((int)upgradesStatId);

            //show purchased setup
            upgradeSetup.SetActive(false);
            doneSetup.SetActive(true);

            SoundManager.Instance.PlayPowerupClip();
            CharUpgradeUI.UpdateCreatureCount?.Invoke();
            saveLoadInstance.SaveGame();
        }
        else
        {
            CurrencyManager.TriggerNoCurrencyFeedBack(this.currencyAmount.currencyType);
        }
       
    }
    public void UnlockCard()
    {
        var progressIndex = SaveLoadManager.Instance.GetCharUpgradeProgress((int)upgradesStatId);

        var diff = Mathf.Abs(progressIndex - this.upgradeIndex);
        //Debug.Log($"{upgradesStatId} ,{upgradeIndex} : diff_{diff}");
        bool value = diff == 1;
        upgradeButton.interactable = value;
        borderObject.SetActive(value);
    }


}
