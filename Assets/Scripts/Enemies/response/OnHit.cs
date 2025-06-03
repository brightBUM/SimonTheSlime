using magar;
using UnityEngine;
public class OnHit : MonoBehaviour
{
    protected LivingEntity agent;
    protected virtual void Start()
    {
        if(agent==null)
            agent=GetComponent<LivingEntity>();    
        
        agent.HealthChangedEvent += OnDamage;
        agent.DieEvent += OnDeath;
    }

    protected virtual void OnDamage(DamageInfo info)
    {
    }
    protected virtual void OnDeath(DamageInfo info)
    {
    }
    protected virtual void OnTeleport()
    {

    }
    protected bool CanResponse(DamageInfo info)
    {
        /*if (NonInteractableWeaponTypes.Count <= 0) return true;
        if (NonInteractableWeaponTypes.Contains(info.type))
            return false;*/
        return true;
    }
}
