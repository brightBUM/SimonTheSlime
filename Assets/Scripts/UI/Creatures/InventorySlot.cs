using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image creatureImage;   // Icon in the slot
    [SerializeField] private GameObject pickedUpPrefab; // Prefab for dragged object
    [SerializeField] GameObject buySetup;
    [SerializeField] GameObject ownedSetup;
    public int creatureType;
    public int slotId;
    private GameObject draggedIcon;
    private RectTransform draggedRect;
    private Canvas parentCanvas;
    private Sprite storedSprite;
    private bool droppedOnValidSlot;
    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }
    public void Init(int slotId,InventoryState state)
    {
        //from save load
        this.slotId = slotId;
        if(state == InventoryState.vacant)
        {
            //vacant 
            ownedSetup.SetActive(true);
            ClearSlot();
        }
        else if(state == InventoryState.buy)
        {
            buySetup.SetActive(true);
        }
        else
        {
            ownedSetup.SetActive(true);
            AssignToSlot(state);
        }

    }
    public void BuySlot()
    {
        //to-do 
        //decrement currency
        buySetup.SetActive(false);
        ownedSetup.SetActive(true);
        ClearSlot();
        SaveLoadManager.Instance.BuyInventorySlot(slotId);
        SaveLoadManager.Instance.SaveGame();
    }
    public void AssignToSlot(InventoryState inventoryState)
    {
        creatureType = ((int)inventoryState);
        creatureImage.sprite = GameManger.Instance.GetCreatureSprite((CreatureType)creatureType);
        GetComponent<Image>().raycastTarget = true;
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
        }
        else
        {
            // Clear inventory slot when successfully placed in equip slot
            ClearSlot();
        }
    }
    public void ClearSlot()
    {
        Debug.Log("clear slot " );

        creatureImage.sprite = null;
        creatureImage.enabled = false;
        GetComponent<Image>().raycastTarget = false; //prevent from drag and drop

    }
    public void MarkAsDropped()
    {
        droppedOnValidSlot = true;
        //remove creature from saveload 
        SaveLoadManager.Instance.RemoveCreatureFromInventory(slotId);

        if (draggedIcon != null)
            Destroy(draggedIcon);
    }
}
