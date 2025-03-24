//using DG.Tweening;
//using System;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Purchasing;
//public class PurchaseManager : MonoBehaviour, IStoreListener
//{
//    IStoreController myStoreController;
//    public ConsumableItem[] consumableItems = new ConsumableItem[4];
//    public Text gemText;
//    public TextMeshProUGUI adGemValueText;
//    int currentValue;
//    private void OnEnable()
//    {
//        EventManager.Instance.AddListener(GameEvent.OnCoinChange, OnCoinChange);
//        OnCoinChange(GameEvent.OnCoinChange, this, null);
//    }
//    void Start()
//    {
//        adGemValueText.text = RemoteConfig.instance.configData.adGemValue + " gems";

//        AssignRemoteConfigValues();

//        SetupBuilder();
//    }

//    private void AssignRemoteConfigValues()
//    {
//        var remotePurchases = RemoteConfig.instance.configData.remotePurchases;
//        for (int i = 0; i < 4; i++)
//        {
//            consumableItems[i].price = remotePurchases[i].price;
//            consumableItems[i].mainGem = remotePurchases[i].mainGem;
//            consumableItems[i].extraGem = remotePurchases[i].extraGem;
//        }
//        UpdateUI();
//    }
//    private void UpdateUI()
//    {
//        foreach (var item in consumableItems)
//        {
//            item.mainGemText.text = item.mainGem.ToString() + " gems";
//            item.extraGemText.text = "+Extra " + item.extraGem.ToString();
//            item.priceText.text = "$" + item.price.ToString();
//        }
//    }
//    private void SetupBuilder()
//    {
//        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
//        foreach (var item in consumableItems)
//        {
//            builder.AddProduct(item.id, ProductType.Consumable);
//        }
//        UnityPurchasing.Initialize(this, builder);
//    }
//    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//    {
//        Debug.Log("Unity Purchase initialization success");
//        this.myStoreController = controller;
//    }
//    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
//    {
//        var product = purchaseEvent.purchasedProduct;
//        Debug.Log("Processing Purchase -" + product.definition.id);

//        foreach (var item in consumableItems)
//        {
//            if (product.definition.id == item.id)
//            {
//                currentValue = Profile.Instance.Coins;
//                Profile.Instance.Coins += (item.mainGem + item.extraGem);
//                OnCoinChange(GameEvent.OnCoinChange, this, Profile.Instance.Coins);
//            }
//        }

//        return PurchaseProcessingResult.Complete;
//    }
//    public void GemBuyButtonPress(int index)
//    {
//        myStoreController.InitiatePurchase(consumableItems[index].id);
//    }

//    public void OnInitializeFailed(InitializationFailureReason error)
//    {
//        Debug.Log("Initialize failed ");
//    }

//    public void OnInitializeFailed(InitializationFailureReason error, string message)
//    {
//        Debug.Log("Initialize failed ");
//    }

//    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//    {
//        Debug.Log("Purchase failed ");

//    }

//    public void BackToHomeScreen()
//    {
//        AppLovinMaxAds.Instance.ShowInterstitial();
//        AppLovinMaxAds.Instance.DestroyBannerView();
//        IronSourceAdManager.Instance.ShowInterstitialAd();
//        IronSourceAdManager.Instance.ReloadInterstitialAd();
//        IronSourceAdManager.Instance.DestroyBannerAd();

//        SceneMaster.Instance.OpenScene(SceneID.Home);
//        SceneMaster.Instance.CloseScene(SceneID.PurchaseGem);
//    }
//    private void OnCoinChange(GameEvent Event_Type, Component Sender, object Param)
//    {
//        if (Param != null)
//        {
//            var endValue = currentValue + ((int)Param - currentValue);
//            gemText.text = Profile.Instance.Coins.ToString();
//            DOTween.To(() => currentValue, x => currentValue = x, endValue, 2f).OnUpdate(() =>
//            {
//                gemText.text = currentValue.ToString("F0");
//            });
//        }
//        else
//        {
//            gemText.text = Profile.Instance.Coins.ToString();
//        }
//    }

//    public void ShowAd()
//    {

//        IronSourceAdManager.Instance.ShowRewardedAd();
//        IronSourceRewardedVideoEvents.onAdClosedEvent += IronSourceRewardedVideoEvents_onAdClosedEvent;
//    }
//    private void IronSourceRewardedVideoEvents_onAdClosedEvent(IronSourceAdInfo obj)
//    {
//        currentValue = Profile.Instance.Coins;
//        Profile.Instance.Coins += RemoteConfig.instance.configData.adGemValue;
//        OnCoinChange(GameEvent.OnCoinChange, this, Profile.Instance.Coins);

//        print("Shop panel rewarded Ad closed ,rewarded gems ");

//        IronSourceAdManager.Instance.LoadRewardedAd();
//        IronSourceRewardedVideoEvents.onAdClosedEvent -= IronSourceRewardedVideoEvents_onAdClosedEvent;
//    }
//    private void OnDisable()
//    {
//        EventManager.Instance?.RemoveEvent(GameEvent.OnCoinChange, OnCoinChange);
//    }
//}
//[Serializable]
//public class ConsumableItem
//{
//    public string id;
//    public string name;
//    public string description;
//    public float price;
//    public int mainGem;
//    public int extraGem;
//    [Header("UI references")]
//    public TextMeshProUGUI mainGemText;
//    public TextMeshProUGUI extraGemText;
//    public TextMeshProUGUI priceText;
//}
