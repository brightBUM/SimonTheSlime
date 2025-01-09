using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TimeOrb : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    Color iconColor;
    Tween tween;
    // Start is called before the first frame update
    void Start()
    {
        iconColor = spriteRenderer.color;
        tween = DOTween.To(() => iconColor.a, x => iconColor.a = x, 0, 0.5f).SetLoops(-1).SetEase(Ease.OutSine).OnUpdate(() =>
        {
            if(spriteRenderer!=null)
            {
                spriteRenderer.color = iconColor;
            }
            else
            {
                tween.Kill();
            }
            
        });

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            //refill bullet time consumable
            playerController.RefillBulletTime();
            SoundManager.instance.PlayTimeOrbCollectSFX();
            //play audio and destroy
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if(tween!=null && tween.IsActive())
        {
            tween.Kill();
        }
    }
}
