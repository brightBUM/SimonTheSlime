using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TimeOrb : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform visual;
    [SerializeField] float shakeStrength = 10f;
    [SerializeField] float jumpOffset = 1f;
    Color iconColor;
    Tween bgTween;
    Tween visualTween;
    Tween jumpTween;
    // Start is called before the first frame update
    void Start()
    {
        iconColor = spriteRenderer.color;
        bgTween = DOTween.To(() => iconColor.a, x => iconColor.a = x, 0, 0.5f).SetLoops(-1).SetEase(Ease.OutSine).OnUpdate(() =>
        {
            if(spriteRenderer!=null)
            {
                spriteRenderer.color = iconColor;
            }
            else
            {
                bgTween.Kill();
            }
            
        });

        var visualTransform = spriteRenderer.transform;
        jumpTween = visualTransform.DOMoveY(visualTransform.position.y + jumpOffset, 0.5f).SetLoops(-1,LoopType.Yoyo);
        visualTween = visual.DOShakeRotation(0.5f,new Vector3(0,0, shakeStrength),10,45).SetLoops(-1);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            //refill bullet time consumable
            playerController.RefillBulletTime();
            SoundManager.Instance.PlayTimeOrbCollectSFX();
            //play audio and destroy
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if(bgTween!=null && bgTween.IsActive())
        {
            bgTween.Kill();
        }
        if(visualTween!=null && visualTween.IsActive())
        {
            visualTween.Kill();
        }
        if(jumpTween!=null && jumpTween.IsActive())
        {
            jumpTween.Kill();
        }
    }
}
