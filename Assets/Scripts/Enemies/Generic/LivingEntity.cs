using System;
using UnityEngine;
namespace magar
{
    public abstract class LivingEntity : MonoBehaviour, IHealth
    {
        private float health;
        public float startingHealth;

        public float Health
        {
            get
            {
                return health;
            }
            set
            {
                if (value > 0)
                {
                    health = value;
                }
            }
        }

        public event Action<DamageInfo> HealthChangedEvent;
        public event Action<DamageInfo> DieEvent;


        protected virtual void Start()
        {
            InitHealth();
        }

        public virtual void TakeDamage(DamageInfo info)
        {
            if (info.amount > 0)
            {
                Health -= info.amount;
                if (Health <= 0f)
                {
                    DieEvent?.Invoke(info);
                    Die();
                }
                else if (Health > 0)
                {
                    HealthChangedEvent?.Invoke(info);
                }
            }
        }

        [ContextMenu("Self Destruc")]
        public void SelfDestruct()
        {
            Die();
        }

        public void InitHealth()
        {
            Health = health;
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}