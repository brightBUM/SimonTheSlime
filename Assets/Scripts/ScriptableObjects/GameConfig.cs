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

    [Header("Level Stats")]
    [OdinSerialize]
    public Dictionary<int, List<int>> levelTargetTime;

    [Header("Level Parts")]
    public List<int> parts;

    [Header("Creature Data")]
    public List<CreatureData> commonData;
    public List<CreatureData> rareData;
    public List<CreatureData> epicData;

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
