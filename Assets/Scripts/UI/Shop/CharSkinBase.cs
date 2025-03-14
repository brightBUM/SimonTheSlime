using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharSkinBase : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TextMeshProUGUI skinNameText;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] GameObject unlockedObject;
    [SerializeField] GameObject equippedObject;
    public int skinNum;
    public bool isPod;


    // Start is called before the first frame update
    private void OnEnable()
    {
        //check if it is unlocked/equipped from saveload
        if (isPod)
        {
            switch(SaveLoadManager.Instance.CheckIfSkinSelectedOrUnlocked(isPod,skinNum))
            {
                case 0:
                    equippedObject.SetActive(true);
                    ShopManager.instance.AddToList(this);
                    break;
                case 1:
                    unlockedObject.SetActive(true);
                    ShopManager.instance.AddToList(this);
                    break;
                case 2:
                    purchaseButton.SetActive(true);
                    break;

            }
        }

        //if not it is purchasable
    }

    public void EquipSkin()
    {
        ShopManager.instance.SetEquippedSkin(this);
    }
    public void FlipSelection(bool state)
    {
        equippedObject.SetActive(state);
        unlockedObject.SetActive(!state);
    }
    
}
