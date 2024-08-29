using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlatform : MonoBehaviour,IPoundable 
{
    [SerializeField] LevelEndGate levelEndGate;
    [SerializeField] int buttonValue = 0;
    [SerializeField] Sprite activeSprite;
    [SerializeField] Sprite inactiveSprite;

    SpriteRenderer spriteRenderer;

    public Action<int> ButtonPress;
    


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPlayerPounded(Action<IPoundable> ContinuePound)
    {
        ContinuePound(this);

        spriteRenderer.sprite = activeSprite;
        SoundManager.instance.PlaySwitchPlatformSFX();

        this.transform.DOScaleX(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo);
        ButtonPress.Invoke(buttonValue);
    }

    public void Reset()
    {
        spriteRenderer.sprite = inactiveSprite;
    }
}
