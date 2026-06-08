using DG.Tweening;
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
    [SerializeField] ScrollRect scrollRect;
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

    public void ScrollToSlot(RectTransform targetSlot)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;

        // Slot center relative to content
        float slotCenterY = Mathf.Abs(targetSlot.anchoredPosition.y)
                          + targetSlot.rect.height * 0.5f;

        // Desired content offset so slot appears at viewport center
        float targetY = slotCenterY - viewportHeight * 0.5f;

        // Clamp to valid scroll range
        float maxY = contentHeight - viewportHeight;
        targetY = Mathf.Clamp(targetY, 0, maxY);

        // Convert to ScrollRect normalized position
        float normalized =
            maxY <= 0 ? 1f : 1f - (targetY / maxY);

        var currentValue = scrollRect.verticalNormalizedPosition;
        DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            scrollRect.verticalNormalizedPosition = currentValue;
        }
        , normalized, 0.25f).SetEase(Ease.OutQuad);
    }
    private void OnDisable()
    {
        SaveLoadManager.Instance.InventoryArranged -= ReArrangeInventory;

    }
}
