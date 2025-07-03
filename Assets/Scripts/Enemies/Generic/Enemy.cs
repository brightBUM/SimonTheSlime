using UnityEngine;
using magar;
using System.Collections;
namespace magar
{
    public class Enemy : LivingEntity
    {
        [Header("Enemy")]
        public Animator animator;
        public int lootIndex;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
            {
                if (playerController.playerState == State.POUND || playerController.playerState == State.DASH)
                {
                    // destroy self
                    TakeDamage(new DamageInfo(1, Vector3.down));

                    //spawn loot
                    LevelManager.Instance.OnEnemyLootDrop(lootIndex,transform.position);
                }
                else
                {
                    //destroy player
                    playerController.Die();
                }
            }

        }

        
    }
}
