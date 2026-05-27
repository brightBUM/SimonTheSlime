using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameConfig")]
public class GameConfig : SerializedScriptableObject
{
    [Header("Default Save File")]
    public string ProfileName = "default";
    public int nanasCount = 500;
    public int melonsCount = 50;
    public int perfectJumpBase = 25;
    public int melonDropChance = 20;

    [Header("Ads Setting")]
    public int interstitialAdCheckPerLevel;
    public int mainMenuRewardedAdNanas;
    public int RetryNanasCost = 100;


    [Header("Level Page Unlock Costs")]
    public UnlockCosts UnlockCosts;
    [Header("Creature Data")]
    public List<CreatureData> commonData;
    public List<CreatureData> rareData;
    public List<CreatureData> epicData;

    [Header("Inventory Data")]
    [SerializeField] int inventorySize;
    [SerializeField] int initialSlotsUnlocked;
    public List<int> inventorySlotCost;

    [Header("Recovery Pods Data")]
    public List<int> podsCost;
    public List<CurrencyAmount> podLevel_2_Cost;
    public List<CurrencyAmount> podLevel_3_Cost;
    public List<CreatureData> GetCreatureList(int index)
    {
        switch(index)
        {
            case 0:
                return commonData;
            case 1:
                return rareData;
            case 2:
                return epicData;
            
        }

        Debug.LogError("invalid index request");
        return null;
    }
}

[System.Serializable]
public class UnlockCosts
{
    public List<CurrencyAmount> page_1;
    public List<CurrencyAmount> page_2;
    public List<CurrencyAmount> page_3;

    public List<CurrencyAmount> GetPageUnlockCost(int index)
    {
        switch(index)
        {
            case 0:
                return page_1;
            case 1:
                return page_2;
            case 2:
                return page_3;
            default: 
                return null;
        }
    }
}
