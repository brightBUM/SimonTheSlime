using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CharSkinBase : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TextMeshProUGUI skinNameText;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] GameObject unlockedObject;
    [SerializeField] GameObject selectedObject;
    public int skinNum;
    public bool isPod;


    // Start is called before the first frame update
    private void OnEnable()
    {
        //check if it is unlocked/equipped from saveload
        switch (SaveLoadManager.Instance.CheckIfSkinSelectedOrUnlocked(isPod, skinNum))
        {
            case 0:
                selectedObject.SetActive(true);
                ShopManager.instance.AddToList(this);
                break;
            case 1:
                unlockedObject.SetActive(true);
                ShopManager.instance.AddToList(this);
                break;
            case 2:
                purchaseButton.SetActive(true);
                break;

        }
    }

    public void EquipSkin()
    {
        ShopManager.instance.SetEquippedSkin(this);
        SaveLoadManager.Instance.EquipSkin(this);
    }
    public void FlipSelection(bool state)
    {
        selectedObject.SetActive(state);
        unlockedObject.SetActive(!state);
    }
    public void PurchaseSkin()
    {
        if(SaveLoadManager.Instance.PurchaseSkin(this))
        {
            ShopManager.instance.AddToList(this);
            purchaseButton.SetActive(false);
            unlockedObject.SetActive(true);

            LogPurchaseEvents();

        }
        else
        {
            // not enough melons
            // tween a button shake
        }
    }

    private void LogPurchaseEvents()
    {
        if(isPod)
        {
            //Firebase.Analytics.FirebaseAnalytics.LogEvent("GAME", "No. of players purchasing Pod Skin " + (skinNum+1).ToString());

        }
        else
        {
            //Firebase.Analytics.FirebaseAnalytics.LogEvent("GAME", "No. of players purchasing Character Skin "+(skinNum+1).ToString());

        }
    }
}
