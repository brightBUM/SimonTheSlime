using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Upgrade Stat", fileName = "UpgradeStat")]
public class UpgradeStatDefinitionSO : ScriptableObject
{
    public UpgradeStatId statId;

    [Header("Designer Info")]
    [TextArea] public string description;

    [Header("Upgrade Steps (order matters)")]
    public List<UpgradeStep> upgrades = new List<UpgradeStep>();

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
    public CurrencyAmount currencyAmount;

    [Tooltip("Value added to the stat when this upgrade is purchased")]
    public float value;
}
