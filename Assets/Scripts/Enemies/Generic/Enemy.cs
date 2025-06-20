using UnityEngine;
using magar;
namespace magar
{
    public class Enemy : LivingEntity
    {
        [Header("Enemy")]
        public Animator animator;
        [SerializeField] LootDrop lootDropPrefab;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
            {
                if (playerController.playerState == State.POUND || playerController.playerState == State.DASH)
                {
                    //spawn loot
                    var lootDropItem = Instantiate(lootDropPrefab,transform.position,Quaternion.identity);
                    lootDropItem.Init();
                    // destroy self
                    TakeDamage(new DamageInfo(1, Vector3.down));

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
