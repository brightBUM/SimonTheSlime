using Sirenix.OdinInspector;
using UnityEngine;

namespace Magar
{

    public class Agent_Reward : OnHit
    {
        public float range=2f;
        public bool rewardOnDamage = false;
        public Transform spawnTransform;

        [ShowIf(nameof(rewardOnDamage), true)]
        public int DamageRewardCount = 1;

        public bool rewardOnDeath = true;
        [ShowIf(nameof(rewardOnDeath), true)]
        public int DeathRewardCount = 2;

        protected override void OnDamage(DamageInfo info)
        {
            if (!rewardOnDamage) return;
            GameObject prefab = Resources.Load<GameObject>("Coin");
            for (int i = 0; i < DamageRewardCount; i++)
            {
                // Generate a random point in a 2D circle
                Vector2 randomCircle = Random.insideUnitCircle * range;
                Vector3 right = spawnTransform.right;
                Vector3 forward = spawnTransform.forward;
                Vector3 randomPos = spawnTransform.position + (right * randomCircle.x) + (forward * randomCircle.y);
                GameObject sp = Instantiate(prefab, randomPos, Quaternion.identity);

            }
        }

        protected override void OnDeath(DamageInfo info)
        {
            if (!rewardOnDeath) return;
            GameObject prefab = Resources.Load<GameObject>("Coin");
            for (int i = 0; i < DeathRewardCount; i++)
            {
                // Generate a random point in a 2D circle
                Vector2 randomCircle = Random.insideUnitCircle * range;
                Vector3 right = spawnTransform.right;
                Vector3 forward = spawnTransform.forward;
                Vector3 randomPos = spawnTransform.position + (right * randomCircle.x) + (forward * randomCircle.y);
                GameObject sp = Instantiate(prefab, randomPos, Quaternion.identity);

            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }

    }
}
