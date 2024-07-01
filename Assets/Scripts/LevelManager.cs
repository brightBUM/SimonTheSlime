using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] AudioClip levelMusic;
    Transform lastCheckpoint;
    public Vector3 LastCheckpointpos { get; set; }
    public static LevelManager Instance;
    private void Awake()
    {
        Instance = this;
    }


}
