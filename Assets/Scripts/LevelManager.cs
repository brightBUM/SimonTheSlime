using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] AudioClip levelMusic;
    [SerializeField] CameraController cameraController;
    Transform lastCheckpoint;
    public Vector3 LastCheckpointpos { get; set; }
    public static LevelManager Instance;
    public CameraController LevelCamera => cameraController;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        
    }


}
