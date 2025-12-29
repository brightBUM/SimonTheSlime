using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

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
    [SerializeField] ScrollPageSizer scrollPageSizer;
    [SerializeField] PageSnapScroll pageSnapScroll;
    [Header("Inventory")]
    [SerializeField] Transform inventoryPanel;
    [SerializeField] Transform invShowPos;
    [SerializeField] Transform invHidePos;
    [Header("Creature Page")]
    [SerializeField] SwipeDetection creaturePanelSwipe;
    [SerializeField] CreaturesPanel creaturePanel;
    [SerializeField] Transform crOpenTransform;
    [SerializeField] Transform crCloseTransform;
    [SerializeField] Transform crHideTransform;
    
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

        //resize the layout w.r.t to screen size
        scrollPageSizer.Apply();

        //initialise page snap scroll
        pageSnapScroll.Init();
        pageSnapScroll.OnPageMoved += PageMoved;

        creaturePanelSwipe.OnOpenPanel += OnCreaturePanelOpen;
        creaturePanelSwipe.OnClosePanel += OnCreaturePanelClosed;
    }
    private void OnCreaturePanelOpen(Action done)
    {
        HideInventory();
        creaturePanel.transform.DOMove(crOpenTransform.position, 0.5f).OnComplete(() =>
        {
            done?.Invoke();
            creaturePanel.TogglePanel(0); //auto refresh from common tab
            
        });
    }
    private void OnCreaturePanelClosed(Action done)
    {
        ShowInventory();
        creaturePanel.transform.DOMove(crCloseTransform.position, 0.5f).OnComplete(() =>
        {
            done?.Invoke();
            
        });
    }
    private void PageMoved(int num)
    {
        if(num>=3)
        {
            //hide inventory
            HideInventory();
            creaturePanel.transform.DOMove(crHideTransform.position, 0.5f).SetEase(Ease.OutBack);
        }
        else if(num<3)
        {
            ShowInventory();
            creaturePanel.transform.DOMove(crCloseTransform.position, 0.5f).SetEase(Ease.OutBack);
        }
    }

    private void HideInventory()
    {
        inventoryPanel.DOMove(invHidePos.position, 0.5f).SetEase(Ease.OutBack);
    }
    private void ShowInventory()
    {
        inventoryPanel.DOMove(invShowPos.position, 0.5f).SetEase(Ease.OutBack);
    }
    private void CloseCreaturePage()
    {

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
        pageSnapScroll.OnPageMoved      -= PageMoved;
        creaturePanelSwipe.OnOpenPanel  -= OnCreaturePanelOpen;
        creaturePanelSwipe.OnClosePanel -= OnCreaturePanelClosed;
    }
}
