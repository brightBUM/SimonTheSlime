using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBarrel : MonoBehaviour
{
    [SerializeField] Material flashMaterial;
    [SerializeField] Sprite damagedSprite;
    [SerializeField] float explodeForce = 20f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject smokeVFX;
    [SerializeField] ParticleSystem explosionVFX;
    Material originalMaterial;
    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = spriteRenderer.material;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            //flash material
            SoundManager.Instance.PlayExplosionSFX();
            LevelManager.Instance.ShakeCamera.OnExplosion();
            explosionVFX.Play();
            spriteRenderer.material = flashMaterial;
            DOVirtual.DelayedCall(0.5f, () =>
            {
                spriteRenderer.material = originalMaterial;
            });
            //add rb force
            playerController.ExplodeOnContact(explodeForce);
            //cam shake 
            LevelManager.Instance.ShakeCamera.OnExplosion();
            //particles , SFX
            smokeVFX.SetActive(true);
            smokeVFX.GetComponent<Animator>().SetInteger("smoke", Random.Range(1, 3));
            //replace with damaged barrel
            spriteRenderer.sprite = damagedSprite;
            //disable collider
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
