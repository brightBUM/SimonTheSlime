using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCollision : MonoBehaviour
{
    PlayerController playerController;
    [Header("pound Effect")]
    [SerializeField] GameObject poundEffect;
    [SerializeField] Sprite[] poundSprites;
    [SerializeField] float maskRange = 3f;
    [SerializeField] float fadeDelay = 0.5f;
    [SerializeField] float fadeDuration = 1f;
    const int ObstacleLayer = 6;
    const int StickableLayer = 10;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController.SquishEffect += SquishSplatterEffect;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerController.playerState == State.LAUNCHED && collision.gameObject.layer != StickableLayer) 
        {
            /// checking for firstbounce
            playerController.SetToFirstBounce();
           
        }
        else if(playerController.playerState == State.BOUNCE)
        {
            //checking after firstbounce , bring it to rest
           playerController.SetToIdle();

        }
        else if(playerController.playerState == State.POUND)
        {
            playerController.ResetPound();
            LevelManager.Instance.LevelCamera.CameraPoundEffect();

            //check if collided with breakables
            if(collision.gameObject.TryGetComponent<BreakablePT>(out BreakablePT breakablePT))
            {
                breakablePT.OnCollisionPounded();
            }
            else
            {
                //splatter effect
                SplatterEffect(transform.position+new Vector3(0, -1, -1) * maskRange);
            }
        }

        //Debug.Log("object layer :  " + collision.gameObject.layer+" , Mask layer : "+(int)obstacleLayerMask);

        if(collision.collider.gameObject.layer == ObstacleLayer && playerController.playerState!=State.GHOST)
        {
            //hit with obstacle , respawn to last checkpoint
            LevelManager.Instance.LevelCamera.CameraHitEffect();
            playerController.PlayerHitEffect();
        }
    }

    private void SquishSplatterEffect(Vector2 offset)
    {
        for(int i=0;i<3;i++)
        {
            var offsetPos = new Vector3(offset.x,offset.y, -1);
            SplatterEffect(offsetPos);
        }
    }
    private void SplatterEffect(Vector3 position)
    {
        var rotRange = Random.Range(0f, 180f);
        var poundObject = Instantiate(poundEffect, position, Quaternion.Euler(0f, 0f, rotRange));
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
    private void OnDestroy()
    {
        playerController.SquishEffect -= SquishSplatterEffect;
    }
}
