using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagCheckpoint : BaseRespawn
{
    [SerializeField] GameObject checkPointUnlockVFX;
    [SerializeField] GameObject unlockedPod;
    [SerializeField] GameObject defaultPod;
    [SerializeField] Transform checkpointPos;
    bool unlocked = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (unlocked) return;
        if(collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            unlocked = true;
            LevelManager.Instance.SetRespawn(this);
            defaultPod.SetActive(false);
            unlockedPod.SetActive(true);
            Instantiate(checkPointUnlockVFX, checkpointPos.position, checkPointUnlockVFX.transform.rotation);
            SoundManager.Instance.PlayFlagCheckPointSFx();
        }
    }

}
