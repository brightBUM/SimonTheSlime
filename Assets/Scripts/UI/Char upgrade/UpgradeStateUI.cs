using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStateUI : MonoBehaviour
{
    [SerializeField] UpgradeStatId upgradesStatId;
    [SerializeField] TextMeshProUGUI upgradeName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI valueChange;

    [SerializeField] TextMeshProUGUI costUI;
    [SerializeField] GameObject upgradeSetup;
    [SerializeField] GameObject maxedSetup;
    [Header("Upgrade Dots")]
    [SerializeField] Transform shadowParent;
    [SerializeField] GameObject shadowPrefab;
    [SerializeField] Transform dotParent;
    [SerializeField] GameObject dotPrefab;
    List<Transform> dotItems;
    private void Start()
    {
        Init();
    }
    IEnumerator DelayedInitialization()
    {
        yield return new WaitForSeconds(1);
        Init();
    }
    public void Init()
    {
        //get current upgrade stats from gamemanager
        var upgradeStatSO = GameManger.Instance.GetCurrentCharUpgradeStat(upgradesStatId);
        var progressIndex = SaveLoadManager.Instance.GetCharUpgradeProgress((int)upgradesStatId);
        upgradeName.text = upgradeStatSO.statId.ToString();
        description.text = upgradeStatSO.description;

        if(progressIndex!= upgradeStatSO.upgrades.Count-1)
        {
            valueChange.text = $"{upgradeStatSO.upgrades[progressIndex].value}>>{upgradeStatSO.upgrades[progressIndex + 1].value}";
            costUI.text = upgradeStatSO.upgrades[progressIndex+1].currencyAmount.amount.ToString();

            dotItems = new List<Transform>();
            for(int i=0;i<upgradeStatSO.upgrades.Count-1 ;i++)
            {
                Instantiate(shadowPrefab, shadowParent);
                var dotObject = Instantiate(dotPrefab, dotParent);
                dotItems.Add(dotObject.transform);
            }
            //yield return new WaitForEndOfFrame();
            HorizontalLayoutGroup layout = dotParent.GetComponent<HorizontalLayoutGroup>();
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(dotParent.GetComponent<RectTransform>());

            layout.enabled = false;
            for (int i = 0; i < dotItems.Count; i++)
            {
                dotItems[i].gameObject.SetActive(false);
            }
            if(progressIndex>0)
                dotItems[progressIndex - 1].gameObject.SetActive(true);
        }
        else
        {
            //show max level
            upgradeSetup.SetActive(false);
            maxedSetup.SetActive(true);
            valueChange.text = upgradeStatSO.upgrades[progressIndex].value.ToString();
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

                dotItems[progressIndex-1].gameObject.SetActive(true);
            }
            else
            {
                //show max level
                upgradeSetup.SetActive(false);
                maxedSetup.SetActive(true);
                valueChange.text = upgradeStatSO.upgrades[progressIndex].value.ToString();
            }

            saveLoadInstance.SaveGame();
        }
        else
        {
            CurrencyManager.TriggerNoCurrencyFeedBack(costAmount.currencyType);
        }
       
    }

    
}
