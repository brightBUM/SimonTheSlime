using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    IDLE,
    AIMING,
    LAUNCHED,
    POUND
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerAnimation playerAnimation;
    public State playerState;
    private Vector2 startPos;
    private Vector2 dragPos;
    private Vector2 aimDir;

    [SerializeField] GameObject start;
    [SerializeField] GameObject dragger;
    [SerializeField] GameObject aimer;
    [SerializeField] float launchForce;
    [SerializeField] float reticleRange;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        playerState = State.AIMING;
        playerAnimation.SetAim();
        startPos = mousePos;
        start.transform.position = startPos;
    }
    
    private void LeftDragging(Vector2 mousePos)
    {
        // calculate aim force and sprite flip direction
        dragPos = mousePos;
        dragger.transform.position = mousePos;
        var dir = dragPos - startPos;
        aimDir = (Vector2)transform.position + (-dir);
        aimer.transform.position = Vector2.ClampMagnitude(aimDir, launchForce);
    }
    private void LeftReleased()
    {
        //launch
        playerState = State.LAUNCHED;
        playerAnimation.SetRoll();
        var forceDir = aimer.transform.position - transform.position;
        //rb.AddForce(forceDir, ForceMode2D.Impulse);
        rb.velocity = forceDir;
    }
    private void RightClicked()
    {
        //pound only when in mid air

        //Debug.Log("right mouse clicked");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("collided with : " + collision.gameObject.name);
        //if(playerState == State.LAUNCHED)
        //{
        //    rb.velocity = Vector2.zero;
        //    rb.rotation = 0f;
        //    transform.rotation = Quaternion.Euler(Vector3.zero);
        //    playerAnimation.SetIdle();
        //    playerState = State.IDLE;
        //}
       
    }
    private void OnDisable()
    {
        playerInput.mouseClicked -= LeftClicked;
        playerInput.mouseReleased -= LeftReleased;
        playerInput.mouseDragging -= LeftDragging;
        playerInput.rightClicked -= RightClicked;
    }
    
}
