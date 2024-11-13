using DG.Tweening;
using System.Collections;
using UnityEngine;
using System;

public class PlayerCollision : MonoBehaviour
{
    PlayerController playerController;
    [Header("pound Effect")]
    [SerializeField] GameObject poundEffect;
    [SerializeField] GameObject gorePrefab;
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
    //int stickSide = 0;
    float stickTimer = 0f;
    ParticleSystem goreFx;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerController.SquishEffect += SquishSplatterEffect;
        if(gorePrefab!=null)
            goreFx = gorePrefab.GetComponentInChildren<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        if (playerController.playerState == State.STICK)
        {
            //additional raycast check for left-right/bottom conditions (i.e player in a corner)
            RaycastCheckDirection(-transform.up, () =>
            {
                if(playerController.stickSide == StickSide.LEFT || playerController.stickSide == StickSide.RIGHT)
                {
                    playerController.ResetGravity();
                    playerController.SetToIdle();
                }
            });

            switch (playerController.stickSide)
            {
                case StickSide.LEFT:
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
                case StickSide.RIGHT:
                    RaycastCheckDirection(transform.right, () =>
                    {
                        playerController.SlideDown();

                    }, () =>
                    {
                        playerController.ResetGravity();
                        playerController.SetToIdle();

                    });
                    break;
                case StickSide.TOP:
                    stickTimer += Time.fixedDeltaTime;
                    if (stickTimer >= topStickDuration)
                    {
                        stickTimer = 0;
                        playerController.ResetGravity();
                        playerController.SetToIdle();
                    }
                    break;
                case StickSide.BOTTOM:
                    stickTimer += Time.fixedDeltaTime;
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
            //hit = true;
            hitAction?.Invoke();
        }
        else
        {
            //hit = false;
            missAction?.Invoke();
        }
    }

    public bool RaycastCheckDirection(Vector3 dir,StickSide stickSide,float distance = 1.3f)
    {
        var rayCastHit2D = Physics2D.Raycast(transform.position, dir, distance, platformLayer | breakableLayer);
        if(rayCastHit2D.collider!=null)
        {
            playerController.SetToStickState(stickSide);
            return true; 
        }
        else
        {
            return false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerController.playerState == State.LAUNCHED && 
            (collision.gameObject.layer == platformLayerValue || collision.gameObject.layer == breakableLayerValue))
        {
            //raycast 4 ways to detect platform sides

            if (RaycastCheckDirection(-transform.right,StickSide.LEFT))
                return;
            else if (RaycastCheckDirection(transform.right, StickSide.RIGHT))
                return;
            else if (RaycastCheckDirection(transform.up, StickSide.TOP))
                return;
            else if (RaycastCheckDirection(-transform.up, StickSide.BOTTOM))
                return;
            
            //corner Bug fix - force the player to down stick condition 

            Vector2 rightDownDir = transform.right + (- transform.up);
            Vector2 leftDownDir = (-transform.right) + (- transform.up);
            
            if (RaycastCheckDirection(rightDownDir.normalized, StickSide.BOTTOM,1.8f)) // raycast check - rightDown
                return;
            else if (RaycastCheckDirection(leftDownDir.normalized, StickSide.BOTTOM,1.8f)) // raycast check - rightDown
                return;

        }
        else if (playerController.playerState == State.BOUNCE)
        {
            //checking after firstbounce , bring it to rest
            playerController.SetToIdle();

        }
        else if (playerController.playerState == State.POUND)
        {
            //check if collided with breakables
            if (collision.gameObject.TryGetComponent<IPoundable>(out IPoundable IPoundable))
            {
                IPoundable.OnPlayerPounded(playerController.ContinuePound);
                LevelManager.Instance.ShakeCamera.OnPound();
            }
            else
            {
                //splatter effect
                SoundManager.instance.PlayPoundSFx();
                SplatterEffect(transform.position + new Vector3(0, -1, -1) * maskRange);
                playerController.ResetPound();
                LevelManager.Instance.ShakeCamera.OnPound();

            }
        }


        if (collision.collider.gameObject.layer == ObstacleLayer && playerController.playerState != State.GHOST)
        {
            //hit with obstacle , respawn to last checkpoint

            if(collision.gameObject.GetComponent<patrol>())
            {
                //LevelManager.Instance.ShakeCamera.OnHit();
                var dir = this.transform.position- collision.transform.position;
                var rot = Quaternion.Euler(0, 0, MathF.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                gorePrefab.transform.position = transform.position;
                gorePrefab.transform.rotation = rot;
                goreFx.Play();
                //Debug.Break();
                playerController.PlayerHitEffect();
            }
            else
            {
                LevelManager.Instance.ShakeCamera.OnHit();
                playerController.PlayerHitEffect();
            }
            
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
        var poundObject = ObjectPoolManager.Instance.Spawn(0, position, Quaternion.Euler(0f, 0f, rotRange));
        var poundSprite = poundObject.GetComponent<SpriteRenderer>();
        poundSprite.sprite = poundSprites[UnityEngine.Random.Range(0, poundSprites.Length)];
        poundSprite.color = new Color(poundSprite.color.r, poundSprite.color.g, poundSprite.color.b, 1f);

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
            ObjectPoolManager.Instance.Despawn(sprite.gameObject, 0);
        });
    }
    private void OnDestroy()
    {
        playerController.SquishEffect -= SquishSplatterEffect;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = hit ? Color.blue : Color.red;
        var dir = transform.right + (-transform.up);
        Gizmos.DrawRay(transform.position, rayCastLength * dir.normalized);
        //Gizmos.DrawRay(transform.position, 5f * transform.right);
    }
}
public enum StickSide
{
    LEFT,
    RIGHT,
    TOP,
    BOTTOM
}
