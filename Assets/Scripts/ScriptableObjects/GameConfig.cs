using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Default Save File")]
    public string ProfileName = "default";
    public int nanasCount = 500;
    public int melonsCount = 50;
    public int perfectJumpBase = 25;
    [Header("Ads Setting")]
    public int interstitialAdCheckPerLevel;
    public int mainMenuRewardedAdNanas;
    public int RetryNanasCost = 100;
    [Header("Level Stats")]
    public List<float> targetTime;
    [Header("Level Parts")]
    public List<int> parts;


}
