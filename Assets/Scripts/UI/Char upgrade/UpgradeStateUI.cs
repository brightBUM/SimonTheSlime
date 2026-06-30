using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStateUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upgradeName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI valueChange;

    [SerializeField] TextMeshProUGUI costUI;
    [SerializeField] GameObject upgradeSetup;
    [SerializeField] GameObject doneSetup;
    [SerializeField] Button upgradeButton;
    [SerializeField] GameObject borderObject;
   
    UpgradeStatId upgradesStatId;
   
    
    public void Init(UpgradeStatId upgradesStatId,int upgradeIndex)
    {
        this.upgradesStatId = upgradesStatId;
        //get current upgrade stats from gamemanager
        var upgradeStatSO = GameManger.Instance.GetCurrentCharUpgradeStat(upgradesStatId);
        var progressIndex = SaveLoadManager.Instance.GetCharUpgradeProgress((int)upgradesStatId);
        upgradeName.text = upgradeStatSO.name+" "+upgradeIndex;
        description.text = upgradeStatSO.description;

        if(progressIndex!= upgradeStatSO.upgrades.Count-1)
        {
            valueChange.text = $"{upgradeStatSO.upgrades[progressIndex].value}>>{upgradeStatSO.upgrades[progressIndex + 1].value}";
            costUI.text = upgradeStatSO.upgrades[progressIndex+1].currencyAmount.amount.ToString();

        }
        else
        {
            //show max level
            upgradeSetup.SetActive(false);
            doneSetup.SetActive(true);
        }

    }
    public void UpgradeButton()
    {
        var saveLoadInstance = SaveLoadManager.Instance;
        var upgradeStatSO = GameManger.Instance.GetCurrentCharUpgradeStat(upgradesStatId);
        var progressIndex = saveLoadInstance.GetCharUpgradeProgress((int)upgradesStatId);
        var costAmount = upgradeStatSO.upgrades[progressIndex + 1].currencyAmount;
        List<CurrencyAmount> currencyList = new List<CurrencyAmount>
        {
            costAmount
        };

        if (saveLoadInstance.CanPurchase(currencyList))
        {
            //inc upgrade level
            //modify saveload 
            saveLoadInstance.SetCharUpgradeProgress((int)upgradesStatId);
            //updatedIndex;
            progressIndex = saveLoadInstance.GetCharUpgradeProgress((int)upgradesStatId);

            //update the UI
            if (progressIndex != upgradeStatSO.upgrades.Count - 1)
            {
                valueChange.text = $"{upgradeStatSO.upgrades[progressIndex].value}>>{upgradeStatSO.upgrades[progressIndex + 1].value}";
                costUI.text = upgradeStatSO.upgrades[progressIndex + 1].currencyAmount.amount.ToString();

            }
            else
            {
                //show max level
                upgradeSetup.SetActive(false);
                doneSetup.SetActive(true);
            }

            saveLoadInstance.SaveGame();
        }
        else
        {
            CurrencyManager.TriggerNoCurrencyFeedBack(costAmount.currencyType);
        }
       
    }
    public void UnlockCard()
    {
        upgradeButton.interactable = true;
        borderObject.SetActive(true);
    }


}
