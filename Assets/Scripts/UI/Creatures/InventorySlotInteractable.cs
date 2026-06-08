
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotInteractable : InventorySlot, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] protected GameObject pickedUpPrefab; // Prefab for dragged object
    [SerializeField] GameObject buySetup;
    [SerializeField] TextMeshProUGUI buyText;
    private GameObject draggedIcon;
    private RectTransform draggedRect;
    private Canvas parentCanvas;
    private Sprite storedSprite;
    private bool droppedOnValidSlot;
    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }
    public override void Init(int slotId,InventoryState state)
    {
        //from save load
        this.slotId = slotId;
        if(state == InventoryState.vacant)
        {
            //vacant 
            ownedSetup.SetActive(true);
            imageBG.enabled = false;
            ClearSlot();
        }
        else if(state == InventoryState.buy)
        {
            buySetup.SetActive(true);
            
            buyText.text = GetSlotCost().ToString();
        }
        else
        {
            ownedSetup.SetActive(true);
            AssignToSlot(state);
        }

    }
    private int GetSlotCost()
    {
        return GameManger.Instance.gameConfig.inventorySlotCost[slotId];
    }
    public void BuySlot()
    {
        //to-do
        //decrement currency
        var cost = GetSlotCost();
        var saveLoadInstance = SaveLoadManager.Instance;
        List<CurrencyAmount> currencyList = new List<CurrencyAmount>
        {
            new CurrencyAmount
            {
                currencyType = CurrencyType.Nanas, amount = cost
            }
        };
        if (saveLoadInstance.CanPurchase(currencyList))
        {
            buySetup.SetActive(false);
            ownedSetup.SetActive(true);
            ClearSlot();
            saveLoadInstance.BuyInventorySlot(slotId);
            saveLoadInstance.SaveGame();
        }
        else
        {
            CurrencyManager.TriggerNoCurrencyFeedBack(CurrencyType.Nanas);
        }
        
    }
    
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (creatureImage.sprite == null) return;

        storedSprite = creatureImage.sprite;
        creatureImage.enabled = false;
        droppedOnValidSlot = false;

        // Create floating ghost icon
        draggedIcon = Instantiate(pickedUpPrefab, parentCanvas.transform);
        draggedIcon.GetComponent<Image>().sprite = storedSprite;
        draggedIcon.GetComponent<Image>().raycastTarget = false;
        draggedRect = draggedIcon.GetComponent<RectTransform>();
        draggedRect.position = eventData.position;

        SoundManager.Instance.PlayCreaturePickupSFx();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedRect != null)
        {
            // Follow pointer
            draggedRect.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // if item dropped in acceptable target
        // Destroy floating icon
        if (draggedIcon != null)
            Destroy(draggedIcon);

        // If drop failed, restore slot
        if (!droppedOnValidSlot)
        {
            creatureImage.enabled = true;
            creatureImage.sprite = storedSprite;
            SoundManager.Instance.PlayCreaturePickCancelSFx();
        }
        else
        {
            // Clear inventory slot when successfully placed in equip slot
            ClearSlot();
        }
    }
    
    public void MarkAsDropped()
    {
        droppedOnValidSlot = true;
        //remove creature from saveload 
        SaveLoadManager.Instance.RemoveCreatureFromInventory(slotId);
        SoundManager.Instance.PlayCreatureDropSFx();

        if (draggedIcon != null)
            Destroy(draggedIcon);
    }
}
