using DG.Tweening;
using UnityEngine;
using static PlayerInput;

public class BreakablePlatform : MonoBehaviour,IPoundable
{
    [SerializeField] GameObject breakablePTVFX;
    [SerializeField] Rigidbody2D[] rigidbodies;
    [SerializeField] Transform blastPoint;
    [SerializeField] float radius = 5.0F;
    [SerializeField] float power = 10.0F;
    [SerializeField] SpriteRenderer spriteRenderer;
    
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

    public void OnPlayerPounded(System.Action<IPoundable> ContinuePound)
    {
        SoundManager.instance.PlayBrickBreakSFx();
        var effect = Instantiate(breakablePTVFX, transform.position, Quaternion.identity);
        rigidbodies = effect.GetComponentsInChildren<Rigidbody2D>();
        //breakablePTVFX.SetActive(true);
        ExplodePlatform();

        ContinuePound(this);

        Destroy(this.gameObject);
    }
    public void SetSize(Vector2 spriteSize)
    {
        spriteRenderer.size = spriteSize;
    }
}
