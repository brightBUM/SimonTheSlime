using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum State
{
    IDLE,
    AIMING,
    LAUNCHED,
    FIRSTBOUNCE,
    POUND,
    GHOST
}
public class PlayerController : MonoBehaviour
{
    public float Velocity => rb.velocity.y;
    public State playerState;

    private Vector2 startPos;
    private Vector2 dragPos;
    private Vector2 aimDir;
    private Vector2 forceDir;
    private Rigidbody2D rb;
    private PlayerInput playerInput;

    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] GameObject start;
    [SerializeField] GameObject dragger;
    [SerializeField] GameObject aimer;
    [SerializeField] bool debugVectors;
    [SerializeField] float dragSensitivity;
    [SerializeField] float poundForce = 10f;
    [SerializeField] float maxForce;
    [SerializeField] float forceLength;
    [SerializeField] float gravity = -20f;
    [SerializeField] float onHitUpForce = 3f;
    //[SerializeField] float reticleRange;
    Action respawnPlayer;
    CircleCollider2D collider;
    float lerpAmount = 0f;
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
    }
    private void OnEnable()
    {
        playerInput.mouseClicked += LeftClicked;
        playerInput.mouseReleased += LeftReleased;
        playerInput.mouseDragging += LeftDragging;
        playerInput.rightClicked += RightClicked;
        respawnPlayer += RespawnPlayer;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void LeftClicked(Vector2 mousePos)
    {
        if(playerState == State.IDLE)
        {
            aimDir = Vector2.zero;
            playerState = State.AIMING;
            playerAnimation.SetAim();
            playerAnimation.ToggleLineRenderer(true);
            startPos = mousePos;
            if (debugVectors)
            {
                ToggleDebug(true);
                start.transform.position = startPos;
            }
        }
    }
    private void LeftDragging(Vector2 mousePos)
    {
        // calculate aim force and sprite flip direction
        if (playerState == State.AIMING)
        {
            dragPos = mousePos;
            var dir = dragPos - startPos;
            aimDir = (Vector2)transform.position + (-dir);
            forceDir = aimDir - (Vector2)transform.position;
            forceDir *= dragSensitivity;
            forceLength = forceDir.magnitude;
            playerAnimation.FlipSprite(forceDir.normalized);
            playerAnimation.DrawTrajectory(Vector2.ClampMagnitude(forceDir, maxForce));
            //aimer.transform.position = Vector2.ClampMagnitude(aimDir, launchForce);
            if (debugVectors)
            {
                dragger.transform.position = mousePos;
                aimer.transform.position = aimDir;
            }
        }

    }
    private void LeftReleased()
    {
        playerAnimation.ToggleLineRenderer(false);
        if (debugVectors)
        {
            ToggleDebug(false);
        }
        if (forceLength < 0.1)
        {
            SetToIdle();
            return;
        }
        //launch
        if (playerState == State.AIMING)
        {
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

    public void ToggleDebug(bool value)
    {
        start.gameObject.SetActive(value);
        dragger.gameObject.SetActive(value);
        aimer.gameObject.SetActive(value);
    }
    public void SetToFirstBounce()
    {
        playerState = State.FIRSTBOUNCE;
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
    public void ResetPound()
    {
        SetToIdle();
        playerAnimation.ResetTrailEffect();
    }

    public Vector2 GetPosition(Vector2 vel,float t)
    {
        var pos = (Vector2)transform.position + vel * t+0.5f*Physics2D.gravity*t*t;
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
        var mid = A + D / 2;
        var Amid = A + mid / UnityEngine.Random.Range(2, 6);
        var Dmid = D + mid / UnityEngine.Random.Range(2, 6);
        var dir1 = D - Amid;
        var dir2 = D - Dmid;

        B = Amid + Vector2.Perpendicular(dir1.normalized) * UnityEngine.Random.Range(3, 7);
        C = Dmid + (Vector2.Perpendicular(dir2.normalized) * -1f * UnityEngine.Random.Range(3, 7));

        //var distance = Vector2.Distance(A, D);
        //var duration = distance / 3.0f; // camera follow speed = 3

        //Debug.Log(string.Format($"distance : {distance} , duration : {duration}"));

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
            collider.enabled = true;
            
        });

    }


    private void OnDisable()
    {
        playerInput.mouseClicked -= LeftClicked;
        playerInput.mouseReleased -= LeftReleased;
        playerInput.mouseDragging -= LeftDragging;
        playerInput.rightClicked -= RightClicked;
        respawnPlayer += RespawnPlayer;
    }

}
