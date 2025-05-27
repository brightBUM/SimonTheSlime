using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public enum State
{
    IDLE,
    AIMING,
    LAUNCHED,
    TIMEDILATION,
    STICK,
    BOUNCE,
    GRAPPLE,
    GRAPPLEHANG,
    POUND,
    SQUISHED,
    GHOST
}
public class PlayerController : MonoBehaviour
{
    public float Velocity => rb.velocity.y;
    public State playerState;
    public StickSide stickSide;
    public Action<Vector2> SquishEffect;
    public Action GrappleRangeShrink;
    public Action GrappleRelaunch;
    public Action<IPoundable> ContinuePound;
    public bool grappleReady;
    public bool poundHeld = false;

    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] GameObject playerSquishDummy;
    [SerializeField] GrappleRope grappleRope;
    [Header("cam")]
    [SerializeField] Transform camLookAhead;
    [SerializeField] float camLookAheadDistance = 5f;
    [SerializeField] float camDragSensitivity = 2f;
    [SerializeField] float camReleaseTime = 1f;
    [Header("Aim")]
    [SerializeField] float dragSensitivity;
    [SerializeField] float poundForce = 10f;
    [SerializeField] float maxForce;
    [SerializeField] float forceLength;
    [SerializeField] float gravity = -20f;
    [Header("Ability")]
    [SerializeField] float dashAmount = 2f;
    [SerializeField] float grapplePullSpeed = 6f;
    [SerializeField] float grappleGrabHangTimer = 5f;
    [SerializeField] float grappledOffSpeed = 10f;
    [SerializeField] float onHitUpForce = 3f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] float bulletTimeScale = 0.5f;
    [SerializeField] float slideDownValue = 0.5f;
    [SerializeField] bool debugVectors;

    private bool dragging = false;
    private bool firstClick = false;
    private bool aimCancel = false;
    private bool aiminLimit = false;
    private Vector2 startPos;
    private Vector2 dragPos;
    private Vector2 dir;
    private Vector2 aimDir;
    private Vector2 forceDir;
    private Vector2 grapplePoint;
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Action respawnPlayer;
    private CircleCollider2D collider;
    private float lerpAmount = 0f;
    private float dashTimer = 1f;
    private float grappleTimer = 0f;
    private int bulletTimeAbility = 0;
    private float slideAccelerate;
    private const float squishOffset = 0.5f;
    private bool respawning;
    private Vector3 lastPos;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        Physics2D.gravity = Vector2.up * gravity;
    }
    void Start()
    {
        playerState = State.IDLE;
        collider = GetComponent<CircleCollider2D>();
        GamePlayScreenUI.Instance.UpdateBulletTimeUI(bulletTimeAbility);
        
    }
    private void OnEnable()
    {
        playerInput.mouseReleased += LeftReleased;
        playerInput.mouseDragging += LeftDragging;
        playerInput.PoundAbility += RightClicked;
        playerInput.PoundReleased += RightClickReleased;
        //playerInput.BulletTimeAbility += ActivateBulletTime;
        playerInput.DashAbility += ActivateDashTime;
        playerInput.GrappleAbility += ActivateGrapple;
        playerInput.RespawnToCheckPoint += ResetStates;
        playerInput.DoubleTapAbility += ActivateDoubleTapAbility;
        respawnPlayer += RespawnPlayer;
        ContinuePound += ContinuePounding;

#if UNITY_ANDROID
        GamePlayScreenUI.Instance.poundAbilityAction += RightClicked;
        GamePlayScreenUI.Instance.poundReleaseAction += RightClickReleased;
        GamePlayScreenUI.Instance.dashButtonAction += ActivateDashTime;
        GamePlayScreenUI.Instance.grappleButtonAction += ActivateGrapple;
#endif

    }
    private void FixedUpdate()
    {
        if (respawning)
        {
            var dir = playerAnimation.transform.position - lastPos;
            var rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            lastPos = transform.position;
            playerAnimation.ghostDummyVisual.transform.rotation = Quaternion.Euler(0, 0, rot);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(dashTimer > 0f)
        {
            GamePlayScreenUI.Instance.UpdateMidAirJumpUI((dashCooldown - dashTimer)/dashCooldown);
            dashTimer -= Time.deltaTime;
        }

        if(playerState!=State.AIMING)
        {
            camLookAhead.position = Vector3.Lerp(camLookAhead.position, transform.position, camReleaseTime*Time.deltaTime);
        }

        if (playerState == State.GRAPPLE)
        {
            var distance = Vector2.Distance(transform.position, grapplePoint);
            //Debug.Log("distance : " + distance);
            if (distance < 1.0f)
            {
                //Debug.Log("grapple distance : " + distance);
                //rb.velocity = rb.velocity.normalized * grappledOffSpeed;
                GrappleRangeShrink.Invoke();
                rb.velocity = Vector2.zero;
                Physics2D.gravity = Vector2.zero;
                playerAnimation.SetGrappleGrab();
                playerState = State.GRAPPLEHANG;
                //playerState = State.LAUNCHED;
                
            }
        }
    }
  
    private void LeftDragging(Vector2 mousePos)
    {
        if (playerState == State.STICK)
        {
            if (stickSide == StickSide.TOP || stickSide == StickSide.BOTTOM)
            {
                return;
            }
        }

        if (playerState == State.POUND || playerState == State.GHOST || playerState == State.GRAPPLE)
            return;

        if (!firstClick)
        {
            aimDir = Vector2.zero;
            startPos = mousePos;
            firstClick = true;
            aimCancel = false;
        }

        dragPos = mousePos;
        dir = dragPos - startPos;

        if (dir.magnitude < 1.0f || aimCancel) //single click rejection
            return;


        if(!dragging) //set visual elements only once when dragging
        {
            playerAnimation.ToggleLineRenderer(true);
            if (playerState == State.IDLE)
            {
                playerState = State.AIMING;
                playerAnimation.SetAim();
            }
            else if(playerState == State.LAUNCHED)
            {
                if (bulletTimeAbility > 0)
                {
                    ActivateBulletTime();
                    playerState = State.TIMEDILATION;
                }
                else
                {
                    GamePlayScreenUI.Instance.NoBulletTimeAbilityFeedback();
                    playerAnimation.ToggleLineRenderer(false);
                    dragging = true;
                    return;
                }
               
            }
            dragging = true;
        }
        // calculate aim force and sprite flip direction
        if (playerState == State.AIMING)
        {
            aimDir = (Vector2)transform.position + (-dir);

            //cam look ahead
            camLookAhead.position = (Vector2)transform.position + (-dir)*camDragSensitivity;

            forceDir = aimDir - (Vector2)transform.position;

            //Vector3 clampedForce = transform.position + Vector3.ClampMagnitude(forceDir, maxForce);
           
            float dotvalueUp = Vector2.Dot(((transform.position+Vector3.up) - transform.position).normalized, forceDir.normalized);
            //Debug.Log("dot value : " + dotvalueUp);
            aiminLimit = dotvalueUp > 0.1f ? true : false; 

            forceDir *= dragSensitivity;
            forceLength = forceDir.magnitude;
            playerAnimation.FlipSprite(forceDir.normalized);
            playerAnimation.DrawTrajectory(Vector2.ClampMagnitude(forceDir, maxForce),aiminLimit);
        }
        else if (playerState == State.STICK)
        {
            //aim only to the opp side of the conveyor/sticky platform
            
            aimDir = (Vector2)transform.position + (-dir);
            forceDir = aimDir - (Vector2)transform.position;
            //Debug.Log("dot value :" + Vector2.Dot(Vector2.left, forceDir.normalized));

            if(stickSide == StickSide.LEFT)
            {
                //Vector3 clampedForce = transform.position + Vector3.ClampMagnitude(forceDir, maxForce);
                float dotvalueLeft = Vector2.Dot(((transform.position + Vector3.right) - transform.position).normalized, forceDir.normalized);
                aiminLimit = dotvalueLeft > 0.1f ? true : false;

            }
            else if(stickSide == StickSide.RIGHT)
            {
                //Vector3 clampedForce = transform.position + Vector3.ClampMagnitude(forceDir, maxForce);
                var dotvalueRight = Vector2.Dot(((transform.position + Vector3.left) - transform.position).normalized, forceDir.normalized);
                aiminLimit = dotvalueRight > 0.1f ? true : false;
            }

            forceDir *= dragSensitivity;
            forceLength = forceDir.magnitude;
            playerAnimation.DrawTrajectory(Vector2.ClampMagnitude(forceDir, maxForce),aiminLimit);

        }
        else if (playerState == State.TIMEDILATION || playerState == State.GRAPPLEHANG)
        {
            // aim 360 in mid air
            aimDir = (Vector2)transform.position + (-dir);
            forceDir = aimDir - (Vector2)transform.position;

            forceDir *= dragSensitivity;
            forceLength = forceDir.magnitude;
            aiminLimit = true;
            playerAnimation.FlipSprite(forceDir.normalized);
            playerAnimation.DrawTrajectory(Vector2.ClampMagnitude(forceDir, maxForce), aiminLimit);
        }

    }
    private void LeftReleased()
    {
        if(!aiminLimit)
        {
            if(playerState==State.AIMING)
            {
                SetToIdle();
            }
            playerAnimation.ToggleLineRenderer(false);
            dragging = false;
            firstClick = false;
            aimCancel = false;
            return;
        }
        if (playerState == State.STICK)
        {
            if (stickSide == StickSide.TOP || stickSide == StickSide.BOTTOM)
            {
                return;
            }
        }
        if (playerState == State.POUND || playerState == State.GHOST)
            return;
        if (dir.magnitude < 1.0f || !dragging)
        {
            firstClick = false;
            aimCancel = false;
            return;
        }

        
        playerAnimation.ToggleLineRenderer(false);

        //launch
        if (playerState == State.AIMING)
        {
            playerState = State.LAUNCHED;
            forceDir = Vector2.ClampMagnitude(forceDir, maxForce);
            rb.velocity = forceDir;
            playerAnimation.ToggleTrailRenderer(true);
            playerAnimation.SetLaunch();
           

        }
        else if (playerState == State.TIMEDILATION)
        {
            playerState = State.LAUNCHED;
            forceDir = Vector2.ClampMagnitude(forceDir, maxForce);

            rb.velocity = forceDir;

        }
        else if (playerState == State.STICK || playerState == State.GRAPPLEHANG)
        {
            if(playerState == State.GRAPPLEHANG)
            {
                GrappleRelaunch.Invoke();
            }
            RelaunchPlayer();
            ResetGravity();
            playerAnimation.FlipSprite(forceDir.normalized);

            grappleTimer = 0f;
        }
        if (GamePlayScreenUI.Instance.BulletTimeActive)
        {
            ResetGravity();
            GamePlayScreenUI.Instance.EndBulletTime(bulletTimeAbility);
        }

        dragging = false;
        firstClick = false;
        aimCancel = false;
    }
    public void PoundButton()
    {
        RightClicked();
    }
    
    private void RightClicked()
    {
        poundHeld = true;
        if(playerState == State.AIMING)
        {
            //cancel aim 
            aimCancel = true;
            playerAnimation.ToggleLineRenderer(false);
            SetToIdle();
        }
        else if(playerState == State.TIMEDILATION)
        {
            //cancel aim 
            aimCancel = true;
            GamePlayScreenUI.Instance.EndBulletTime(bulletTimeAbility);
            ResetGravity();
            playerAnimation.ToggleLineRenderer(false);
            playerState = State.LAUNCHED;
        }
        else if(playerState == State.STICK || playerState == State.GRAPPLEHANG)
        {
            aimCancel = true;
            playerAnimation.ToggleLineRenderer(false);
        }
        else if(playerState == State.LAUNCHED)
        {
            //pound only when in mid air

            StartCoroutine(DelayedPound());
        }
    }

    private void RightClickReleased()
    {
        poundHeld = false;
    }
    IEnumerator DelayedPound()
    {
        //slam transit animation in mid air
        Physics2D.gravity = Vector2.zero;
        rb.velocity = Vector2.zero;
        playerAnimation.SetRoll();
        playerState = State.POUND;
        
        yield return new WaitForSeconds(0.2f);

        rb.velocity = Vector2.down * poundForce;
        ResetGravity();
        playerAnimation.PoundTrailEffect();
    }
    private void RelaunchPlayer()
    {
        playerState = State.LAUNCHED;
        forceDir = Vector2.ClampMagnitude(forceDir, maxForce);
        rb.velocity = forceDir;
        playerAnimation.SetRelaunch();
        playerAnimation.ToggleTrailRenderer(true);
    }

    private void ActivateBulletTime()
    {
        bulletTimeAbility--;
        playerState = State.TIMEDILATION;
        Physics2D.gravity = Vector2.zero;
        rb.velocity = Vector2.zero;
        Time.timeScale = bulletTimeScale;
        //to avoid physics lag during SloMo
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        //start a bullet timer 
        SoundManager.Instance.PlaySloMoTimer();
        GamePlayScreenUI.Instance.StartTimer(bulletTimeAbility, () =>
        {
            aimCancel = true;
            playerState = State.LAUNCHED;
            playerAnimation.ToggleLineRenderer(false);
            ResetGravity();
        });
        
    }
    public void SetToFirstBounce()
    {
        //playerState = State.BOUNCE;
        rb.velocity = Vector2.zero;
        Physics2D.gravity = Vector2.down*3f;
        playerAnimation.SetRoll();
    }
    public void SetToIdle()
    {
        playerState = State.IDLE;
        rb.velocity = Vector2.zero;
        playerAnimation.SetIdle();
        playerAnimation.ToggleTrailRenderer(false);
        playerAnimation.ResetVelocity();
    }
    public void SetToSquishState(Vector3 stickPos,HitDirection hitDirection)
    {
        playerAnimation.ToggleTrailRenderer(false);
        playerAnimation.ToggleSpriteRenderer(false);
        playerSquishDummy.SetActive(true);
        playerSquishDummy.transform.position = stickPos;
        Vector3 offset;
        Vector3 pos = playerSquishDummy.transform.position;
        //dummy orientation
        switch(hitDirection)
        {
            case HitDirection.Left:
                playerSquishDummy.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                playerSquishDummy.GetComponent<SpriteRenderer>().flipY = true;
                offset = new Vector2(-UnityEngine.Random.Range(squishOffset, squishOffset + 1), UnityEngine.Random.Range(-squishOffset, squishOffset));
                SquishEffect.Invoke(pos+offset);
                break;
            case HitDirection.Right:
                playerSquishDummy.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                offset = new Vector2(UnityEngine.Random.Range(squishOffset, squishOffset + 1), UnityEngine.Random.Range(-squishOffset, squishOffset));
                SquishEffect.Invoke(pos + offset);
                break;
            case HitDirection.Up:
                //direction
                playerSquishDummy.transform.rotation = Quaternion.Euler(Vector3.zero);
                playerSquishDummy.GetComponent<SpriteRenderer>().flipY = true;
                //offset
                offset = new Vector2(UnityEngine.Random.Range(-squishOffset, squishOffset), UnityEngine.Random.Range(squishOffset-1, squishOffset));
                SquishEffect.Invoke(pos + offset);
                break;
            case HitDirection.Down:
                //direction
                playerSquishDummy.transform.rotation = Quaternion.Euler(Vector3.zero);
                //offset
                offset = new Vector2(UnityEngine.Random.Range(-squishOffset, squishOffset), -UnityEngine.Random.Range(squishOffset, squishOffset + 1));
                SquishEffect.Invoke(pos + offset);
                break;
            
        }

        playerState = State.SQUISHED;
        rb.velocity = Vector2.zero;
        //playerAnimation.SetSquish();
        StartCoroutine(DelayedRespawn());
    }
    
    IEnumerator DelayedRespawn()
    {
        lerpAmount = 0;
        playerState = State.GHOST;
        yield return new WaitForSeconds(0.5f);

        GamePlayScreenUI.Instance.ShowRetryScreen();
    }
    public void SetToStickState(StickSide stickSide)
    {
        playerState = State.STICK;
        this.stickSide = stickSide;
        rb.velocity = Vector2.zero;
        playerAnimation.SetStick((int)stickSide+1);
        playerAnimation.ToggleTrailRenderer(false);
        SoundManager.Instance.PlayStickSFx();
        //disable rb to avoid gravity
        
        Physics2D.gravity = Vector2.zero;
        
    }
    public void SlideDown()
    {
        //Physics2D.gravity = Vector2.down * (slideDownValue);
        slideAccelerate += slideDownValue*Time.deltaTime;
        transform.position += Vector3.down*slideAccelerate * Time.deltaTime;
    }
    private void ContinuePounding(IPoundable poundable)
    {
        if(poundable is BreakablePlatform)
        {
            if (poundHeld)
            {
                PushPound();
                return;
            }
        }
        ResetPound();
    }
    public void ResetPound()
    {
        SetToIdle();
        playerAnimation.ResetTrailEffect();
    }
    private void PushPound()
    {
        rb.velocity += Vector2.down * 20f;
    }
    public Vector2 GetPosition(Vector2 vel,float t)
    {
        var pos = (Vector2)transform.position + vel * t+0.5f*Vector2.up*gravity*t*t;
        return pos;
    }
    public void PlayerHitEffect()
    {
        lerpAmount = 0;
        playerState = State.GHOST;
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.AddForce(Vector2.up * onHitUpForce, ForceMode2D.Impulse);

        //trigger retry screen here (also pause the game )
        DOVirtual.DelayedCall(0.3f,() =>
        {
            GamePlayScreenUI.Instance.ShowRetryScreen();
        });
    }
    public void DelayedRespawn(float time)
    {
        DOVirtual.DelayedCall(time, () =>
        {
            playerSquishDummy.SetActive(false);
            SoundManager.Instance.PlayGhostRespawnSFx(true);
            playerAnimation.HitEffect(respawnPlayer);
        });
    }
    private void RespawnPlayer()
    {
        //move to last checkpoint with ghost effect

        //disable colliders and change to ghost sprite
        collider.enabled = false;

        //ghost curve path via tweening
        //generates random curve between player position and last checkpoint
        Vector2 A, B, C, D;
        A = transform.position;
        D = LevelManager.Instance.LastCheckpointpos;

        var distance = Vector2.Distance(A, D);
        var mid = (A + D) / 2;
        var Amid = (A + mid )/2;
        var Dmid = (D + mid )/2;
        var dir1 = D - Amid;
        var dir2 = D - Dmid;

        B = Amid + (Vector2.Perpendicular(dir1.normalized) * distance/2);
        C = Dmid + (Vector2.Perpendicular(dir2.normalized) *-1f* distance/2);
       

        respawning = true;
        
        //flip ghost sprite
        playerAnimation.ghostDummyVisual.flipY = D.x < A.x ? true:false;

        DOTween.To(() => lerpAmount, x => lerpAmount = x, 1, 1.5f).SetEase(Ease.Linear).OnUpdate(() =>
        {
            var AB = Vector2.Lerp(A, B, lerpAmount);
            var BC = Vector2.Lerp(B, C, lerpAmount);
            var CD = Vector2.Lerp(C, D, lerpAmount);
            var ABC = Vector2.Lerp(AB, BC, lerpAmount);
            var BCD = Vector2.Lerp(BC, CD, lerpAmount);

            var ABCD = Vector2.Lerp(ABC, BCD, lerpAmount);

            transform.position = ABCD;



        }).OnComplete(() =>
        {
            respawning = false;
            playerAnimation.DisableGhostParticle();
            SoundManager.Instance.PlayGhostRespawnSFx(false);
            LevelManager.Instance.LastCheckPointEffect();
            playerAnimation.transform.rotation = Quaternion.Euler(Vector3.zero);
            collider.enabled = true;
            playerAnimation.ToggleGhostDummy(false);

            DOVirtual.DelayedCall(0.9f, () => {

                //reset to idle
                SetToIdle();
                ResetGravity();
                playerAnimation.ToggleSpriteRenderer(true);
                
            });

        });

    }

    public void RefillBulletTime()
    {
        bulletTimeAbility += 2;
        bulletTimeAbility = Mathf.Clamp(bulletTimeAbility, 0, 2);
        GamePlayScreenUI.Instance.UpdateBulletTimeUI(bulletTimeAbility);
    }
    public void ResetGravity()
    {
        Physics2D.gravity = Vector2.up * gravity;
        slideAccelerate = 0f;
    }

    public void ActivateDoubleTapAbility()
    {

#if UNITY_ANDROID
        if(grappleReady)
        {
            ActivateGrapple();
        }
        else
        {
            ActivateDashTime();
        }
#endif
    }
    public void ActivateDashTime()
    {
        //Vector2 initialVelocity = rb.velocity;
        if(dashTimer<=0 && playerState == State.LAUNCHED)
        {
            playerAnimation.ToggleSpriteTrailRenderer(true);
            rb.AddForce(rb.velocity.normalized * dashAmount, ForceMode2D.Impulse);
            LevelManager.Instance.ShakeCamera.OnDash();
            SoundManager.Instance.PlayDashSFX();
            dashTimer = dashCooldown;
            DOVirtual.DelayedCall(0.5f, () =>
            {
                playerAnimation.ToggleSpriteTrailRenderer(false);

            });
        }
       
    }
    public void ActivateGrapple()
    {
        if(grappleReady && playerState == State.LAUNCHED)
        {
            rb.velocity = Vector2.zero;
            Physics2D.gravity = Vector2.zero;

            var grappleDirection = grapplePoint - (Vector2)transform.position;
            playerAnimation.FlipSprite(grappleDirection.normalized);
            playerAnimation.SetGrapplePose();
            SoundManager.Instance.PlayGrappleRopeSFX();

            playerState = State.GRAPPLE;

            //activate line renderer
            StartCoroutine(grappleRope.AnimateRope(grapplePoint, () =>
            {
                if(playerState!=State.GRAPPLEHANG)
                {
                    rb.velocity = grappleDirection.normalized * grapplePullSpeed;
                }
                playerAnimation.ToggleTrailRenderer(false);   
                LevelManager.Instance.ShakeCamera.OnGrapple();
                SoundManager.Instance.PlayGrapplePullSFX();

            }));
        }
    }
    public void ResetStates()
    {
        PlayerHitEffect();
        playerAnimation.ResetAll();
    }
    public void ExplodeOnContact(float force)
    {
        //add exploding force 
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            PlayerHitEffect();
        });
    }
    public void SetGrapplePoint(Vector2 point,ref Action playerDropped)
    {
        this.grapplePoint = point;
        grappleReady = true;
        playerDropped += DetachPlayerFromDrone;
    }
    public void FreeGrapplePoint(ref Action playerDropped)
    {
        this.grapplePoint = Vector2.zero;
        grappleReady = false;
        playerDropped -= DetachPlayerFromDrone;
    }
    private void DetachPlayerFromDrone()
    {
        if (playerState == State.GRAPPLEHANG)
        {
            grappleTimer = 0f;
            playerAnimation.SetRelaunch();
            playerState = State.LAUNCHED;
            GrappleRelaunch.Invoke();
            ResetGravity();
        }
    }
    private void OnDisable()
    {
        playerInput.mouseReleased -= LeftReleased;
        playerInput.mouseDragging -= LeftDragging;
        playerInput.PoundAbility -= RightClicked;
        //playerInput.BulletTimeAbility -= ActivateBulletTime;
        playerInput.DashAbility -= ActivateDashTime;
        playerInput.GrappleAbility -= ActivateGrapple;
        playerInput.RespawnToCheckPoint -= ResetStates;
        playerInput.DoubleTapAbility -= ActivateDoubleTapAbility;

        respawnPlayer -= RespawnPlayer;
        ContinuePound -= ContinuePounding;

#if UNITY_ANDROID
        GamePlayScreenUI.Instance.poundAbilityAction += RightClicked;
        GamePlayScreenUI.Instance.poundReleaseAction += RightClickReleased;
        GamePlayScreenUI.Instance.dashButtonAction    -= ActivateDashTime;
        GamePlayScreenUI.Instance.grappleButtonAction -= ActivateGrapple;
#endif

    }
    

}
