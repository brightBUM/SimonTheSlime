using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image creatureImage;   // Icon in the slot
    [SerializeField] private GameObject pickedUpPrefab; // Prefab for dragged object

    private GameObject draggedIcon;
    private RectTransform draggedRect;
    private Canvas parentCanvas;

    void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (creatureImage.sprite == null) return;

        // Hide original
        creatureImage.enabled = false;

        // Create floating icon
        draggedIcon = Instantiate(pickedUpPrefab, parentCanvas.transform);
        draggedIcon.GetComponent<Image>().sprite = creatureImage.sprite;

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

        //else
        // Restore slot image if drop failed
        creatureImage.enabled = true;
    }
}
