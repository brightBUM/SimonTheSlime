using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CharSkinBase : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI skinNameText;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] GameObject unlockedObject;
    [SerializeField] GameObject selectedObject;
    public int skinNum;
    public bool isPod;

   
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(WaitForShopManagerInit());
    }
    private IEnumerator WaitForShopManagerInit()
    {
        yield return new WaitUntil(()=>ShopManager.instance.Init);

        skinNameText.text = GameManger.Instance.GetSkinByIndex(isPod,skinNum).skinName;
        SetShopButtons();
    }
    private void SetShopButtons()
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
            default:
                Debug.Log("char skin base default condition");
                break;

        }
    }
    public void EquipSkin()
    {
        try
        {
            ShopManager.instance.SetEquippedSkin(this);
            SaveLoadManager.Instance.EquipSkin(this);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
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
