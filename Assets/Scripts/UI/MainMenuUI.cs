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
    [SerializeField] GameObject NoAdsButton;
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField ageField;
    [SerializeField] WatchAdRewardUI watchAdRewardUI;
    [SerializeField] Transform spinWheelText;
    [SerializeField] Transform spinWheelIcon;
    [Header("Char UI Pod")]
    [SerializeField] RectTransform canvasRect ;
    [SerializeField] RectTransform itemRect ;
    [SerializeField] Image linesObject;
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
         //update no ads button UI
        if(IronSourceAdManager.Instance.NoAdsPurchased)
        {
            DisableNoAdsButton();
        }
        //resize the layout w.r.t to screen size
        scrollPageSizer.Apply();

        //initialise page snap scroll
        //pageSnapScroll.SnapToStartComplete += CharacterUIReparent;
        pageSnapScroll.Init();
        pageSnapScroll.OnPageMoved += PageMoved;
        pageSnapScroll.OnScrollStart += ScrollStart;
        pageSnapScroll.OnScrollEnd += ScrollEnd;
        creaturePanelSwipe.OnOpenPanel += OnCreaturePanelOpen;
        creaturePanelSwipe.OnClosePanel += OnCreaturePanelClosed;

        //tween spin wheel
        spinWheelText.transform.DOScale(1.1f, 0.25f).SetLoops(-1, LoopType.Yoyo).SetLink(this.gameObject);
        spinWheelIcon.DORotate(new Vector3(0, 0, -45), 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutBack)
            .SetLink(this.gameObject);
    }

    public void CharacterUIReparent()
    {
        // store current world position (after layout has settled)
        Vector3 worldPos = itemRect.position;
        Quaternion worldRot = itemRect.rotation;
        Vector3 worldScale = itemRect.lossyScale;

        // reparent (keep world position)
        itemRect.SetParent(canvasRect, true);

        // restore (usually position is enough; rotation if needed)
        itemRect.position = worldPos;
        itemRect.rotation = worldRot;

        // then set draw order
        itemRect.SetSiblingIndex(1);
    }
    private void OnCreaturePanelOpen(Action done)
    {
        HideInventory();
        CurrencyManager.ToggleCurrencyPanel?.Invoke(false);
        creaturePanel.transform.DOMove(crOpenTransform.position, 0.5f).OnComplete(() =>
        {
            done?.Invoke();
            creaturePanel.TogglePanel(0); //auto refresh from common tab
            
        });
    }
    private void OnCreaturePanelClosed(Action done)
    {
        ShowInventory();
        CurrencyManager.ToggleCurrencyPanel?.Invoke(true);
        creaturePanel.transform.DOMove(crCloseTransform.position, 0.5f).OnComplete(() =>
        {
            done?.Invoke();
            
        });
    }
    private void ScrollStart()
    {
        SoundManager.Instance.PlayFloorScrollSFx();

    }
    private void ScrollEnd()
    {
        //SoundManager.Instance.PlayFloorScrollSnapSFx();
    }
    private void PageMoved(int num)
    {

        if (num == 4)
        {
            //char upgrade panel

            CurrencyManager.ToggleCurrencyPanel(true);
            //characterUI.parent = mainParent;
            //characterUI.localPosition = characterUIAnchorPoint_1.localPosition;
            //itemRect.SetSiblingIndex(3);
            //StartCoroutine(ToggleLines(0.3f, true));
        }
        else if (num == 3)
        {
            //main menu panel
            CurrencyManager.ToggleCurrencyPanel(false);


            //characterUI.parent = mainMenuParent;
            //characterUI.localPosition = characterUIAnchorPoint_2.localPosition;
            //itemRect.SetSiblingIndex(1);
            //StartCoroutine(ToggleLines(0f, false));

        }


        if (num>=3)
        {
            //hide inventory
            HideInventory();
            creaturePanel.transform.DOMove(crHideTransform.position, 0.5f).SetEase(Ease.OutBack);
        }
        else if(num<3)
        {
            ShowInventory();
            CurrencyManager.ToggleCurrencyPanel?.Invoke(true);
            creaturePanel.transform.DOMove(crCloseTransform.position, 0.5f).SetEase(Ease.OutBack);
        }


    }
    IEnumerator ToggleLines(float delay , bool value)
    {
        yield return new WaitForSeconds(delay);

        if (value)
        {
            float timer = 0f;
            float duration = 0.5f;
            while (timer <= duration)
            {
                linesObject.fillAmount = (float)timer / duration;
                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            linesObject.fillAmount = 0f;

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
    public void DisableNoAdsButton()
    {
        NoAdsButton.SetActive(false);
    }

    public void RemoveAdsPurchase()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        PurchaseManager.Instance.NoAdsPurchaseButton();
#endif

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
        //pageSnapScroll.SnapToStartComplete -= CharacterUIReparent;
        pageSnapScroll.OnPageMoved      -= PageMoved;
        pageSnapScroll.OnScrollStart    -= ScrollStart;
        pageSnapScroll.OnScrollEnd      -= ScrollEnd;

        creaturePanelSwipe.OnOpenPanel  -= OnCreaturePanelOpen;
        creaturePanelSwipe.OnClosePanel -= OnCreaturePanelClosed;
    }
}
