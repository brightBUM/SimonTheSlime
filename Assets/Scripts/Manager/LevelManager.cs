using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] CameraShake camShake;
    [SerializeField] LevelStart levelStart;
    [SerializeField] Transform collectiblesParent;
    [SerializeField] PlayerController playerController;
    [SerializeField] float targetTime;
    public int levelIndex = 0;
    public float levelTimer;
    public bool startLevelTimer = false;
    //[Header("collectibles")]
    private int targetbananas;
    private int collectedBananas;
    private int levelScore;
    private int collectedGems;
    private BaseRespawn baseRespawn;
    private int stars;
    public int retryCount = 1;
    public Vector3 LastCheckpointpos { get; set; }
    public static LevelManager Instance;
    public CameraShake ShakeCamera => camShake;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        LastCheckpointpos = levelStart.transform.position;
        targetbananas = collectiblesParent.childCount;
        //bangeable pt
        targetbananas += FindObjectsByType<BangablePlatform>(FindObjectsSortMode.None).Length * 4;

        levelTimer = 0f;
        //Application.targetFrameRate = 120;
    }
    private void Start()
    {
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
    private string TimeFormatConversion(float time)
    {
        var seconds = time % 60;
        var minutes = time / 60;
        string timeFormat = Mathf.FloorToInt(minutes) + ":" + Mathf.FloorToInt(seconds);
        return timeFormat;
    }

    public string GetGemsCount()
    {
        return collectedGems.ToString();
    }
    public string GetLevelBananasCount()
    {
        //if(collectedBananas>=targetbananas)
        //    stars++;

        return collectedBananas.ToString() + "/" + targetbananas.ToString();
    }
    public string GetLevelTimerText()
    {
        //if (levelTimer <= targetTime)
        //    stars++;
        return TimeFormatConversion(levelTimer) + "/" + TimeFormatConversion(targetTime);
    }

    public int GetWonStars()
    {
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
        var nextLevelIndex = levelIndex + 1;
        if (!SaveLoadManager.Instance.GetLevelUnlockData(nextLevelIndex))
        {
            SaveLoadManager.Instance.UnlockLevel(nextLevelIndex);

        }
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
