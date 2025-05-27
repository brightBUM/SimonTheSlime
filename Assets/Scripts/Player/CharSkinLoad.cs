using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSkinLoad : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public bool isPod = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        RefreshSkin();
        
    }
    public void RefreshSkin()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(isPod)
        {
            var color = GameManger.Instance.GetPodSkinColor();
            if(color == Color.black)
            {
                spriteRenderer.material = new Material(Shader.Find("Sprites/Default")); ;
            }
            else
            {
                spriteRenderer.material.SetColor("_replaceColor", GameManger.Instance.GetPodSkinColor());
            }

        }
        else
        {
            var color = GameManger.Instance.GetCharSkinColor();
            if (color == Color.black)
            {
                spriteRenderer.material = new Material(Shader.Find("Sprites/Default")); ;
            }
            else
            {
                spriteRenderer.material.SetColor("_replaceColor", GameManger.Instance.GetCharSkinColor());
            }
        }
    }
}
