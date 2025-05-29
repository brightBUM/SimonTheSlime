using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using magar;
using Magar;
public class air_patrolling_animator_response : Agent_AnimatorResponse
{

    protected override void Start()
    {
        base.Start();
        agent.GetComponent<AirPatrollingEnemy>().OnPatrolPointReachedEvent += OnPointReached;
        agent.GetComponent<AirPatrollingEnemy>().OnPlayerFoundEvent += OnEnemyFound;
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

}
