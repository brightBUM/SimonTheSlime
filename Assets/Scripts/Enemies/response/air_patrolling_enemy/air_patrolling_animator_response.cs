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
        agent.GetComponent<AirPatrollingEnemy>().OnShootProjectileEvent += OnShootProjectile;
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
    void OnShootProjectile()
    {
        animator.SetTrigger("alert");

    }

}
