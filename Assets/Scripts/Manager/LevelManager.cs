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
    [SerializeField] int maxLaunches;
    public int levelIndex = 0;
    private int currentLaunches;
    private float levelTimer;
    //[Header("collectibles")]
    private int targetbananas;
    private int collectedBananas;
    private int stars;
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
        levelTimer += Time.deltaTime;
        var seconds = levelTimer % 60;
        var minutes = levelTimer / 60;
        string time = Mathf.FloorToInt(minutes) + ":" + Mathf.FloorToInt(seconds);
        GamePlayScreenUI.instance.UpdateTimerText(time);
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
        return levelTimer.ToString() + "/" + targetTime.ToString();
    }
    public string GetLevelLaunches()
    {
        if (currentLaunches <= maxLaunches)
            stars++;
        return currentLaunches.ToString() + "/" + maxLaunches.ToString();
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
