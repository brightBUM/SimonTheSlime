using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.LevelPlay;

public class GamePlayScreenUI : MonoBehaviour
{
    [Header("GamePlayScreen")]
    [SerializeField] Image dashFillImage;
    [SerializeField] Image bulletTimeIcon;
    [SerializeField] TextMeshProUGUI bulletTimeText;
    [SerializeField] TextMeshProUGUI levelTimerText;
    [SerializeField] TextMeshProUGUI bananaUI;
    [SerializeField] Image timerFillUI;
    [SerializeField] Transform greenWheelUI;
    [SerializeField] GameObject aimReticleObject;
    [SerializeField] Button dashButton;
    [SerializeField] Button grappleButton;
    [SerializeField] Image iconImage;
    [SerializeField] Color timeOverColor;
    [SerializeField] float duration = 0.5f;

    [Header("Level Complete")]
    [SerializeField] TextMeshProUGUI scoreboardTitleUI;
    [SerializeField] TextMeshProUGUI bananasLevelCompleteUI;
    [SerializeField] TextMeshProUGUI gemsUI;
    [SerializeField] GameObject nextLevelButton;
    [SerializeField] float levelCompleteTextDelay = 0.2f;
    [SerializeField] float scoreCountTime = 2f;
    private List<TextMeshProUGUI> levelCompleteTexts;
    [SerializeField] List<GameObject> starItem;
    [Header("Retry")]
    [SerializeField] Button bananaRespawnButton;
    [SerializeField] TextMeshProUGUI nanasCost;

    [Header("Panel")]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameplayScreen;
    [SerializeField] GameObject ScoreboardScreen;
    [SerializeField] GameObject retryScreen;

    Color defaultColor;
    public static GamePlayScreenUI Instance;
    public Action<float> UpdateMidAirJumpUI;
    public Action poundAbilityAction;
    public Action poundReleaseAction;
    public Action dashButtonAction;
    public Action grappleButtonAction;
    private TweenerCore<float, float, FloatOptions> tween;
    public bool BulletTimeActive => timerFillUI.fillAmount < 1f;
    private void OnEnable()
    {
        UpdateMidAirJumpUI += UpdateDashAbilityUI;
        bananaRespawnButton.onClick.AddListener(RespawnViaBananas);
        ScaleTexts();

        dashButton.onClick.AddListener(DashViaButton);
        grappleButton.onClick.AddListener(GrappleViaButton);
        LootDrop.OnCollection += TweenCollection;
    }

    private void TweenCollection(Sprite sprite, Vector3 vector)
    {
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = sprite;
        iconImage.preserveAspect = true;

        var screenpos = Camera.main.WorldToScreenPoint(vector);
        iconImage.transform.position = screenpos;
        iconImage.transform.DOMove(bananaUI.transform.position, 1f).OnComplete(() =>
        {
            iconImage.gameObject.SetActive(false);
        });
    }

    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        UpdateDashAbilityUI(0f);
        UpdateBananaCount(LevelManager.Instance.GetLevelBananasCount());
        defaultColor = timerFillUI.color;
        ScoreboardScreen.transform.localScale = Vector3.zero;
        Time.timeScale = 1f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void ScaleTexts()
    {
        //levelCompleteTexts = new List<TextMeshProUGUI>()
        //{
        //    bananasLevelCompleteUI,
        //    levelTimerCompleteUI,
        //    gemsUI,
        //    levelScoreUI,
        //};

        //foreach (var text in levelCompleteTexts)
        //{
        //    text.transform.localScale = Vector3.zero;
        //}
    }
    public void TogglePauseMenu()
    {
        if (ScoreboardScreen.activeInHierarchy)
            return;

        GameManger.Instance.TogglePauseGame();

        if (GameManger.Instance.IsPaused)
        {
            gameplayScreen.SetActive(false);
            pauseScreen.SetActive(true);
            pauseScreen.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce).SetUpdate(true);
        }
        else
        {
            pauseScreen.transform.localScale = Vector3.zero;
            pauseScreen.SetActive(false);
            gameplayScreen.SetActive(true);
        }
    }
    
    //public void TriggerLevelCompleteScoreboard(bool value)
    //{
    //    if (value)
    //    {
    //        scoreboardTitleUI.text = "Level Complete";
    //        ScoreboardScreen.SetActive(value);
    //        ScoreboardScreen.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
    //        LevelManager.Instance.AddLevelStatsToProfile();
    //    }
    //    else
    //    {
    //        ScoreboardScreen.transform.localScale = Vector3.zero;
    //        ScoreboardScreen.SetActive(false);
    //    }
    //}

    public void TriggerLevelFailedScoreboard()
    {
        //triggered with next button


#if UNITY_ANDROID && !UNITY_EDITOR //check interstitial ad condition
        
        SaveLoadManager.Instance.playerProfile.interStitialAdCount++;

        if (SaveLoadManager.Instance.CheckInterstitialAdCondition())
        {
            IronSourceAdManager.Instance.ShowInterstitialAd();
            IronSourceAdManager.Instance.interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
            IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed += InterstitialAd_OnAdDisplayFailed;

            return;
        }

#endif
        LevelFailedLeaderBoard();

    }
    private void LevelFailedLeaderBoard()
    {
        GameManger.Instance.TogglePauseGame(false);
        LevelManager.Instance.startLevelTimer = false;
        retryScreen.SetActive(false);
        scoreboardTitleUI.text = "Level Failed";
        nextLevelButton.SetActive(false);
        ScoreboardScreen.SetActive(true);
        ScoreboardScreen.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
        UpdateLevelCompleteUI();

        //LevelManager.Instance.AddLevelStatsToProfile();
        //SaveLoadManager.Instance.SaveGame();
    }
    private void InterstitialAd_OnAdDisplayFailed(com.unity3d.mediation.LevelPlayAdDisplayInfoError obj)
    {
        //incase ad load fails , continue with level complete
        Debug.Log("level end interstitial ad display failed");
        LevelFailedLeaderBoard();

        IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed -= InterstitialAd_OnAdDisplayFailed;
    }
    private void InterstitialOnAdClosedEvent(LevelPlayAdInfo info)
    {
        SaveLoadManager.Instance.playerProfile.interStitialAdCount = 0;

        LevelFailedLeaderBoard();

        IronSourceAdManager.Instance.LoadInterstitialAd();
        IronSourceAdManager.Instance.interstitialAd.OnAdClosed -= InterstitialOnAdClosedEvent;
    }

    public void ShowRetryScreen()
    {
        Debug.Log("show retry");
        gameplayScreen.SetActive(false);

        GameManger.Instance.TogglePauseGame(true);
        
        if(SaveLoadManager.Instance.playerProfile.nanas <= CostToRespawn())
        {
            bananaRespawnButton.interactable = false;
        }
       
        nanasCost.text = LevelManager.Instance.retryCount > 3 ? " " : CostToRespawn().ToString() + " Nanas";

        retryScreen.transform.localScale = Vector3.zero;
        retryScreen.SetActive(true);
        retryScreen.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce).SetUpdate(true);
    }
    private int CostToRespawn()
    {
        return GameManger.Instance.gameConfig.RetryNanasCost * LevelManager.Instance.retryCount;
    }
    public void RespawnViaBananas()
    {
        //current cost to respawn
        var cost = CostToRespawn();

        //decrement cost from player profile & respawn

        if(SaveLoadManager.Instance.playerProfile.nanas>= cost)
        {
            SaveLoadManager.Instance.playerProfile.nanas -= cost;
            retryScreen.SetActive(false);
            gameplayScreen.SetActive(true);
            GameManger.Instance.TogglePauseGame(false);
            LevelManager.Instance.BananaRespawn();
            
        }

        //check for retryCount
        if (LevelManager.Instance.retryCount > 3)
        {
            //disable banana retry button interactable 
            bananaRespawnButton.interactable = false;
        }
        SaveLoadManager.Instance.SaveGame();
    }
    public void RespawnViaAd()
    {
#if UNITY_EDITOR
        //allow free respawn in editor , bcoz no test ads
        retryScreen.SetActive(false);
        gameplayScreen.SetActive(true);
        GameManger.Instance.TogglePauseGame(false);
        LevelManager.Instance.TriggerPlayerRespawn();
        LevelManager.Instance.adRespawnCount++;
#elif UNITY_ANDROID
        //trigger rewarded ad  here
        IronSourceAdManager.Instance.ShowRewardedAd();
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
#endif

    }

    private void RewardedVideoOnAdClosedEvent(IronSourceAdInfo info)
    {
        retryScreen.SetActive(false);
        gameplayScreen.SetActive(true);
        GameManger.Instance.TogglePauseGame(false);
        LevelManager.Instance.TriggerPlayerRespawn();
        LevelManager.Instance.adRespawnCount++;

        FirebaseAnalyticsManager.Instance.LogEvent("No. of Retries in Level", new Dictionary<string, object>
    {
        { "screen", "GAME" },
        {"level", LevelManager.Instance.levelIndex+1 }

    });

        FirebaseAnalyticsManager.Instance.LogEvent("No of times Watch Ad is clicked for Extra life", new Dictionary<string, object>
    {
        { "screen", "GAME"},
        {"level", LevelManager.Instance.levelIndex+1 }
    });

        IronSourceRewardedVideoEvents.onAdClosedEvent -= RewardedVideoOnAdClosedEvent;

    }

    private void UpdateDashAbilityUI(float value)
    {
        dashFillImage.fillAmount = value;
    }
    public void UpdateBulletTimeUI(int num)
    {
        bulletTimeText.text = num.ToString();

        float alpha = 1f;
        alpha = num > 0 ? 1f : 0.2f;
        bulletTimeIcon.color = new Color(bulletTimeIcon.color.r, bulletTimeIcon.color.g, bulletTimeIcon.color.b,alpha);
    }
    public void NoBulletTimeAbilityFeedback()
    {
        bulletTimeIcon.rectTransform.DOShakeAnchorPos(0.3f,20);
        SoundManager.Instance.PlayOutofBulletTimeSFX();
    }
    
    public void UpdateBananaCount(string text)
    {
        bananaUI.text = text;
    }
    public void UpdateTimerText(string time)
    {
        levelTimerText.text = time;
    }
    [ContextMenu("LevelComplete")]
    public void ShowLevelCompleteScreen()
    {
        gameplayScreen.SetActive(false);

        //TriggerLevelCompleteScoreboard(true);
        //UpdateLevelCompleteUI();

        //add level stats to profile
        LevelManager.Instance.AddLevelStatsToProfile();

        FindAnyObjectByType<ScoreBoard>().TriggerScoreBoard();

        //check if first play or replay
        SaveLoadManager.Instance.FirstOrReplay(LevelManager.Instance.GetWonStars());

        //unlock next level , if atleast 1 star acquired
        LevelManager.Instance.UnlockNextLevel();

        SaveLoadManager.Instance.SaveGame();
    }
    public void UpdateLevelCompleteUI()
    {

        LevelManager levelManager = LevelManager.Instance;

        bananasLevelCompleteUI.text = levelManager.GetLevelBananasCount();
        
        gemsUI.text = levelManager.GetMelonsCount();
        
        //for(int i = 0;i<levelCompleteTexts.Count-1;i++)
        //{
        //    StartCoroutine(DelayedStarScale(levelCompleteTextDelay + (levelCompleteTextDelay * i), levelCompleteTexts[i].gameObject.transform));
        //}


        //DOVirtual.DelayedCall(0.2f, () =>
        //{
        //    levelScoreUI.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBounce).OnComplete(() =>
        //    {
        //        int scoreValue = 0;
        //        levelScoreUI.text = scoreValue.ToString();

        //        int target = levelManager.CalculateLevelScore();
        //        DOTween.To(() => scoreValue, x => scoreValue = x, target, scoreCountTime).SetUpdate(true).OnUpdate(() =>
        //        {
        //            levelScoreUI.text = scoreValue.ToString();

        //        });
        //    });

        //});
    }
    
    private void StarSystem()
    {
        //spawn stars and store if best score achieved      
        var currentStars = LevelManager.Instance.GetWonStars();

        for (int i = 0; i < currentStars; i++)
        {
            starItem[i].SetActive(true);
            StartCoroutine(DelayedStarScale(0.2f + (0.2f * i), starItem[i].gameObject.transform));
        }

        if (SaveLoadManager.Instance == null)
            return;

        //check if scored stars are more than saved level stars
        SaveLoadManager.Instance.FirstOrReplay(LevelManager.Instance.GetWonStars());
        
    }
    IEnumerator DelayedStarScale(float delayTime,Transform transform)
    {
        yield return new WaitForSeconds(delayTime);

        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
    }
    public void StartTimer(int value,Action timerComplete)
    {
        //SoundManager.instance.PlaySloMoTimer();

        aimReticleObject.SetActive(true);

        //update image fill to red as time runs out
        timerFillUI.DOColor(timeOverColor, duration).SetUpdate(true);

        tween = DOTween.To(() => timerFillUI.fillAmount, x => timerFillUI.fillAmount = x, 0, duration).SetUpdate(true).OnComplete(() =>
        {
            EndBulletTime(value);
            timerComplete();
            
        });

    }
    public void EndBulletTime(int value)
    {
        if (tween.IsPlaying())
        {
            tween.Kill();
        }
        //Debug.Log("kill bullet time ");
        aimReticleObject.SetActive(false);
        ResetScales();
        UpdateBulletTimeUI(value);
    }
    public void ResetScales()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        timerFillUI.fillAmount = 1f;
        timerFillUI.color = defaultColor;
    }
    
    public void GotoLevelSelectionScreen()
    {
        SceneLoader.Instance.LoadScene(2);
        GameManger.Instance.TogglePauseGame(false);
        GameManger.Instance.ToggleMenuMusic(true);
    }
    public void GotoNextLevel()
    {
        SceneLoader.Instance.LoadNextScene();
        GameManger.Instance.TogglePauseGame(false);

    }
    public void ReplayScene()
    {
        SceneLoader.Instance.ReloadCurrentScreen();
        GameManger.Instance.TogglePauseGame(false);

    }
    public void LoadMenu()
    {
        SceneLoader.Instance.LoadScene(1);
        GameManger.Instance.TogglePauseGame(false);
        GameManger.Instance.ToggleMenuMusic(true);
    }
    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }

    
    public void DashViaButton()
    {
        dashButtonAction.Invoke();    
    }
    public void GrappleViaButton()
    {
        grappleButtonAction.Invoke();
    }
    private void OnDisable()
    {
        UpdateMidAirJumpUI -= UpdateDashAbilityUI;

        bananaRespawnButton.onClick.RemoveListener(RespawnViaBananas);
        dashButton.onClick.RemoveListener(DashViaButton);
        grappleButton.onClick.RemoveListener(GrappleViaButton);

        LootDrop.OnCollection -= TweenCollection;

    }
}
