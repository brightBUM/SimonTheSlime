using DG.Tweening;
using System.Collections;
using UnityEngine;
using System;

public class PlayerCollision : MonoBehaviour
{
    PlayerController playerController;
    [Header("pound Effect")]
    [SerializeField] GameObject poundEffect;
    [SerializeField] Sprite[] poundSprites;
    [SerializeField] float maskRange = 3f;
    [SerializeField] float fadeDelay = 0.5f;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float rayCastLength = 1.3f;
    [SerializeField] float topStickDuration = 1.15f;
    [SerializeField] float downStickDuration = 0.5f;

    [SerializeField] LayerMask platformLayer;
    [SerializeField] LayerMask breakableLayer;
    [SerializeField] LayerMask conveyorLayer;
    const int ObstacleLayer = 6;
    const int platformLayerValue = 7;
    const int breakableLayerValue = 9;
    const int StickableLayer = 10;
    bool hit;
    int stickSide = 0;
    float stickTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController.SquishEffect += SquishSplatterEffect;
    }

    private void FixedUpdate()
    {
        if (playerController.playerState == State.STICK)
        {
            switch (stickSide)
            {
                case 1:
                    RaycastCheckDirection(-transform.right, () =>
                    {
                        playerController.SlideDown();

                    }, () =>
                    {
                        //no longer is contact with side collider , drop off player
                        playerController.ResetGravity();
                        playerController.SetToIdle();
                    });
                    break;
                case 2:
                    RaycastCheckDirection(transform.right, () =>
                    {
                        playerController.SlideDown();

                    }, () =>
                    {
                        playerController.ResetGravity();
                        playerController.SetToIdle();

                    });
                    break;
                case 3:
                    stickTimer += Time.deltaTime;
                    if (stickTimer >= topStickDuration)
                    {
                        stickTimer = 0;
                        playerController.ResetGravity();
                        playerController.SetToIdle();
                    }
                    break;
                case 4:
                    stickTimer += Time.deltaTime;
                    if (stickTimer >= downStickDuration)
                    {
                        stickTimer = 0;
                        playerController.ResetGravity();
                        playerController.SetToIdle();
                    }
                    break;
                default:
                    break;
            }
        }
    }
    private void RaycastCheckDirection(Vector3 direction, Action hitAction = null, Action missAction = null)
    {
        var rayCastHit2D = Physics2D.Raycast(transform.position, direction, rayCastLength, platformLayer | breakableLayer);
        if (rayCastHit2D.collider != null)
        {
            hit = true;
            hitAction?.Invoke();
        }
        else
        {
            hit = false;
            missAction?.Invoke();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerController.playerState == State.LAUNCHED && 
            (collision.gameObject.layer == platformLayerValue || collision.gameObject.layer == breakableLayerValue))
        {
            //raycast 4 ways to detect platform sides
            //change to stick state and set sprite based on collision direction
            //Debug.Log("collided after launch");

            RaycastCheckDirection(-transform.right, () =>
            {
                playerController.SetToStickState(1f);
                stickSide = 1;
                return;
            });

            RaycastCheckDirection(transform.right, () =>
            {
                //Debug.Break();
                playerController.SetToStickState(2f);
                stickSide = 2;
                return;
            });

            RaycastCheckDirection(transform.up, () =>
            {
                playerController.SetToStickState(3f);
                stickSide = 3;
                return;

            });

            RaycastCheckDirection(-transform.up, () =>
            {
                //set to idle when thrown down
                playerController.SetToStickState(4f);
                stickSide = 4;
                return;
            });
        }
        else if (playerController.playerState == State.BOUNCE)
        {
            //checking after firstbounce , bring it to rest
            playerController.SetToIdle();

        }
        else if (playerController.playerState == State.POUND)
        {
            playerController.ResetPound();
            LevelManager.Instance.ShakeCamera.OnPound();

            //check if collided with breakables
            if (collision.gameObject.TryGetComponent<BreakablePT>(out BreakablePT breakablePT))
            {
                breakablePT.OnCollisionPounded();
            }
            else
            {
                //splatter effect
                SoundManager.instance.PlayPoundSFx();
                SplatterEffect(transform.position + new Vector3(0, -1, -1) * maskRange);
            }
        }

        //Debug.Log("object layer :  " + collision.gameObject.layer+" , Mask layer : "+(int)obstacleLayerMask);

        if (collision.collider.gameObject.layer == ObstacleLayer && playerController.playerState != State.GHOST)
        {
            //hit with obstacle , respawn to last checkpoint
            SoundManager.instance.PlayGhostRespawnSFx(true);
            LevelManager.Instance.ShakeCamera.OnHit();
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
        var rotRange = UnityEngine.Random.Range(0f, 180f);
        var poundObject = Instantiate(poundEffect, position, Quaternion.Euler(0f, 0f, rotRange));
        var poundSprite = poundObject.GetComponent<SpriteRenderer>();
        poundSprite.sprite = poundSprites[UnityEngine.Random.Range(0, poundSprites.Length)];

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

    private void OnDrawGizmos()
    {
        Gizmos.color = hit ? Color.blue : Color.red;
        Gizmos.DrawRay(transform.position, rayCastLength * transform.right);
        //Gizmos.DrawRay(transform.position, 5f * transform.right);
    }
}
