using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Collections.Generic;
public class PurchaseManager : MonoBehaviour, IDetailedStoreListener
{
    public IStoreController myStoreController;
    public List<ConsumableItem> bananaItems;
    public List<ConsumableItem> melonItems;
    public NonConsumableItem noAdsPurchaseItem;
    
    public static PurchaseManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //adGemValueText.text = RemoteConfig.instance.configData.adGemValue + " gems";

        SetupBuilder();
    }
    
    private void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var item in bananaItems)
        {
            builder.AddProduct(item.id, ProductType.Consumable);
        }
        foreach (var item in melonItems)
        {
            builder.AddProduct(item.id, ProductType.Consumable);
        }

        builder.AddProduct(noAdsPurchaseItem.id, ProductType.NonConsumable);
        UnityPurchasing.Initialize(this, builder);
        
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Unity Purchase initialization success");
        this.myStoreController = controller;
        CheckNonConsumableReceipt();
    }

    private void CheckNonConsumableReceipt()
    {
        if(myStoreController != null)
        {
            var product = myStoreController.products.WithID(noAdsPurchaseItem.id);
            if(product != null)
            {
                if(product.hasReceipt)
                {
                    //remove ads, update in adManager
                    IronSourceAdManager.Instance.NoAdsPurchased = true;
                }
            }
        }
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        Debug.Log("Processing Purchase -" + product.definition.id);

        var playerProfile = SaveLoadManager.Instance.playerProfile;
        foreach (var item in bananaItems)
        {
            if (product.definition.id == item.id)
            {
                SaveLoadManager.Instance.AddCurrency(CurrencyType.Nanas, item.value);
            }
        }

        foreach(var item in melonItems)
        {
            if (product.definition.id == item.id)
            {
                SaveLoadManager.Instance.AddCurrency(CurrencyType.Melons, item.value);
            }
        }

        // Non-consumable: No Ads
        if (product.definition.id == noAdsPurchaseItem.id)
        {
            IronSourceAdManager.Instance.NoAdsPurchased = true;
            FindAnyObjectByType<MainMenuUI>()?.DisableNoAdsButton();
            Debug.Log("No Ads purchase successful - ads removed");
        }

        return PurchaseProcessingResult.Complete;
    }
    public void PurchaseBananasButton(int index)
    {
        myStoreController.InitiatePurchase(bananaItems[index].id);
    }
    public void PurchaseGemsButton(int index)
    {
        myStoreController.InitiatePurchase(melonItems[index].id);
    }
    public void NoAdsPurchaseButton()
    {
        myStoreController.InitiatePurchase(noAdsPurchaseItem.id);
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

        if (message != null)
        {
            errorMessage += $" More details: {message}";
        }

        Debug.Log(errorMessage);
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
            $" Purchase failure reason: {failureDescription.reason}," +
            $" Purchase failure details: {failureDescription.message}");
    }


    private void OnDisable()
    {
        //EventManager.Instance?.RemoveEvent(GameEvent.OnCoinChange, OnCoinChange);
    }

    
}
[Serializable]
public class ConsumableItem
{
    public string id;
    public string name;
    public string description;
    public float price;
    public int value;
    
}
[Serializable]
public class NonConsumableItem
{
    public string id;
    public string name;
    public string description;
    public float price;
}
