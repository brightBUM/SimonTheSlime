using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade Stat", fileName = "UpgradeStat")]
public class UpgradeStatDefinitionSO : ScriptableObject
{
    public UpgradeStatId statId;

    [Header("Designer Info")]
    [TextArea] public string description;

    [Header("Base Stat Value")]
    public float baseValue;

    [Header("Upgrade Steps (order matters)")]
    public List<UpgradeStep> upgrades = new List<UpgradeStep>();

    public int MaxLevel => upgrades.Count;

    // cost for next upgrade
    public int GetNextCost(int currentLevel)
    {
        if (currentLevel < 0 || currentLevel >= upgrades.Count)
            return -1;

        return upgrades[currentLevel].cost;
    }

    // base + sum of step values
    public float GetValueAtLevel(int level)
    {
        level = Mathf.Clamp(level, 0, MaxLevel);

        float value = baseValue;

        for (int i = 0; i < level; i++)
            value += upgrades[i].value;

        return value;
    }
}

public enum UpgradeStatId
{
    SlamPower,
    HangTime,
    BulletTime,
    TimeOrb
}

[Serializable]
public class UpgradeStep
{
    public int cost;

    [Tooltip("Value added to the stat when this upgrade is purchased")]
    public float value;
}
