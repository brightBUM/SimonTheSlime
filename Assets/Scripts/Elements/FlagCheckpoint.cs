using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCheckpoint : MonoBehaviour
{
    [SerializeField] GameObject checkPointUnlockVFX;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Material flashMaterial;
    bool unlocked = false;
    Material originalMaterial;
    // Start is called before the first frame update
    void Start()
    {
        originalMaterial = spriteRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            spriteRenderer.material = flashMaterial;
            StartCoroutine(DelayMaterialSwitch());
        }

    }

    IEnumerator DelayMaterialSwitch()
    {
        yield return new WaitForSeconds(1);
        spriteRenderer.material = originalMaterial;
    }    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (unlocked) return;
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            unlocked = true;
            animator.SetTrigger("unlock");
            LevelManager.Instance.LastCheckpointpos = this.transform.position;
        }
    }
}
