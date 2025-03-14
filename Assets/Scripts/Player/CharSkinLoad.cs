using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSkinLoad : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.material.SetColor("_replaceColor", GameManger.Instance.GetCharSkinColor());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
