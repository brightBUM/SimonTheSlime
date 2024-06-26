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
    POUND
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
    [SerializeField] float maxForce;
    [SerializeField] float forceLength;
    [SerializeField] float gravity = -20f;
    //[SerializeField] float reticleRange;

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
    }
    private void OnEnable()
    {
        playerInput.mouseClicked += LeftClicked;
        playerInput.mouseReleased += LeftReleased;
        playerInput.mouseDragging += LeftDragging;
        playerInput.rightClicked += RightClicked;
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

        }
        
    }
    private void RightClicked()
    {
        //pound only when in mid air

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
    }
    private void OnDisable()
    {
        playerInput.mouseClicked -= LeftClicked;
        playerInput.mouseReleased -= LeftReleased;
        playerInput.mouseDragging -= LeftDragging;
        playerInput.rightClicked -= RightClicked;
    }
    
}
