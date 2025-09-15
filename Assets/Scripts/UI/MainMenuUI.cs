using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;
    [SerializeField] Transform contentParent;
    [SerializeField] GameObject registrationPanel;
    [SerializeField] GameObject NoAdsButton;
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField ageField;
    [SerializeField] WatchAdRewardUI watchAdRewardUI;
    private void OnEnable()
    {
        //check if game loaded for first time
        if (SaveLoadManager.Instance.firstLoad)
        {
            //show profile registration page
            registrationPanel.SetActive(true);
            //show banner ad only the first time
            IronSourceAdManager.Instance.LoadBannerAd();
        }

        //update no ads button UI
        if(IronSourceAdManager.Instance.NoAdsPurchased)
        {
            DisableNoAdsButton();
        }
    }
    public void DisableNoAdsButton()
    {
        NoAdsButton.SetActive(false);
    }

    public void RemoveAdsPurchase()
    {
        PurchaseManager.Instance.NoAdsPurchaseButton();
    }
    public void ActivatePanel(int index)
    {
        foreach (Transform child in contentParent)
        {
            child.gameObject.SetActive(false);
        }

        panels[index].SetActive(true);

        if (index == 2)
        {
            FirebaseAnalyticsManager.Instance.LogEvent("No. of clickes on Store", new Dictionary<string, object>
    {
        { "screen", "MAIN MENU" }
    });
        }


    }
    
    public void SaveProfileInfo()
    {
        if (Int32.TryParse(ageField.text, out int age))
        {
            SaveLoadManager.Instance.SaveProfileInfo(nameField.text, age);
            registrationPanel.SetActive(false);
            //hide banner ad when login panel closes
            IronSourceAdManager.Instance.HideBannerAd();

        }
        else
        {
            Debug.Log("int parse failed");
        }
    }
    public void PrivacyPolicyLink()
    {
        GameManger.Instance.PrivacyPolicy();
    }
    public void TermsLink()
    {
        GameManger.Instance.TermsAndConditions();
    }

}
