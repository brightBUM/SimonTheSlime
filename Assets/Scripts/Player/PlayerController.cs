using DG.Tweening;
using System;
using System.Collections;
using UnityEditor;
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
    public Action<Vector2> SquishEffect;
    public Action GrappleRangeShrink;
    public bool grappleReady;

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
    private int bulletTimeAbility = 2;
    private float slideAccelerate;
    private const float squishOffset = 1.5f;
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
        GamePlayScreenUI.instance.UpdateBulletTimeUI(bulletTimeAbility);
    }
    private void OnEnable()
    {
        playerInput.mouseReleased += LeftReleased;
        playerInput.mouseDragging += LeftDragging;
        playerInput.PoundAbility += RightClicked;
        playerInput.BulletTimeAbility += ActivateBulletTime;
        playerInput.DashAbility += ActivateDashTime;
        playerInput.GrappleAbility += ActivateGrapple;
        respawnPlayer += RespawnPlayer;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(dashTimer > 0f)
        {
            GamePlayScreenUI.instance.UpdateMidAirJumpUI((dashCooldown - dashTimer)/dashCooldown);
            dashTimer -= Time.deltaTime;
        }

        if(playerState!=State.AIMING)
        {
            camLookAhead.position = Vector3.Lerp(camLookAhead.position, transform.position, camReleaseTime*Time.deltaTime);
        }

        if (playerState == State.GRAPPLE)
        {
            var distance = Vector2.Distance(transform.position, grapplePoint);
            Debug.Log("distance : " + distance);
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

        if (playerState == State.GRAPPLEHANG)
        {
            grappleTimer += Time.deltaTime;
            if (grappleTimer >= grappleGrabHangTimer)
            {
                grappleTimer = 0f;
                playerAnimation.SetRelaunch();
                playerState = State.LAUNCHED;
                ResetGravity();
            }
        }
        
    }
  
    private void LeftDragging(Vector2 mousePos)
    {
        if(!firstClick)
        {
            aimDir = Vector2.zero;
            startPos = mousePos;
            firstClick = true;
            aimCancel = false;
        }

        dragPos = mousePos;
        dir = dragPos - startPos;
        //Debug.Log("dir value : " + dir.magnitude);

        if (dir.magnitude < 0.1f || aimCancel) //single click rejection
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
                    GamePlayScreenUI.instance.NoBulletTimeAbilityFeedback();
                    playerAnimation.ToggleLineRenderer(false);
                    dragging = true;
                    return;
                }
               
            }
            dragging = true;
        }
        // calculate aim force and sprite flip direction
        if (playerState == State.AIMING || playerState == State.TIMEDILATION)
        {
            aimDir = (Vector2)transform.position + (-dir);

            //cam look ahead
            camLookAhead.position = (Vector2)transform.position + (-dir)*camDragSensitivity;

            forceDir = aimDir - (Vector2)transform.position;

            forceDir *= dragSensitivity;
            forceLength = forceDir.magnitude;
            //Debug.Log("forcelength : "+forceLength);
            playerAnimation.FlipSprite(forceDir.normalized);
            playerAnimation.DrawTrajectory(Vector2.ClampMagnitude(forceDir, maxForce));
        }
        else if (playerState == State.STICK || playerState == State.GRAPPLEHANG)
        {
            //aim only to the opp side of the conveyor/sticky platform
           
            aimDir = (Vector2)transform.position + (-dir);
            forceDir = aimDir - (Vector2)transform.position;
            //Debug.Log("dot value :" + Vector2.Dot(Vector2.left, forceDir.normalized));

            forceDir *= dragSensitivity;
            forceLength = forceDir.magnitude;
            playerAnimation.DrawTrajectory(Vector2.ClampMagnitude(forceDir, maxForce));

        }
        
    }
    private void LeftReleased()
    {
        
        if (dir.magnitude < 0.1f || !dragging)
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

        }
        else if (playerState == State.TIMEDILATION)
        {
            playerState = State.LAUNCHED;
            forceDir = Vector2.ClampMagnitude(forceDir, maxForce);
            rb.velocity = forceDir;
        }
        else if (playerState == State.STICK || playerState == State.GRAPPLEHANG)
        {
            RelaunchPlayer();
            ResetGravity();
            playerAnimation.FlipSprite(forceDir.normalized);
            grappleTimer = 0f;
        }
        if (GamePlayScreenUI.instance.BulletTimeActive)
        {
            GamePlayScreenUI.instance.EndBulletTime(bulletTimeAbility);
        }
        dragging = false;
        firstClick = false;
        aimCancel = false;
    }
    private void RightClicked()
    {
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
            GamePlayScreenUI.instance.EndBulletTime(bulletTimeAbility);
            playerAnimation.ToggleLineRenderer(false);
            playerState = State.LAUNCHED;
        }
        else if(playerState == State.STICK)
        {
            aimCancel = true;
            playerAnimation.ToggleLineRenderer(false);
        }
        else if(playerState == State.LAUNCHED)
        {
            //pound only when in mid air
            playerState = State.POUND;
            rb.velocity = Vector2.down * poundForce;
            playerAnimation.SetRoll();
            playerAnimation.PoundTrailEffect();
        }
        //Debug.Log("right mouse clicked");
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
        Time.timeScale = bulletTimeScale;
        //to avoid physics lag during SloMo
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        //start a bullet timer 
        GamePlayScreenUI.instance.StartTimer(bulletTimeAbility, () =>
        {
            aimCancel = true;
            playerState = State.LAUNCHED;
            playerAnimation.ToggleLineRenderer(false);
        });
        
    }
    public void SetToFirstBounce()
    {
        playerState = State.BOUNCE;
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
        yield return new WaitForSeconds(2f);
        lerpAmount = 0;
        playerSquishDummy.SetActive(false);

        playerState = State.GHOST;
        playerAnimation.HitEffect(respawnPlayer);
    }
    public void SetToStickState(float sideValue)
    {
        playerState = State.STICK;
        rb.velocity = Vector2.zero;
        playerAnimation.SetStick(sideValue);
        playerAnimation.ToggleTrailRenderer(false);
        //disable rb to avoid gravity
        //rb.isKinematic = true;
        //collider.enabled = false;
        Physics2D.gravity = Vector2.zero;
        
    }
    public void SlideDown()
    {
        //Physics2D.gravity = Vector2.down * (slideDownValue);
        slideAccelerate += slideDownValue*Time.deltaTime;
        transform.position += Vector3.down*slideAccelerate * Time.deltaTime;
    }
    public void ResetPound()
    {
        SetToIdle();
        playerAnimation.ResetTrailEffect();
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
        playerAnimation.HitEffect(respawnPlayer);
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.AddForce(Vector2.up * onHitUpForce, ForceMode2D.Impulse);
    }
    private void RespawnPlayer()
    {
        //Debug.Log("Player controller Respawn Player");

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

        //var distance = Vector2.Distance(A, D);
        //var duration = distance / 3.0f; // camera follow speed = 3

        //Debug.Log(string.Format($"distance : {distance} , duration : {duration}"));
        //this.A.position = A;
        //this.B.position = B;
        //this.C.position = C;
        //this.D.position = D;

        DOTween.To(() => lerpAmount, x => lerpAmount = x, 1, 1.5f).SetEase(Ease.Linear).OnUpdate(() =>
        {
            var AB = Vector2.Lerp(A, B, lerpAmount);
            var BC = Vector2.Lerp(B, C, lerpAmount);
            var CD = Vector2.Lerp(C, D, lerpAmount);
            var ABC = Vector2.Lerp(AB, BC, lerpAmount);
            var BCD = Vector2.Lerp(BC, CD, lerpAmount);

            var ABCD = Vector2.Lerp(ABC, BCD, lerpAmount);

            this.transform.position = ABCD;

        }).OnComplete(() =>
        {
            //reset to idle
            SetToIdle();
            playerAnimation.DisableGhostParticle();
            collider.enabled = true;
        });

    }

    public void RefillBulletTime()
    {
        bulletTimeAbility += 2;
        GamePlayScreenUI.instance.UpdateBulletTimeUI(bulletTimeAbility);
    }
    public void ResetGravity()
    {
        Physics2D.gravity = Vector2.up * gravity;
        slideAccelerate = 0f;
    }

    private void ActivateDashTime()
    {
        //Vector2 initialVelocity = rb.velocity;
        if(dashTimer<=0)
        {
            playerAnimation.ToggleSpriteTrailRenderer(true);
            rb.AddForce(rb.velocity.normalized * dashAmount, ForceMode2D.Impulse);
            LevelManager.Instance.LevelCamera.CameraPoundEffect();
            dashTimer = dashCooldown;
            DOVirtual.DelayedCall(0.5f, () =>
            {
                playerAnimation.ToggleSpriteTrailRenderer(false);

            });
        }
       
    }
    private void ActivateGrapple()
    {
        if(grappleReady && playerState == State.LAUNCHED)
        {
            rb.velocity = Vector2.zero;
            Physics2D.gravity = Vector2.zero;

            var grappleDirection = grapplePoint - (Vector2)transform.position;
            playerAnimation.FlipSprite(grappleDirection.normalized);
            playerAnimation.SetGrapplePose();
            //activate line renderer
           
            StartCoroutine(grappleRope.AnimateRope(grapplePoint, () =>
            {
                rb.velocity = grappleDirection.normalized * grapplePullSpeed;
                ResetGravity();
                playerState = State.GRAPPLE;
            }));
        }
    }
    public void ExplodeOnContact(float force)
    {
        //add exploding force 
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        //respawn on landing again
    }
    public void SetGrapplePoint(Vector2 point)
    {
        this.grapplePoint = point;
        grappleReady = true;
    }
    public void FreeGrapplePoint()
    {
        this.grapplePoint = Vector2.zero;
        grappleReady = false;
    }
    private void OnDisable()
    {
        playerInput.mouseReleased -= LeftReleased;
        playerInput.mouseDragging -= LeftDragging;
        playerInput.PoundAbility -= RightClicked;
        playerInput.BulletTimeAbility -= ActivateBulletTime;
        playerInput.DashAbility -= ActivateDashTime;
        playerInput.GrappleAbility -= ActivateGrapple;

        respawnPlayer -= RespawnPlayer;
    }

}
