using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSkinLoad : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public bool isPod = false;
    Material material;
    bool skinAnimate;
    // Start is called before the first frame update
    private void OnEnable()
    {
        RefreshSkin();
        
    }
    public void RefreshSkin()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var image = GetComponent<Image>();

        if (spriteRenderer != null)
        {
            material = spriteRenderer.material;
        }
        else if (image != null)
        {
            material = image.material;
        }
        else
        {
            material = null;
        }

        if(isPod)
        {
            var skin = GameManger.Instance?.GetPodSkinColor();
            SetSkinToMaterial(skin, material);
        }
        else
        {
            var skin = GameManger.Instance?.GetCharSkinColor();
            SetSkinToMaterial(skin, material);
            skinAnimate = GameManger.Instance.ShouldAnimate();
        }
    }
    private void Update()
    {
        if(skinAnimate)
        {
            float speed = 3f;
            float oscillatingValue = Mathf.Sin(Time.time * speed); // oscillates between -1 and 1
            material.SetFloat("_Hue", oscillatingValue);
        }
    }
    private void SetSkinToMaterial(Skin skin,Material material)
    {
        material.SetFloat("_Hue", skin.hueshift);
        material.SetFloat("_Saturation", skin.saturation);
        material.SetColor("_Tint", skin.tintColor);
        material.SetFloat("_Invert", skin.invert);
    }
}
