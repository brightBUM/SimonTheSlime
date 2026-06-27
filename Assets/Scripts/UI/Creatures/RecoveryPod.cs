using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Services.LevelPlay;

public enum PodState
{
    Buy,        //one time state 
    Upgrade,    //vacant state where you can upgrade pod to inc recovery speed 
    Assigned,   //assigned state - show recovery timer
    Recovered   //recovery complete - tap to init creature animation - new & existing
}
public class RecoveryPod : MonoBehaviour,IDropHandler
{
    [SerializeField] int podId;
    [Header("BuySetup")]
    [SerializeField] GameObject buySetup;
    [SerializeField] Button buyButton;
    [SerializeField] TextMeshProUGUI buyText;
    [Header("Pod level ref")]
    [SerializeField] GameObject[] podVisualLevels;
    [SerializeField] GameObject[] levelsNums;
    [Header("Vacant Setup")]
    [SerializeField] GameObject podLevelsSetup;
    [SerializeField] GameObject vacantSetup;
    [SerializeField] Button upgradeButton;
    [SerializeField] Transform currencyItemParent;
    [SerializeField] CurrencyItemUI currencyItemUI;
    [SerializeField] TextMeshProUGUI speedUpgradeText;
    // current upgrade speed text
    // upgrade progress
    [Header("Assigned Setup")]
    [SerializeField] GameObject assignedSetup;
    [SerializeField] Image creatureImage;
    [SerializeField] TextMeshProUGUI timeRemainingText;
    [SerializeField] TextMeshProUGUI gemText;
    [SerializeField] Button hardCurrencyFinishButton;
    [SerializeField] Image glassImage;
    [SerializeField] Color[] creatureColors;
    [SerializeField] GameObject bubbleVFX;
    [SerializeField] Button watchAdButton;
    [SerializeField] Button gemRecoverButton;
    [Header("Complete Setup")]
    [SerializeField] GameObject completeSetup;
    [SerializeField] Transform glowTransform;
    [SerializeField] Button completeButton;
    public PodState podState;
    public int podLevel;
    private int gemCostValue;
    private String[] upgradeTexts = { "1x >> 2x Speed", "2x >> 3x Speed", "Max Level" };
    private void OnEnable()
    {
        buyButton.onClick.       AddListener(BuyPod);
        upgradeButton.onClick.   AddListener(UpgradePod);
        completeButton.onClick.  AddListener(RecoverCreature);
        watchAdButton.onClick.   AddListener(ShowRewardedAdforHours);
        gemRecoverButton.onClick.AddListener(InstantRecoverViaGem);
    }
    public void Init(int index,RecoveryPodData recoveryPodData = null)
    {
        this.podId = index;
        if(recoveryPodData!=null)
            this.podState = recoveryPodData.GetPodState();

        var setups = new GameObject[] { buySetup, vacantSetup, assignedSetup, completeSetup };
        foreach(var item in setups)
        {
            item.SetActive(false);
        }

        
        switch (podState)
        {
            case PodState.Buy:
                buySetup.SetActive(true);
                podLevelsSetup.SetActive(false);
                //cost to buy
                buyText.text = GetPodBuyCost().ToString();
                break;
            case PodState.Upgrade:
                this.podLevel = recoveryPodData.podLevel;
                ShowUpgradeState();
                break;
            case PodState.Assigned:
                this.podLevel = recoveryPodData.podLevel;
                OnCreatureAssigned(recoveryPodData.creatureType - 1); //-1 to counter for the saveload setup
                SetPodVisualLevel();
                break;
            case PodState.Recovered:
                this.podLevel = recoveryPodData.podLevel;
                OnRecoveryComplete(recoveryPodData.creatureType - 1);
                SetPodVisualLevel();
                break;
        }
        
    }

    private int GetPodBuyCost()
    {
        return GameManger.Instance.gameConfig.podsCost[this.podId];
    }
    private void Update()
    {
        if(podState == PodState.Assigned)
        {
            var recoverPodData = SaveLoadManager.Instance.playerProfile.recoveryPodData[this.podId];
            var timeSpan = recoverPodData.GetRemainingTime();
            timeRemainingText.text = $"{timeSpan.Hours} : {timeSpan.Minutes} : {timeSpan.Seconds}";
            this.gemCostValue = CalculateGemCost(timeSpan);
            gemText.text = gemCostValue.ToString();
            
            if(recoverPodData.IsComplete())
            {
                podState = PodState.Recovered;
                assignedSetup.SetActive(false);
                completeSetup.SetActive(true);
                bubbleVFX.SetActive(false);
                glowTransform.gameObject.SetActive(true);
                SoundManager.Instance.PlayPodRecoveredClip();
                //tween glow transform same as loot drop
            }

        }
    }
    private void ShowRewardedAdforHours()
    {
        IronSourceAdManager.Instance.ShowRewardedAd();
        IronSourceAdManager.Instance.rewardedAd.OnAdRewarded += RewardedAd_OnAdRewarded;
    }
    private int CalculateGemCost(TimeSpan remaining)
    {
        if (remaining <= TimeSpan.Zero)
            return 0;

        return Mathf.Max(1, Mathf.CeilToInt((float)remaining.TotalHours));
    }
    private void InstantRecoverViaGem()
    {
        List<CurrencyAmount> currencyList = new List<CurrencyAmount>
        {
            new CurrencyAmount
            {
                currencyType = CurrencyType.Melons, amount = gemCostValue
            }
        };
        if(SaveLoadManager.Instance.CanPurchase(currencyList))
        {
            SaveLoadManager.Instance.playerProfile.recoveryPodData[this.podId].ApplyGemBoost();
            SoundManager.Instance.PlayPurchaseSFX();
        }
        else
        {
            CurrencyManager.TriggerNoCurrencyFeedBack(CurrencyType.Melons);
        }
    }

    private void RewardedAd_OnAdRewarded(LevelPlayAdInfo arg1, LevelPlayReward arg2)
    {
        Debug.Log("ad rewarded event");
        SaveLoadManager.Instance.playerProfile.recoveryPodData[this.podId].ApplyAdBoost();
        
        //load the next ad 
        IronSourceAdManager.Instance.LoadRewardedAd();
        IronSourceAdManager.Instance.rewardedAd.OnAdRewarded -= RewardedAd_OnAdRewarded;
    }

    
    private void OnRecoveryComplete(int creatureType)
    {
        vacantSetup.SetActive(false);
        completeSetup.SetActive(true);
        creatureImage.enabled = true;
        creatureImage.sprite = GameManger.Instance.GetCreatureSprite((CreatureType)creatureType);
        glassImage.color = creatureColors[creatureType];
        glowTransform.gameObject.SetActive(true);

    }
    private void ShowUpgradeState()
    {
        vacantSetup.SetActive(true);
        //to do - show level of pod
        SetPodVisualLevel();
        //upgrade speed
        speedUpgradeText.text = upgradeTexts[this.podLevel-1];

        //cost to upgrade
        CalculateCostToUpgrade();
    }
    private void SetPodVisualLevel()
    {
        ToggleItem(podVisualLevels, this.podLevel - 1);
        ToggleItem(levelsNums, this.podLevel - 1);
    }
    private void CalculateCostToUpgrade()
    {
        var gameManagerInstance = GameManger.Instance;
        //get from game config
        var costList = gameManagerInstance.GetRecoveryPodUpgradeAmount(this.podLevel);

        if(costList!=null)
        {
            foreach(Transform child in currencyItemParent)
            {
                Destroy(child.gameObject);
            }
            foreach (var cost in costList)
            {
                //sprite from gameManager
                var sprite = gameManagerInstance.GetCurrencyIcon((int)cost.currencyType);
                //spawn on UI button
                var currencyItemUIObject = Instantiate(currencyItemUI, currencyItemParent);
                currencyItemUIObject.SetCurrencyData(sprite, cost.amount.ToString());
            }
        }
        else
        {
            //if costList is null
            if(podLevel==3)
            {
                upgradeButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("invalid Pod Level Error");
            }
        }
        
    }
    
    //unlock / buy this pod
    public void BuyPod()
    {
        //decrement cost
        var cost = GetPodBuyCost();
        List<CurrencyAmount> currencyList = new List<CurrencyAmount>
        {
            new CurrencyAmount
            {
                currencyType = CurrencyType.Nanas, amount = cost
            }
        };
        if(SaveLoadManager.Instance.CanPurchase(currencyList))
        {
            this.podLevel = 1;
            buySetup.SetActive(false);
            podLevelsSetup.SetActive(true);
            podState = PodState.Upgrade;
            ShowUpgradeState();
            SaveLoadManager.Instance.BuyNewPod();
            SoundManager.Instance.PlayPurchaseSFX();
        }
        else
        {
            CurrencyManager.TriggerNoCurrencyFeedBack(CurrencyType.Nanas);

        }

    }
    //upgrade pod
    public void UpgradePod()
    {
        //check if canAfford
        var costList = GameManger.Instance.GetRecoveryPodUpgradeAmount(this.podLevel);
        if(SaveLoadManager.Instance.CanPurchase(costList))
        {
            if (this.podLevel < 3) //update local variable first , then saveload
                podLevel++;
            //update text for the current upgrade progress
            SaveLoadManager.Instance.UpgradePod(podId);
            SoundManager.Instance.PlayUpgradeClip();
            ShowUpgradeState();
        }
        else
        {
            //not affordable - currency panel tween
            foreach(var cost in costList)
            {
                CurrencyManager.TriggerNoCurrencyFeedBack(cost.currencyType);
            }
        }
    }

    public void ToggleItem(GameObject[] items, int index)
    {
        foreach (var item in items)
        {
            item.SetActive(false);
        }
        items[index].SetActive(true);
    }
    public void OnPointerEnter(PointerEventData eventData) { Debug.Log("pointer entered"); }
    public void OnPointerExit(PointerEventData eventData) { Debug.Log("pointer exited"); }

    public void OnDrop(PointerEventData eventData)
    {
        if (podState != PodState.Upgrade)
            return;
        // Check if we dragged something with an Image
        var inventorySlot = eventData.pointerDrag?.GetComponent<InventorySlotInteractable>();
        if (inventorySlot != null)
        {
            // get creature Type
            OnCreatureAssigned((int)inventorySlot.creatureType);

            // clear inventory slot
            inventorySlot.MarkAsDropped();

            SaveLoadManager.Instance.AssignCreature(podId, (int)inventorySlot.creatureType);
        
            podState = PodState.Assigned;
            //save after transfer
            SaveLoadManager.Instance.SaveGame();
        }
    }
    //Assign creature to Pod
    public void OnCreatureAssigned(int creatureType)
    {
        vacantSetup.SetActive(false);
        assignedSetup.SetActive(true);
        creatureImage.enabled = true;
        creatureImage.sprite = GameManger.Instance.GetCreatureSprite((CreatureType)creatureType);
        glassImage.color = creatureColors[creatureType];
        bubbleVFX.SetActive(true);
    }
    //recovery complete
    public void RecoverCreature()
    {
        podState = PodState.Upgrade;
        completeSetup.SetActive(false);
        glowTransform.gameObject.SetActive(false);
        creatureImage.enabled = false;
        glassImage.color = Color.white;
        bubbleVFX.SetActive(false);
        ShowUpgradeState();

        var saveLoadInstance = SaveLoadManager.Instance;

        //add it to creature collection
        var podData = saveLoadInstance.playerProfile.recoveryPodData[podId];
        var creatureType = (CreatureType)podData.creatureType - 1;

        //reset any applied ad/currency boosts
        podData.reducedSeconds = 0;

        //to do implement - random draw logic
        var creaturePool = GameManger.Instance.gameConfig.GetCreatureList((int)creatureType);
        var randomItem = UnityEngine.Random.Range(0, creaturePool.Count);
        //get creature stats from creatures scriptable object
        var creatureData = creaturePool[randomItem];
        //if new -- new pop up animation ,add to creature panel
        if(saveLoadInstance.CheckIfCreatureUnlocked(creatureData.creatureId))
        {
            // trigger new reveal
            CreaturesPanel.Instance.creatureReveal.TriggerNewReveal(creatureData);
            CreaturesPanel.Instance.RefreshAllStickers();
        }
        else
        {
            // else -- simple pop up (To-do)
            CreaturesPanel.Instance.creatureReveal.TriggerExistingReveal(creatureData);

        }

        saveLoadInstance.CompleteRecovery(podId);
        saveLoadInstance.SaveGame();

    }
    

    private void OnDisable()
    {
        buyButton.onClick.       RemoveListener(BuyPod);
        upgradeButton.onClick.   RemoveListener(UpgradePod);
        completeButton.onClick.  RemoveListener(RecoverCreature);
        watchAdButton.onClick.   RemoveListener(ShowRewardedAdforHours);
        gemRecoverButton.onClick.RemoveListener(InstantRecoverViaGem);

    }


}
public static class RecoveryTimeConfig
{
    // Base times in hours
    // to do plug values to gameconfig , remote config
    private static readonly Dictionary<int, TimeSpan> baseTimes =
        new Dictionary<int, TimeSpan>
        {
            { 0,   TimeSpan.FromSeconds(0) },
            { 1,   TimeSpan.FromSeconds(30) },
            { 2,   TimeSpan.FromSeconds(45) },
            { 3,   TimeSpan.FromSeconds(60) }
        };

    public static TimeSpan GetBaseTime(int type)
    {
        return baseTimes[type];
    }

    public static TimeSpan GetAdjustedTime(int type, int podLevel)
    {
        TimeSpan baseTime = GetBaseTime(type);

        // Example multipliers: lv1 = 1x, lv2 = 0.5x, lv3 = 0.33x
        float multiplier = podLevel switch
        {
            1 => 1f,
            2 => 0.5f,
            3 => 0.33f,
            _ => 1f
        };

        return TimeSpan.FromTicks((long)(baseTime.Ticks * multiplier));
    }
}
[System.Serializable]
public class RecoveryPodData
{
    public int podLevel = 0;
    public int creatureType;
    public long timeAssignedTicks;         // for save/load safe serialization
    public bool isUnlocked = false;        // did the player buy this pod slot?
    public double reducedSeconds;
    public DateTime TimeAssigned
    {
        get => new DateTime(timeAssignedTicks, DateTimeKind.Utc);
        set => timeAssignedTicks = value.ToUniversalTime().Ticks;
    }

    public TimeSpan GetRecoveryDuration() =>
        RecoveryTimeConfig.GetAdjustedTime(creatureType, podLevel);

    public TimeSpan GetRemainingTime()
    {
        //if (creatureType == default) return TimeSpan.Zero; // no creature assigned
        TimeSpan duration = GetRecoveryDuration();
        TimeSpan elapsed = DateTime.UtcNow - TimeAssigned;
        TimeSpan remaining = duration - elapsed - TimeSpan.FromSeconds(reducedSeconds);
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
    public void UpgradePodLevel()
    {
        if (podLevel < 3)
            podLevel++;
    }
    public bool IsComplete() => creatureType != default && GetRemainingTime() <= TimeSpan.Zero;
    public void ApplyAdBoost()
    {
        reducedSeconds += TimeSpan.FromHours(6).TotalSeconds;
    }
    public void ApplyGemBoost()
    {
        reducedSeconds += GetRemainingTime().TotalSeconds;
    }
    // The state logic
    public PodState GetPodState()
    {
        if (!isUnlocked)
            return PodState.Buy;

        if (creatureType == 0) // no creature assigned
            return PodState.Upgrade;

        if (IsComplete())
            return PodState.Recovered;

        return PodState.Assigned;
    }

    
    public void AssignCreature(int type)
    {
        creatureType = type+1; // for enum conversion
        TimeAssigned = DateTime.UtcNow;
    }

    // claiming recovered creature
    public void ClearPod()
    {
        creatureType = default;
        timeAssignedTicks = 0;
    }
}
