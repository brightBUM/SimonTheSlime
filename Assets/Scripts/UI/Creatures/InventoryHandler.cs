using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//class that populates inventory slots
public enum InventoryState
{
    common,
    rare,
    epic,
    vacant,
    buy
}
public class InventoryHandler : MonoBehaviour
{
    [SerializeField] InventorySlot slotPrefab;
    [SerializeField] Transform parent;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    bool isSlotInteractable;
    // Start is called before the first frame update
    private void OnEnable()
    {
        SaveLoadManager.Instance.InventoryArranged += ReArrangeInventory;
    }
    void Start()
    {
        isSlotInteractable = slotPrefab is InventorySlotInteractable;
        
        PopulateInventory();
    }
    public void PopulateInventory()
    {
        //get inventory data from saveLoad
        var inventoryData = SaveLoadManager.Instance.playerProfile.inventoryData;
        for (int i = 0; i < inventoryData.Count; i++)
        {
            //populate the inventory
            if(!isSlotInteractable && inventoryData[i]==InventoryState.buy)
            {
                //spawn only the owned slots for non interactable inventory 
                //i.e only assigned and vacant ones
                break;
            }
            var spawnedSlot = Instantiate(slotPrefab, parent);
            spawnedSlot.Init(i, inventoryData[i]);
            inventorySlots.Add(spawnedSlot);
        }
    }
    [ContextMenu("AddCollective")]
    public void AddCollectiveCreatures()
    {
        for(int i = 0;i<4;i++)
        {
            if(SaveLoadManager.Instance.IsInventorySlotAvailable())
            {
                AddCreature((CreatureType)0);
            }
            else
            {
                Debug.Log("inventory full ");
                //to - do
                //implement full text dotween pop up
            }
        }
        SaveLoadManager.Instance.RearrangeInventory();
        SaveLoadManager.Instance.SaveGame();

    }
    public void AddCreature(CreatureType creatureType)
    {
        SaveLoadManager.Instance.AddCreatureToInventory((int)creatureType);
    }
    public void ReArrangeInventory()
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }
        PopulateInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        SaveLoadManager.Instance.InventoryArranged -= ReArrangeInventory;

    }
}
