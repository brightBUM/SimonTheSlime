using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ChainToInventory : MonoBehaviour
{
    [SerializeField] Image collectionImgPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
   
    public void CollectChain(CreatureChain creatureChain, Vector3 targetPos,Action InventoryFullAction)
    {
        creatureChain.SpriteSortChain(-1);
        // Work on a copy so we can safely remove from the original list
        var segmentsCopy = new List<(Transform transform, CreatureType type)>();
        for (int i = 0; i < creatureChain.segments.Count; i++)
        {
            var transform = creatureChain.segments[i];
            var type = creatureChain.creatureTypes[i];
            segmentsCopy.Add((transform, type));
        }
        // Clear the chain list immediately so it stops updating positions
        creatureChain.segments.Clear();

        float duration = 0.25f;
        int index = 0;
        foreach (var segment in segmentsCopy)
        {
            //Debug.Break();
            segment.transform.DOMove(targetPos, duration).OnComplete(() =>
            {
                SoundManager.Instance.PlayCreatureCollectToLevelSFx(index);
                index++;
                segment.transform.gameObject.SetActive(false);
                if (SaveLoadManager.Instance.IsInventorySlotAvailable())
                {
                    var inventoryHandler = GamePlayScreenUI.Instance.inventoryPanel;
                    //Debug.Break();
                    //spawn ui collection Image
                    Image flyImg = Instantiate(collectionImgPrefab, inventoryHandler.transform);
                    flyImg.sprite = GameManger.Instance.GetCreatureSprite(segment.type);
                    flyImg.raycastTarget = false;
                    RectTransform flyRect = flyImg.GetComponent<RectTransform>();
                    var spawnPos = Camera.main.WorldToScreenPoint(segment.transform.position);
                    flyRect.transform.position = spawnPos;
                    flyRect.localScale = Vector3.one*0.5f;

                    var slotIndex = SaveLoadManager.Instance.GetVacantInventorySlotIndex();
                    var targetRect = (RectTransform)inventoryHandler.inventorySlots[slotIndex].transform;
                    inventoryHandler.ScrollToSlot(targetRect);

                    Vector2 localPoint;

                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        (RectTransform)inventoryHandler.transform,
                        RectTransformUtility.WorldToScreenPoint(null, targetRect.position),
                        null,
                        out localPoint
                    );

                    inventoryHandler.AddCreature(segment.type);
                    flyRect.DOAnchorPos(localPoint, 0.25f).OnComplete(() =>
                    {
                        SoundManager.Instance.PlayCreatureToInventorySFx(index);
                        //Debug.Break();
                        inventoryHandler.inventorySlots[slotIndex].Init(slotIndex, (InventoryState)segment.type);
                        flyRect.gameObject.SetActive(false);
                    }).SetLink(this.gameObject);
                }
                else
                {
                    //Debug.Log("Inventory Full");
                    InventoryFullAction?.Invoke();
                }
            });
            duration += 0.25f;
            
        }
        
    }
     
}
