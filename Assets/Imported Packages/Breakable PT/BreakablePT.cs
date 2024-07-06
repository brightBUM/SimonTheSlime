using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInput;

public class BreakablePT : MonoBehaviour
{
    [SerializeField] GameObject breakablePTVFX;
    [SerializeField] Rigidbody2D[] rigidbodies;
    [SerializeField] Transform blastPoint;
    [SerializeField] float radius = 5.0F;
    [SerializeField] float power = 10.0F;
    private void Start()
    {
    }
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if(other.collider.TryGetComponent<PlayerInput>(out PlayerInput playerInput))
    //    {
    //        Debug.Log("state  - "+ playerInput.playerState);
    //        if (playerInput.playerState == PlayerInput.PlayerState.POUND)
    //        {
                
    //            //Destroy(this.gameObject);
    //            //var rb = GetComponent<Rigidbody2D>();
    //            //rb.bodyType = RigidbodyType2D.Dynamic;
    //            //rb.gravityScale = 1.0f;
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("no playerInput component");
    //    }
    //}
    public void OnCollisionPounded()
    {
        //SoundManager.instance.PlayBrickBreakSFx();
        var effect = Instantiate(breakablePTVFX, transform.position, Quaternion.identity);
        rigidbodies = effect.GetComponentsInChildren<Rigidbody2D>();
        //breakablePTVFX.SetActive(true);
        ExplodePlatform();
        Destroy(this.gameObject);
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
