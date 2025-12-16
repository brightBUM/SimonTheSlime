using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [Header("Vacant Setup")]
    [SerializeField] GameObject vacantSetup;
    [SerializeField] Button upgradeButton;
    // current upgrade speed text
    // upgrade progress
    [Header("Assigned Setup")]
    [SerializeField] GameObject assignedSetup;
    [SerializeField] Image creatureImage;
    [SerializeField] TextMeshProUGUI timeRemainingText;
    [SerializeField] Button hardCurrencyFinishButton;
    [Header("Complete Setup")]
    [SerializeField] GameObject completeSetup;
    [SerializeField] Button completeButton;
    public PodState podState;
    private void OnEnable()
    {
        buyButton.onClick.     AddListener(BuyPod);
        upgradeButton.onClick. AddListener(UpgradePod);
        completeButton.onClick.AddListener(RecoverCreature);
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
                //cost to buy
                break;
            case PodState.Upgrade:
                ShowUpgradeState();
                break;
            case PodState.Assigned:
                assignedSetup.SetActive(true);
                creatureImage.sprite = GameManger.Instance.GetCreatureSprite((CreatureType)(recoveryPodData.creatureType-1));  //-1 to counter for the saveload setup
                break;
            case PodState.Recovered:
                completeSetup.SetActive(true);
                break;
        }
        
    }
    private void Update()
    {
        if(podState == PodState.Assigned)
        {
            var recoverPodData = SaveLoadManager.Instance.playerProfile.recoveryPodData;
            var timeSpan = recoverPodData[podId].GetRemainingTime();
            timeRemainingText.text = $"{timeSpan.Hours} : {timeSpan.Minutes} : {timeSpan.Seconds}";

            if(recoverPodData[podId].IsComplete())
            {
                podState = PodState.Recovered;
                assignedSetup.SetActive(false);
                completeSetup.SetActive(true);
            }

        }
    }

    private void ShowUpgradeState()
    {
        vacantSetup.SetActive(true);
        //to do - show level of pod
        //upgrade speed
        //cost to upgrade
    }
    //unlock / buy this pod
    public void BuyPod()
    {
        buySetup.SetActive(false);
        vacantSetup.SetActive(true);
        podState = PodState.Upgrade;
        SaveLoadManager.Instance.BuyNewPod();
    }
    //upgrade pod
    public void UpgradePod()
    {
        //update text for the current upgrade progress
        SaveLoadManager.Instance.UpgradePod(podId);
    }
    public void OnPointerEnter(PointerEventData eventData) { Debug.Log("pointer entered"); }
    public void OnPointerExit(PointerEventData eventData) { Debug.Log("pointer exited"); }

    public void OnDrop(PointerEventData eventData)
    {
        if (podState != PodState.Upgrade)
            return;
        // Check if we dragged something with an Image
        var inventorySlot = eventData.pointerDrag?.GetComponent<InventorySlot>();
        if (inventorySlot != null)
        {
            // get creature Type
            OnCreatureAssigned((int)inventorySlot.creatureType);

            // clear inventory slot
            inventorySlot.MarkAsDropped();

            //save after transfer
            SaveLoadManager.Instance.SaveGame();
        }
    }
    //Assign creature to Pod
    public void OnCreatureAssigned(int creatureType)
    {
        vacantSetup.SetActive(false);
        assignedSetup.SetActive(true);
        creatureImage.sprite = GameManger.Instance.GetCreatureSprite((CreatureType)creatureType);
        
        SaveLoadManager.Instance.AssignCreature(podId,creatureType);
        
        podState = PodState.Assigned;
    }
    //recovery complete
    public void RecoverCreature()
    {
        podState = PodState.Upgrade;
        completeSetup.SetActive(false);
        ShowUpgradeState();

        //add it to creature collection
        var creatureType = (CreatureType)SaveLoadManager.Instance.playerProfile.recoveryPodData[podId].creatureType - 1;

        //to do implement - random draw logic
        //get creature stats from creatures scriptable object
        //if new -- new pop up animation ,add to creature panel
        //else -- simple pop up

        SaveLoadManager.Instance.CompleteRecovery(podId);
        SaveLoadManager.Instance.SaveGame();

    }

    private void OnDisable()
    {
        buyButton.onClick.     RemoveListener(BuyPod);
        upgradeButton.onClick. RemoveListener(UpgradePod);
        completeButton.onClick.RemoveListener(RecoverCreature);
    }

    
}
public static class RecoveryTimeConfig
{
    // Base times in hours
    private static readonly Dictionary<int, TimeSpan> baseTimes =
        new Dictionary<int, TimeSpan>
        {
            { 0, TimeSpan.FromSeconds(0) },
            { 1, TimeSpan.FromSeconds(3) },
            { 2,   TimeSpan.FromSeconds(5) },
            { 3,   TimeSpan.FromSeconds(10) }
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
        TimeSpan remaining = duration - elapsed;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }
    public void UpgradePodLevel()
    {
        if (podLevel < 3)
            podLevel++;
    }
    public bool IsComplete() => creatureType != default && GetRemainingTime() <= TimeSpan.Zero;

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
