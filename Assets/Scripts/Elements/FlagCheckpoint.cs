using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCheckpoint : MonoBehaviour
{
    [SerializeField] GameObject checkPointUnlockVFX;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform checkpointPos;
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
        
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (unlocked) return;
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            unlocked = true;
            animator.SetTrigger("unlock");
            LevelManager.Instance.LastCheckpointpos = this.transform.position;
            Instantiate(checkPointUnlockVFX, checkpointPos.position, checkPointUnlockVFX.transform.rotation);
            SoundManager.instance.PlayFlagCheckPointSFx();
        }
    }
}
