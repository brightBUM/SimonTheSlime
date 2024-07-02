using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCollision : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] CameraController cameraController;
    [Header("pound Effect")]
    [SerializeField] GameObject poundEffect;
    [SerializeField] Sprite[] poundSprites;
    [SerializeField] float maskRange = 3f;
    [SerializeField] float fadeDelay = 0.5f;
    [SerializeField] float fadeDuration = 1f;
    const int ObstacleLayer = 6;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerController.playerState == State.LAUNCHED)
        {
            //checking for firstbounce
           playerController.SetToFirstBounce();
        }
        else if(playerController.playerState == State.FIRSTBOUNCE)
        {
            //checking after firstbounce , bring it to rest
           playerController.SetToIdle();

        }
        else if(playerController.playerState == State.POUND)
        {
            cameraController.CameraPoundEffect();
            playerController.ResetPound();
            //splatter effect
            SplatterEffect();
        }

        //Debug.Log("object layer :  " + collision.gameObject.layer+" , Mask layer : "+(int)obstacleLayerMask);

        if(collision.collider.gameObject.layer == ObstacleLayer && playerController.playerState!=State.GHOST)
        {
            //hit with obstacle , respawn to last checkpoint
            cameraController.CameraHitEffect();
            playerController.PlayerHitEffect();
        }
    }

    private void SplatterEffect()
    {
        var rotRange = Random.Range(165f, 185f);
        var poundObject = Instantiate(poundEffect, transform.position + new Vector3(0,-1,-1) * maskRange, Quaternion.Euler(0f, 0f, rotRange));
        var poundSprite = poundObject.GetComponent<SpriteRenderer>();
        poundSprite.sprite = poundSprites[Random.Range(0, poundSprites.Length)];

        StartCoroutine(DelayedFade(poundSprite));
    }

    IEnumerator DelayedFade(SpriteRenderer sprite)
    {
        yield return new WaitForSeconds(fadeDelay);

        float alpha = 1.0f;
        DOTween.To(() => alpha, x => alpha = x, 0, fadeDuration).OnUpdate(() =>
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);

        }).OnComplete(() =>
        {
            Destroy(sprite.gameObject);
        });
    }
}
