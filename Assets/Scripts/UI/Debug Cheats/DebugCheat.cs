using UnityEngine;
using IngameDebugConsole;
public class DebugCheat : MonoBehaviour
{
    [ConsoleMethod("ToggleAdStatus","toggles Ad load status panel")]
    public static void ToggleAdStatusPanel()
    {
        IronSourceAdManager.Instance.ToggleAdPanel();
    }
    [ConsoleMethod("AddCurrency ","adds currency to profile")]
    public static void AddCurrency(CurrencyType currencyType,int amount)
    {
        var saveloadManagerInstance = SaveLoadManager.Instance;
        saveloadManagerInstance.testCurrentAmmount.currencyType = currencyType;
        saveloadManagerInstance.testCurrentAmmount.amount = amount;
        saveloadManagerInstance.AddTestCurrencyAmount();
    }
    [ConsoleMethod("AddToInventory ", "adds creature to inventory")]
    public static void AddCreatureToInventory(CreatureType creatureType,int amount)
    {
        var saveloadManagerInstance = SaveLoadManager.Instance;

        

        for (int i = 0; i < amount; i++)
        {
            if (!saveloadManagerInstance.IsInventorySlotAvailable())
            {
                Debug.Log("no vacant inventory slot");
                return;
            }
            else
            {
                saveloadManagerInstance.AddCreatureToInventory((int)creatureType);

            }
        }
        saveloadManagerInstance.RearrangeInventory();
    }
    [ConsoleMethod("AddToChain","adds creature to follow chain")]
    public static void AddCreatureToChain(CreatureType creatureType,int amount)
    {
        var saveloadManagerInstance = SaveLoadManager.Instance;

        var creatureChain = FindAnyObjectByType<CreatureChain>();

        if(creatureChain == null)
        {
            Debug.Log("no creature chain object exists");
            return;
        }

        var sprite = GameManger.Instance.GetCreatureSprite(creatureType);
        for(int i=0; i<amount; i++)
        {
            creatureChain.AddToChain(creatureType, sprite);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
