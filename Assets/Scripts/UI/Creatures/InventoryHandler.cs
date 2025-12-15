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
    // Start is called before the first frame update
    private void OnEnable()
    {
        SaveLoadManager.Instance.InventoryArranged += ReArrangeInventory;
    }
    void Start()
    {
        PopulateInventory();
    }
    public void PopulateInventory()
    {
        //get inventory data from saveLoad
        var inventoryData = SaveLoadManager.Instance.playerProfile.inventoryData;
        for (int i = 0; i < inventoryData.Count; i++)
        {
            //populate the inventory
            var spawnedSlot = Instantiate(slotPrefab, parent);
            spawnedSlot.Init(i, inventoryData[i]);

        }
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
