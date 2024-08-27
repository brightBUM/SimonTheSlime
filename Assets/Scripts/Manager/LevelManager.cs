using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] AudioClip levelMusic;
    [SerializeField] CameraShake camShake;
    [SerializeField] Transform lastCheckpoint;
    [SerializeField] Transform collectiblesParent;
    [SerializeField] PlayerController playerController;
    //[Header("collectibles")]
    private int targetbananas;
    private int collectedBananas;
    public Vector3 LastCheckpointpos { get; set; }
    public static LevelManager Instance;
    public CameraShake ShakeCamera => camShake;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        LastCheckpointpos = lastCheckpoint.position;
        targetbananas = collectiblesParent.childCount;
    }
    private void Start()
    {
    }
    public void BangablePlatformSpawn()
    {
        targetbananas += 4;
    }
    public string GetLevelBananasCount()
    {
        return collectedBananas.ToString() + "/" + targetbananas.ToString();
    }
    public void CollectBanana()
    {
        collectedBananas++;
        GamePlayScreenUI.instance.UpdateBananaCount(GetLevelBananasCount());
    }

    public void PlayerInputToggle(bool value)
    {
        if(playerController!=null)
        {
            //setting to pound state while block player input 
            playerController.playerState = !value ? State.POUND : State.IDLE;
        }
    }
}
