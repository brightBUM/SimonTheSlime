using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] AudioClip levelMusic;
    [SerializeField] CameraShake camShake;
    [Header("collectibles")]
    [SerializeField] int targetbananas;
    private int collectedBananas;
    [SerializeField] Transform lastCheckpoint;
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
