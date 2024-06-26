using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    Animator animator;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.playerState == State.LAUNCHED)
        {
            animator.SetFloat("velocity", playerController.Velocity);
        }
    }
    public void FlipSprite(Vector2 aimDirection)
    {
        spriteRenderer.flipX = Vector2.Dot(Vector2.right, aimDirection) < 0 ? true : false;
    }
    public void SetAim()
    {
        //animator.ResetTrigger("idle");
        animator.SetTrigger("aim");
    }
    public void SetRoll()
    {
        animator.SetTrigger("roll");
    }
    public void ResetAim()
    {
        animator.ResetTrigger("aim");
    }
    public void SetIdle()
    {
        //animator.ResetTrigger("roll");
        animator.SetTrigger("idle");
    }
}
