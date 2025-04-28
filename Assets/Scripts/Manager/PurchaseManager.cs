using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System.Collections.Generic;
public class PurchaseManager : MonoBehaviour, IDetailedStoreListener
{
    IStoreController myStoreController;
    public List<ConsumableItem> bananaItems;
    public List<ConsumableItem> melonItems;
    private void OnEnable()
    {
        
    }
    void Start()
    {
        //adGemValueText.text = RemoteConfig.instance.configData.adGemValue + " gems";

        SetupBuilder();
        AssignRemoteConfigValues();
    }

    private void AssignRemoteConfigValues()
    {
        //var remotePurchases = RemoteConfig.instance.configData.remotePurchases;
        //for (int i = 0; i < 4; i++)
        //{
        //    consumableItems[i].price = remotePurchases[i].price;
        //    consumableItems[i].mainGem = remotePurchases[i].mainGem;
        //    consumableItems[i].extraGem = remotePurchases[i].extraGem;
        //}
        UpdateUI();
    }
    private void UpdateUI()
    {
        foreach (var item in bananaItems)
        {
            var product = myStoreController.products.WithID(item.id);
            var localizedPrice = product.metadata.localizedPriceString;
            Debug.Log("localized prize : "+localizedPrice);

            item.valueText.text = item.value.ToString() + " Nanas";
            item.priceText.text = localizedPrice;
        }

        foreach (var item in melonItems)
        {
            var product = myStoreController.products.WithID(item.id);
            var localizedPrice = product.metadata.localizedPriceString;
            Debug.Log("localized prize : " + localizedPrice);

            item.valueText.text = item.value.ToString() + " Gems";
            item.priceText.text = localizedPrice;
        }
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
        UnityPurchasing.Initialize(this, builder);
        
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Unity Purchase initialization success");
        this.myStoreController = controller;
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        Debug.Log("Processing Purchase -" + product.definition.id);

        foreach (var item in bananaItems)
        {
            if (product.definition.id == item.id)
            {
                Debug.Log("nanas before : " + SaveLoadManager.Instance.playerProfile.nanas);
                SaveLoadManager.Instance.playerProfile.nanas += item.value;
                Debug.Log("nanas after : " + SaveLoadManager.Instance.playerProfile.nanas);
            }
        }

        foreach(var item in melonItems)
        {
            if (product.definition.id == item.id)
            {
                Debug.Log("melons before : " + SaveLoadManager.Instance.playerProfile.melons);
                SaveLoadManager.Instance.playerProfile.melons += item.value;
                Debug.Log("melons after : " + SaveLoadManager.Instance.playerProfile.melons);
            }
        }

        ShopManager.instance.UpdateCurrencyUI();

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
    [Header("UI references")]
    public TextMeshProUGUI valueText;
    public Text priceText;
}
