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
        protected virtual void OnEnable() { }
        public virtual void TakeDamage(DamageInfo info)
        {
            if (info.amount > 0)
            {
                health -= info.amount;
                if (health <= 0f)
                {
                    DieEvent?.Invoke(info);
                    Die();
                }
                else if (health > 0)
                {
                    HealthChangedEvent?.Invoke(info);
                }
            }
        }

        [ContextMenu("Self Destruct")]
        public void SelfDestruct()
        {
            TakeDamage(new DamageInfo(startingHealth+1, Vector3.zero));
        }

        [ContextMenu("Self Damage")]
        public void SelfDamage()
        {
            TakeDamage(new DamageInfo(.1f, Vector3.zero));
        }
        public void InitHealth()
        {
            Health = startingHealth;
            health = startingHealth;
        }

        public void Die()
        {
            Destroy(gameObject);
        }
        protected virtual void OnDisable() { }
    }
}