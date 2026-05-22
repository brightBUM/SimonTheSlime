using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image creatureImage;   // Icon in the slot
    [SerializeField] protected Image imageBG;
    [SerializeField] protected Image selectableImage;
    
    [SerializeField] protected GameObject ownedSetup;
    public int creatureType;
    public int slotId;

    public virtual void Init(int slotId, InventoryState state)
    {
        //from save load
        this.slotId = slotId;
        if (state == InventoryState.vacant)
        {
            //vacant 
            ownedSetup.SetActive(true);
            imageBG.enabled = false;
            ClearSlot();
        }
        else
        {
            ownedSetup.SetActive(true);
            AssignToSlot(state);
        }

    }
    public void ClearSlot()
    {

        creatureImage.sprite = null;
        creatureImage.enabled = false;
        selectableImage.raycastTarget = false; //prevent from drag and drop
        imageBG.enabled = false;
    }
    public void AssignToSlot(InventoryState inventoryState)
    {
        creatureType = ((int)inventoryState);
        creatureImage.enabled = true;
        creatureImage.sprite = GameManger.Instance.GetCreatureSprite((CreatureType)creatureType);
        selectableImage.raycastTarget = true;
        imageBG.enabled = true;
        imageBG.color = GameManger.Instance.GetCreatureColor((CreatureType)creatureType);
    }
}
