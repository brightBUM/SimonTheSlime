using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInput;

public class BreakablePT : MonoBehaviour
{
    [SerializeField] GameObject breakablePTVFX;
    [SerializeField] Rigidbody2D[] rigidbodies;
    [SerializeField] GameObject bananaPrefab;
    [SerializeField] Transform blastPoint;
    [SerializeField] SpriteRenderer originalSprite;
    [SerializeField] Color breakColor;
    [SerializeField] float coinForce = 5f;
    [SerializeField] float radius = 5.0F;
    [SerializeField] float power = 10.0F;
    public int HitCount = 0;
    private void Start()
    {

    }
    
    public void OnCollisionPounded()
    {
        if(HitCount>0)
        {
            //spawn coins on hit
            GetComponentInChildren<BouncyDeform>().HitDeform();
            SoundManager.instance.PlayCoinBangSFX();
            var coin = Instantiate(bananaPrefab, transform.position, Quaternion.identity);
            coin.AddComponent<Rigidbody2D>().AddForce(Vector2.up*coinForce,ForceMode2D.Impulse);
            coin.GetComponent<BoxCollider2D>().enabled = false;
            DOVirtual.DelayedCall(0.5f, () =>
            {
                coin.GetComponent<BoxCollider2D>().enabled = true;
            });
            HitCount--;
            Destroy(coin, 2f);
            if(HitCount==0)
            {
                originalSprite.color = breakColor;
            }
            //block hit feedback (squeeze effects)
        }
        else
        {
            SoundManager.instance.PlayBrickBreakSFx();
            var effect = Instantiate(breakablePTVFX, transform.position, Quaternion.identity);
            rigidbodies = effect.GetComponentsInChildren<Rigidbody2D>();
            //breakablePTVFX.SetActive(true);
            ExplodePlatform();
            Destroy(this.gameObject);
        }
        
    }

    void ExplodePlatform()
    {
        foreach (Rigidbody2D rb in rigidbodies)
        {
            if (rb != null)
            {
                // Apply force to rigidbodies within the explosion radius
                Vector2 direction = rb.transform.position - transform.position;
                float distance = direction.magnitude;

                // Calculate the force based on the distance from the explosion center
                float force = (1 - (distance / radius)) * power;

                // Apply the force
                rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

                Destroy(rb.gameObject, Random.Range(3f,4f));
            }
        }
    }
}
