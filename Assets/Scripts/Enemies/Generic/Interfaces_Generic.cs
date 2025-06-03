using System;
using UnityEngine;
public struct DamageInfo
{
    public float amount;
    public Vector3 dir;
    public  DamageInfo(float amount, Vector3 direction)
    {
        this.amount= amount;    
        this.dir= direction;  
    }
}
public interface IHealth : IDamageable
{
    public event Action<DamageInfo> HealthChangedEvent;
    public event Action<DamageInfo> DieEvent;

    public float Health {  get;  set; }
    public void InitHealth();
    public void Die();
}
public interface IDamageable
{
    public void TakeDamage(DamageInfo info);
}