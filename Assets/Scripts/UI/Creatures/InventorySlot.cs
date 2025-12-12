using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image creatureImage;   // Icon in the slot
    [SerializeField] private GameObject pickedUpPrefab; // Prefab for dragged object
    public CreatureType creatureType;
    private GameObject draggedIcon;
    private RectTransform draggedRect;
    private Canvas parentCanvas;
    private Sprite storedSprite;
    private bool droppedOnValidSlot;
    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
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
        //destroy slot from scroll area
        //Debug.Log("clear slot");
        Destroy(gameObject);
    }
    public void MarkAsDropped()
    {
        droppedOnValidSlot = true;
        //remove creature from saveload 
    }
}
