using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using magar;
public class PlayerHealth : LivingEntity
{
    PlayerController controller;
    protected override void Start()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
    }
    public override void TakeDamage(DamageInfo info)
    {
        Debug.Log("player health take damage");

        if (info.amount > 0)
        {
            Debug.Log($"player health die {health} , {info.amount}");
            health -= info.amount;
            Debug.Log($"player health die {health} , {info.amount}");
            if (health <= 0f)
            {
                controller.Die();
            }
            
        }

    }

    
}
