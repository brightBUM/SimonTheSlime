using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magar
{
    public class Agent_AnimatorResponse : OnHit
    {
        public Animator animator;
        protected override void OnDamage(DamageInfo info)
        {
            base.OnDamage(info);
            animator.SetTrigger("hit");
            
        }
        protected override void OnDeath(DamageInfo info)
        {
            base.OnDeath(info);
            animator.SetTrigger("die");


        }

    }
}