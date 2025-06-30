using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] CameraShake camShake;
    [SerializeField] LevelStart levelStart;
    [SerializeField] Transform collectiblesParent;
    [SerializeField] PlayerController playerController;
    [SerializeField] ComboUI ComboUIPrefab;
    public float targetTime = 45f;
    public int levelIndex = 0;
    public float levelTimer;
    public bool startLevelTimer = false;
    //[Header("collectibles")]
    [HideInInspector] public int targetbananas;
    [HideInInspector] public int collectedBananas;
    private int levelScore;
    private int collectedGems;
    private BaseRespawn baseRespawn;
    public int retryCount = 1;
    public int adRespawnCount = 0;
    public int TotalRespawnCount => retryCount + adRespawnCount;
    public int comboCount;
    public Vector3 LastCheckpointpos { get; set; }
    public static LevelManager Instance;
    public CameraShake ShakeCamera => camShake;
    ComboUI ComboParent;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        LastCheckpointpos = levelStart.transform.position;
       
        levelTimer = 0f;

        levelIndex = GameManger.Instance.selectedIndex - 3;
        //Application.targetFrameRate = 120;
    }
    private void Start()
    {
        comboCount = 0;
        ComboParent = Instantiate(ComboUIPrefab);
        ComboParent.transform.localScale = Vector3.zero;
        //Debug.Log("combo count : " + comboCount);
        GameManger.Instance?.ToggleMenuMusic(false);
    }
    private void Update()
    {
        if (startLevelTimer)
        {
            levelTimer += Time.deltaTime;

            GamePlayScreenUI.Instance.UpdateTimerText(TimeFormatConversion(levelTimer));
        }
    }
    public void UpdateTargetBananas(int count)
    {
        targetbananas+=count;
        GamePlayScreenUI.Instance.UpdateBananaCount(GetLevelBananasCount());
    }
    public void StartLevel()
    {
        startLevelTimer = true;
        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
        }
    }
    public int CalculateLevelScore()
    {
        return ((collectedBananas * 5) + (int)levelTimer);
    }
    public string TimeFormatConversion(float time)
    {
        var seconds = time % 60;
        var minutes = time / 60;
        string timeFormat = Mathf.FloorToInt(minutes) + ":" + Mathf.FloorToInt(seconds);
        return timeFormat;
    }
    public string GetPerfectJumpBonus()
    {
        var baseValue = GameManger.Instance.gameConfig.perfectJumpBase;
        var perfectJumpBonus = comboCount*baseValue;
        SaveLoadManager.Instance.playerProfile.nanas += perfectJumpBonus;
        return $"{baseValue}x{comboCount} = {perfectJumpBonus} nanas";
    }
    public string GetGemsCount()
    {
        return collectedGems.ToString();
    }
    public string GetLevelBananasCount()
    {
        return collectedBananas.ToString() + "/" + targetbananas.ToString();
    }
    public string GetLevelTimerText()
    {
        
        return TimeFormatConversion(levelTimer) + "/" + TimeFormatConversion(targetTime);
    }

    public int GetWonStars()
    {
        int stars = 0;

        if (collectedBananas >= targetbananas)
        {
            stars++;
            //Debug.Log("nanas star awarded");
        }

        if (levelTimer <= targetTime)
        {
            stars++;
            //Debug.Log("target time star awarded");
        }

        if (retryCount + adRespawnCount <= 1)
        {
            //Debug.Log("No respawn star awarded ");
            stars++;
        }

        //Debug.Log(" star awarded : "+stars);

        return stars;
    }
    
    public void CollectBanana()
    {
        collectedBananas++;
        GamePlayScreenUI.Instance.UpdateBananaCount(GetLevelBananasCount());
    }
    public void AddNanasToProfile()
    {
        SaveLoadManager.Instance.playerProfile.nanas += collectedBananas;
        SaveLoadManager.Instance.SaveGame();
    }
    public void PlayerInputToggle(bool value)
    {
        if (playerController != null)
        {
            //setting to pound state while block player input 
            //playerController.playerState = !value ? State.POUND : State.IDLE;
        }
    }
    public void UnlockNextLevel()
    {
        SaveLoadManager.Instance.UnlockLevel();
    }
    public void TriggerPlayerRespawn()
    {
        playerController.DelayedRespawn(0);
    }
    public void SetRespawn(BaseRespawn baseRespawn)
    {
        this.baseRespawn = baseRespawn;
        LastCheckpointpos = this.baseRespawn.CheckPointPosition();
    }
    public void LastCheckPointEffect()
    {
        this.baseRespawn.RespawnEffect();
    }
    public void ComboAchieved()
    {
        comboCount++;
        ComboParent.TweenCombo(playerController.transform.position, comboCount);
    }
    
    public void InvokeLevelCompleteAnalytics()
    {
        startLevelTimer = false;
        FirebaseAnalyticsManager.Instance.LogEvent("Time taken to complete Level", new Dictionary<string, object>
    {
        { "screen", "GAME" },
        { "level", levelIndex+1},
        { "time_taken", levelTimer }
    });
    }

    public void BananaRespawn()
    {
        TriggerPlayerRespawn();
        retryCount++;
        FirebaseAnalyticsManager.Instance.LogEvent("No of Retries in Level", new Dictionary<string, object>
    {
        { "screen", "GAME"},
        { "level", levelIndex+1},
    });

        FirebaseAnalyticsManager.Instance.LogEvent("No of time bananas is clicked for Extra life", new Dictionary<string, object>
    {
        { "screen", "GAME" },
        { "level", levelIndex+1}
    });

    }
}
