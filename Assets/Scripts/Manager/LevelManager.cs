using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] AudioClip levelMusic;
    [SerializeField] CameraShake camShake;
    [SerializeField] Transform lastCheckpoint;
    [SerializeField] Transform collectiblesParent;
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
        targetbananas = collectiblesParent.childCount+11;
    }
    private void Start()
    {
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

}
