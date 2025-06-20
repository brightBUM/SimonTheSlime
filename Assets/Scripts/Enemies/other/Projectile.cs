using magar;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 5f;
    public int damage = 1;
    public float lifeTime = 3f;
    public ParticleSystem explosionEffect;

    private Vector2 direction;
    private Rigidbody2D rb;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        //speed = projectileSpeed;
        rb = GetComponent<Rigidbody2D>();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }
 /*   private void OnCollisionEnter2D(Collision2D collision)
    {
            LivingEntity livingEntity = collision.gameObject.GetComponent<LivingEntity>();
            if (livingEntity != null)
            {
                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                livingEntity.TakeDamage(new DamageInfo(damage, hitDirection));
            }
            Destroy(gameObject);
    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
            
            LivingEntity livingEntity = collision.GetComponent<LivingEntity>();
            if (livingEntity != null)
            {
                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                livingEntity.TakeDamage(new DamageInfo(damage, hitDirection));
            }
            Instantiate(explosionEffect,transform.position,Quaternion.identity);

            Destroy(gameObject);
        
    }
}