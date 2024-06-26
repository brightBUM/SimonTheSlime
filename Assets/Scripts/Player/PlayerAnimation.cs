using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
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
        
    }

    public void SetAim()
    {
        animator.SetTrigger("aim");
    }
    public void SetRoll()
    {
        animator.SetTrigger("roll");
    }
    public void SetIdle()
    {
        animator.SetTrigger("idle");
    }
}
