using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AssetTheme
{
    ExclusionZone,
    GreenZone,
    PowerStation
}
public class TileAssetHandler : MonoBehaviour
{
    [SerializeField] GameObject[] themedBackgrounds;

    public RuleTile originalRuleTile;
    public RuleTile powerStationRuleTile;
    public RuleTile GreenZoneRuleTile;

    public AssetTheme assetTheme;
    public static TileAssetHandler Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public RuleTile GetThemedRuleTile()
    {
        switch(assetTheme)
        {
            case AssetTheme.ExclusionZone:
                return originalRuleTile;
            case AssetTheme.GreenZone:
                return GreenZoneRuleTile;
            case AssetTheme.PowerStation:
                return powerStationRuleTile;
            default: return null;
        }
    }

    public void SetBackground()
    {
        foreach(var background in themedBackgrounds)
        {
            background.SetActive(false);
        }

        switch (assetTheme)
        {
            case AssetTheme.ExclusionZone:
                themedBackgrounds[0].SetActive(true);
                break;
            case AssetTheme.GreenZone:
                themedBackgrounds[1].SetActive(true);
                break;
            case AssetTheme.PowerStation:
                themedBackgrounds[2].SetActive(true);
                break;
            default: 
                break;
        }
    }
}
