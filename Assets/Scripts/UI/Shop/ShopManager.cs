using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] Transform[] buttonsTabs;
    [SerializeField] Transform[] ScreenTabs;
    [Header("Currency UI")]
    [SerializeField] TextMeshProUGUI nanasText;
    [SerializeField] TextMeshProUGUI melonsText;
    [Header("Packs UI")]
    [SerializeField] List<Text> melonPackPriceText;
    [SerializeField] List<TextMeshProUGUI> melonPackValueText;
    [SerializeField] List<Text> bananaPackPriceText;
    [SerializeField] List<TextMeshProUGUI> bananaPackValueText;

    [Header("Skins UI")]
    public List<CharSkinBase> charSkinList;
    public List<CharSkinBase> podList;
    [SerializeField] CharSkinLoad charSkinLoad;
    public static ShopManager instance;
    public bool Init = false;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        charSkinList = new List<CharSkinBase>();
        podList = new List<CharSkinBase>();
        Init = true;
    }

    private void OnEnable()
    {
        UpdateCurrencyUI();
        UpdatePacksUIFromRemote();

    }
    public void UpdateCurrencyUI()
    {
        nanasText.text = SaveLoadManager.Instance.playerProfile.nanas.ToString();
        melonsText.text = SaveLoadManager.Instance.playerProfile.melons.ToString();

    }
    public void UpdateCurrencyUI(int currencyNum,int from , int to)
    {
        if(currencyNum==0) // soft currency
        {
            Utility.AnimateCounter(nanasText, from, to, 1f);

        }
        else if(currencyNum==1) // hard currency
        {
            Utility.AnimateCounter(melonsText, from, to, 1f);
        }

    }
    private void UpdatePacksUIFromRemote()
    {
        var purchaseManager = PurchaseManager.Instance;
        if (purchaseManager == null /*|| !RemoteConfig.Instance.IsFetchSucess*/)
            return;

        // var bananaPackValues = RemoteConfig.Instance.configData.skinsandPacks.bananaPackValue;
        // var melonPackValues = RemoteConfig.Instance.configData.skinsandPacks.gemsPackValue;

        for (int i = 0; i < purchaseManager.bananaItems.Count; i++)
        {
            var product = purchaseManager.myStoreController.products.WithID(purchaseManager.bananaItems[i].id);
            var localizedPrice = product.metadata.localizedPriceString;
            //Debug.Log("localized prize : "+localizedPrice);
            //value from remote date

            // bananaPackValueText[i].text = bananaPackValues[i].ToString() + " Nanas";
            bananaPackPriceText[i].text = localizedPrice;
        }

        for (int i = 0; i < purchaseManager.melonItems.Count; i++)
        {
            var product = purchaseManager.myStoreController.products.WithID(purchaseManager.melonItems[i].id);
            var localizedPrice = product.metadata.localizedPriceString;
            //Debug.Log("localized prize : "+localizedPrice);

            //melonPackValueText[i].text = melonPackValues[i].ToString() + " Melons";
            melonPackPriceText[i].text = localizedPrice;
        }
    }

    public void NoMelonsFeedBack()
    {
        if(!DOTween.IsTweening(melonsText.rectTransform))
        {
            melonsText.rectTransform.DOShakeAnchorPos(0.3f, 20);
            SoundManager.Instance.PlayOutofBulletTimeSFX();
        }
    }
    public void AddToList(CharSkinBase charSkinBase)
    {
        if (charSkinBase.isPod)
        {
            podList.Add(charSkinBase);
        }
        else
        {
            charSkinList.Add(charSkinBase);
        }
    }
    
    public void SaveOnShopExit()
    {
        charSkinList.Clear();
        podList.Clear();
        SaveLoadManager.Instance.SaveGame();
        charSkinLoad.RefreshSkin();
    }
    public void SetEquippedSkin(CharSkinBase charSkinBase)
    {
        if (charSkinBase.isPod)
        {
            //loop all the pods
            foreach (CharSkinBase podSkin in podList)
            {
                if (charSkinBase.skinNum == podSkin.skinNum)
                {
                    podSkin.FlipSelection(true);
                }
                else
                {
                    podSkin.FlipSelection(false);
                }
            }

        }
        else
        {
            foreach (CharSkinBase charSkin in charSkinList)
            {
                if (charSkinBase.skinNum == charSkin.skinNum)
                {
                    charSkin.FlipSelection(true);
                }
                else
                {
                    charSkin.FlipSelection(false);
                }
            }
        }
        Debug.Log(string.Format("pod list count : {0} ,charSkin List Count : {1}", charSkinList.Count, podList.Count));
    }
    public void SelectPage(int index)
    {
        for (int i = 0; i < buttonsTabs.Length; i++)
        {
            if(i==index)
            {
                buttonsTabs[i].DOScale(Vector3.one, 0.1f).SetEase(Ease.OutFlash).SetUpdate(true);
                ScreenTabs[i].gameObject.SetActive(true);
            }
            else
            {
                buttonsTabs[i].localScale = Vector3.zero;
                ScreenTabs[i].gameObject.SetActive(false);
            }
        }
    }
    public void PurchaseNanasPack(int index)
    {
        PurchaseManager.Instance.PurchaseBananasButton(index);
    }
    public void PurchaseMelonPack(int index)
    {
        PurchaseManager.Instance.PurchaseGemsButton(index);
    }
}
