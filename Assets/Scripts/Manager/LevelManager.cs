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
    [SerializeField] float targetTime;
    [SerializeField] int maxLaunchTries = 100;
    public int levelIndex = 0;
    private int currentLaunches;
    private float levelTimer;
    //[Header("collectibles")]
    private int targetbananas;
    private int collectedBananas;
    private int stars;
    public bool startLevelTimer = false;
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
        levelTimer = 0f;

    }
    private void Start()
    {
    }
    private void Update()
    {
        if(startLevelTimer)
        {
            levelTimer += Time.deltaTime;

            GamePlayScreenUI.instance.UpdateTimerText(TimeFormatConversion(levelTimer));
        }
        
    }
    private string TimeFormatConversion(float time)
    {
        var seconds = time % 60;
        var minutes = time / 60;
        string timeFormat = Mathf.FloorToInt(minutes) + ":" + Mathf.FloorToInt(seconds);
        return timeFormat;
    }
    public void BangablePlatformSpawn()
    {
        targetbananas += 4;
    }
    public string GetLevelBananasCount()
    {
        if(collectedBananas>=targetbananas)
            stars++;
        return collectedBananas.ToString() + "/" + targetbananas.ToString();
    }
    public string GetLevelTimerText()
    {
        if (levelTimer <= targetTime)
            stars++;
        return TimeFormatConversion(levelTimer) + "/" + TimeFormatConversion(targetTime);
    }
    public string GetLevelLaunches()
    {
        if (currentLaunches <= maxLaunchTries)
            stars++;
        return currentLaunches.ToString() + "/" + maxLaunchTries.ToString();
    }
    public int GetWonStars()
    {
        return stars;
    }
    public void CollectBanana()
    {
        collectedBananas++;
        GamePlayScreenUI.instance.UpdateBananaCount(GetLevelBananasCount());
    }
    public void IncrementLaunches()
    {
        currentLaunches++;
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
