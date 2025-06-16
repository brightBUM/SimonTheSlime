using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using magar;
using Magar;
[RequireComponent(typeof(HorizontalMovingEnemy))]
public class HorizontalMovingEnemy_AnimatorResponse : Agent_AnimatorResponse
{

    protected override void Start()
    {
        base.Start();
        agent.GetComponent<HorizontalMovingEnemy>().OnMovePointReachedEvent += OnPointReached;
    }

    void OnPointReached()
    {
        //animator
        animator.SetTrigger("pointreached");

    }

    void OnBlastInitiated()
    {
        animator.SetTrigger("blast");
    }

}
