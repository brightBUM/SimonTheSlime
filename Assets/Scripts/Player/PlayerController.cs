using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public enum State
{
    IDLE,
    AIMING,
    LAUNCHED,
    STICK,
    BOUNCE,
    POUND,
    SQUISHED,
    GHOST
}
public class PlayerController : MonoBehaviour
{
    public float Velocity => rb.velocity.y;
    public State playerState;
    public Action<Vector2> SquishEffect;

    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] GameObject playerSquishDummy;
    [SerializeField] float dragSensitivity;
    [SerializeField] float poundForce = 10f;
    [SerializeField] float maxForce;
    [SerializeField] float forceLength;
    [SerializeField] float gravity = -20f;
    [SerializeField] float onHitUpForce = 3f;
    [SerializeField] float midAirJumpCooldown = 1f;
    [SerializeField] float bulletTimeScale = 0.5f;
    [SerializeField] bool debugVectors;
    
    private Vector2 startPos;
    private Vector2 dragPos;
    private Vector2 dir;
    private Vector2 aimDir;
    private Vector2 forceDir;
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private Action respawnPlayer;
    private CircleCollider2D collider;
    private float lerpAmount = 0f;
    private float jumpTimer = 1f;
    private int bulletTimeAbility = 0;
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
        playerInput.mouseClicked += LeftClicked;
        playerInput.mouseReleased += LeftReleased;
        playerInput.mouseDragging += LeftDragging;
        playerInput.rightClicked += RightClicked;
        playerInput.QkeyPressed += ActivateBulletTime;
        respawnPlayer += RespawnPlayer;
    }
    // Update is called once per frame
    void Update()
    {
        if(jumpTimer > 0f)
        {
            GamePlayScreenUI.instance.UpdateMidAirJumpUI((midAirJumpCooldown - jumpTimer)/midAirJumpCooldown);
            jumpTimer -= Time.deltaTime;
        }
    }
    private void StartPosConfig(Vector2 mousePos)
    {
        aimDir = Vector2.zero;
        playerAnimation.ToggleLineRenderer(true);
        startPos = mousePos;

    }
    private void LeftClicked(Vector2 mousePos)
    {
       
        if(playerState == State.IDLE)
        {
            playerState = State.AIMING;
            playerAnimation.SetAim();
            StartPosConfig(mousePos);
        }
        else if (playerState == State.BOUNCE)
        {
            if(jumpTimer <= 0f)
            {
                //can jump in mid air 
                StartPosConfig(mousePos);
            }
        }
        else if(playerState == State.STICK)
        {
            StartPosConfig(mousePos);
        }
    }
    private void LeftDragging(Vector2 mousePos)
    {
        // calculate aim force and sprite flip direction
        if (playerState == State.AIMING || playerState == State.BOUNCE)
        {
            dragPos = mousePos;
            dir = dragPos - startPos;
            aimDir = (Vector2)transform.position + (-dir);
            forceDir = aimDir - (Vector2)transform.position;
            forceDir *= dragSensitivity;
            forceLength = forceDir.magnitude;
            //Debug.Log("forcelength : "+forceLength);
            playerAnimation.FlipSprite(forceDir.normalized);
            playerAnimation.DrawTrajectory(Vector2.ClampMagnitude(forceDir, maxForce));
        }
        else if(playerState == State.STICK)
        {
            //aim only to the opp side of the conveyor/sticky platform
            dragPos = mousePos;
            dir = dragPos - startPos;
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
        
        playerAnimation.ToggleLineRenderer(false);
        
        if (forceLength < 1)
        {
            //cancel single tap/low force
            SetToIdle();
            return;
        }
        //launch
        if (playerState == State.AIMING)
        {
            //check if player is aiming toward the platform
            var dotValue = Vector2.Dot(Vector2.up, forceDir.normalized);
            if (dotValue < 0.1)
            {
                SetToFirstBounce();
            }
            else
            {
                playerState = State.LAUNCHED;
            }
            forceDir = Vector2.ClampMagnitude(forceDir, maxForce);
            rb.velocity = forceDir;
            playerAnimation.ToggleTrailRenderer(true);

        }
        else if (playerState == State.BOUNCE)
        {
            if (jumpTimer <= 0f)
            {
                //can jump in mid air 
                RelaunchPlayer();
                jumpTimer = midAirJumpCooldown;
            }
        }
        else if(playerState == State.STICK)
        {
            RelaunchPlayer();
            //rb.isKinematic = false;
            //collider.enabled = true;
            ResetGravity();
            playerAnimation.FlipSprite(forceDir.normalized);
        }
        if (GamePlayScreenUI.instance.BulletTimeActive)
        {
            GamePlayScreenUI.instance.EndBulletTime(bulletTimeAbility);
        }

    }
    private void RightClicked()
    {
        //pound only when in mid air
        if(playerState == State.LAUNCHED)
        {
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
        if (playerState != State.BOUNCE)
            return;
        if(bulletTimeAbility>0)
        {
            bulletTimeAbility--;

            Time.timeScale = bulletTimeScale;
            //to avoid physics lag during SloMo
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            //start a bullet timer 
            GamePlayScreenUI.instance.StartTimer(bulletTimeAbility);
        }
        else
        {
            GamePlayScreenUI.instance.NoBulletTimeAbilityFeedback();
        }
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
        Vector2 offset;
        //dummy orientation
        switch(hitDirection)
        {
            case HitDirection.Left:
                playerSquishDummy.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                playerSquishDummy.GetComponent<SpriteRenderer>().flipY = true;
                offset = new Vector2(-UnityEngine.Random.Range(squishOffset, squishOffset + 1), UnityEngine.Random.Range(-squishOffset, squishOffset));
                SquishEffect.Invoke(offset);
                break;
            case HitDirection.Right:
                playerSquishDummy.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                offset = new Vector2(UnityEngine.Random.Range(squishOffset, squishOffset + 1), UnityEngine.Random.Range(-squishOffset, squishOffset));
                SquishEffect.Invoke(offset);
                break;
            case HitDirection.Up:
                //direction
                playerSquishDummy.transform.rotation = Quaternion.Euler(Vector3.zero);
                playerSquishDummy.GetComponent<SpriteRenderer>().flipY = true;
                //offset
                offset = new Vector2(UnityEngine.Random.Range(-squishOffset, squishOffset), UnityEngine.Random.Range(squishOffset, squishOffset + 1));
                SquishEffect.Invoke(offset);
                break;
            case HitDirection.Down:
                //direction
                playerSquishDummy.transform.rotation = Quaternion.Euler(Vector3.zero);
                //offset
                offset = new Vector2(UnityEngine.Random.Range(-squishOffset, squishOffset), -UnityEngine.Random.Range(squishOffset, squishOffset + 1));
                SquishEffect.Invoke(offset);
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
    public void SetToStickState()
    {
        playerState = State.STICK;
        rb.velocity = Vector2.zero;
        playerAnimation.SetStick();
        playerAnimation.ToggleTrailRenderer(false);
        //disable rb to avoid gravity
        //rb.isKinematic = true;
        //collider.enabled = false;
        Physics2D.gravity = Vector2.zero;
        
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
        bulletTimeAbility = 2;
        GamePlayScreenUI.instance.UpdateBulletTimeUI(bulletTimeAbility);
    }
    public void ResetGravity()
    {
        Physics2D.gravity = Vector2.up * gravity;

    }
    private void OnDisable()
    {
        playerInput.mouseClicked -= LeftClicked;
        playerInput.mouseReleased -= LeftReleased;
        playerInput.mouseDragging -= LeftDragging;
        playerInput.rightClicked -= RightClicked;
        playerInput.QkeyPressed += ActivateBulletTime;
        respawnPlayer += RespawnPlayer;
    }

}
