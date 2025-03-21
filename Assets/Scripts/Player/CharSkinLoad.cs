using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSkinLoad : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    private void OnEnable()
    {
        RefreshSkin();
        
    }
    public void RefreshSkin()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_replaceColor", GameManger.Instance.GetCharSkinColor());
    }
}
