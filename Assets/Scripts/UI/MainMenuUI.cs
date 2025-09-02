using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] List<GameObject> panels;
    [SerializeField] Transform contentParent;
    [SerializeField] GameObject registrationPanel;
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField ageField;
    [SerializeField] WatchAdRewardUI watchAdRewardUI;
    [Header("Chest System")]
    [SerializeField] GameObject chestSystemPanel;
    [SerializeField] PageSnapScroll pageSnapScroll;
    [SerializeField] Transform inventoryPanel;
    [SerializeField] Transform showPos;
    [SerializeField] Transform hidePos;
    [Header("Char Upgrades")]
    [SerializeField] GameObject charUpgradePanel;


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

        //initialise page snap scroll
        pageSnapScroll.Init();
        pageSnapScroll.OnPageMoved += PageMoved;
    }

    private void PageMoved(int num)
    {
        if(num>=3)
        {
            //hide inventory
            inventoryPanel.DOMove(hidePos.position, 0.5f).SetEase(Ease.OutBack);
        }
        else if(num<3)
        {
            inventoryPanel.DOMove(showPos.position, 0.5f).SetEase(Ease.OutBack);

        }
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

    private void OnDisable()
    {
        pageSnapScroll.OnPageMoved -= PageMoved;

    }
}
