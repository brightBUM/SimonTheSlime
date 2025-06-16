using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using magar;
using Magar;
public class HorizontalPatroller_AnimatorResponse : Agent_AnimatorResponse
{

    protected override void Start()
    {
        base.Start();
        agent.GetComponent<HorizontalPatrollingEnemy>().OnMovePointReachedEvent += OnPointReached;
        agent.GetComponent<HorizontalPatrollingEnemy>().OnPlayerFoundEvent += OnEnemyFound;
        agent.GetComponent<HorizontalPatrollingEnemy>().OnBlastInitiatedEvent += OnBlastInitiated;
    }

    void OnPointReached()
    {
        //animator
        animator.SetTrigger("pointreached");

    }
    void OnEnemyFound()
    {
        animator.SetTrigger("alert");
    }

    void OnBlastInitiated()
    {
        animator.SetTrigger("blast");
    }

}
