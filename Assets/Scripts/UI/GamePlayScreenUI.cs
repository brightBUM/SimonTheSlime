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
using UnityEngine.Events;

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
    [SerializeField] TextMeshProUGUI nanasLevelFailedUI;
    [SerializeField] TextMeshProUGUI cursedNanasLevelFailedUI;
    [SerializeField] TextMeshProUGUI gemsLevelFailedUI;
    [SerializeField] GameObject nextLevelButton;
    [SerializeField] float levelCompleteTextDelay = 0.2f;
    [SerializeField] float scoreCountTime = 2f;
    private List<TextMeshProUGUI> levelCompleteTexts;
    [SerializeField] List<GameObject> starItem;
    [Header("Retry")]
    [SerializeField] Button bananaRespawnButton;
    [SerializeField] TextMeshProUGUI nanasCost;
    [SerializeField] Image retryImageFill;
    [Header("Panel")]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameplayScreen;
    [SerializeField] GameObject ScoreboardScreen;
    [SerializeField] GameObject retryScreen;
    public InventoryHandler inventoryPanel;
    [SerializeField] Transform invShowPos;

    Color defaultColor;
    public static GamePlayScreenUI Instance;
    public Action<float> UpdateMidAirJumpUI;
    public Action poundAbilityAction;
    public Action poundReleaseAction;
    public Action dashButtonAction;
    public Action grappleButtonAction;
    public Action ShowInventoryPanelAction;
    public UnityEvent EnableThanksScreen;
    private TweenerCore<float, float, FloatOptions> tween;
    public bool BulletTimeActive => timerFillUI.fillAmount < 1f;
    float retryCountDownDuration = 3f;

    bool dashButtonState;
    bool grappleButtonState;
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        UpdateMidAirJumpUI += UpdateDashAbilityUI;
        bananaRespawnButton.onClick.AddListener(RespawnViaBananas);
        ScaleTexts();

        dashButton.onClick.AddListener(DashViaButton);
        grappleButton.onClick.AddListener(GrappleViaButton);
        LootDrop.OnCollection += TweenCollection;
        ShowInventoryPanelAction += ShowInventory;
    }

    
    void Start()
    {
        UpdateDashAbilityUI(0f);
        UpdateBananaCount(LevelManager.Instance.GetLevelBananasCount());
        defaultColor = timerFillUI.color;
        ScoreboardScreen.transform.localScale = Vector3.zero;
        Time.timeScale = 1f;

        dashButtonState = dashButton.gameObject.activeInHierarchy;
        grappleButtonState = grappleButton.gameObject.activeInHierarchy;

        duration = GameManger.Instance.GetCharUpgradeCurrentValue(UpgradeStatId.BulletTime);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    public void EnableButtonsForDungeon()
    {
        dashButton.gameObject.SetActive(true);
        grappleButton.gameObject.SetActive(true);
    }
    public void RestoreButtonStates()
    {
        dashButton.gameObject.SetActive(dashButtonState);
        grappleButton.gameObject.SetActive(grappleButtonState);
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
    private void ShowInventory()
    {
        inventoryPanel.transform.DOMove(invShowPos.position, 0.5f).SetEase(Ease.OutBack);
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
    public void ToggleGameplayScreen(bool value)
    {
        gameplayScreen.SetActive(value);
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

        if(retryCountdownCoroutine!=null)
            StopCoroutine(retryCountdownCoroutine);

#if UNITY_ANDROID && !UNITY_EDITOR //check interstitial ad condition
        
        SaveLoadManager.Instance.playerProfile.interStitialAdCount++;

        if (SaveLoadManager.Instance.CheckInterstitialAdCondition())
        {
            var IronSourceInstance = IronSourceAdManager.Instance;
            if (IronSourceInstance.IsInterstitialAdReady())
            {
                IronSourceInstance.ShowInterstitialAd();
                IronSourceInstance.interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
                IronSourceInstance.interstitialAd.OnAdDisplayFailed += InterstitialAd_OnAdDisplayFailed; ;
                IronSourceInstance.interstitialAd.OnAdLoadFailed += InterstitialAd_OnAdLoadFailed;
                return;
            }
            else
            {
                Debug.Log("android - interstitial ad not ready");
            }
        }  

#endif
        Debug.Log("showing level failed leaderboard without ad");

        LevelFailedLeaderBoard();

    }

    private void InterstitialAd_OnAdLoadFailed(LevelPlayAdError obj)
    {
        //incase ad load fails , continue with level complete
        Debug.Log("level end interstitial ad display failed");
        LevelFailedLeaderBoard();

        IronSourceAdManager.Instance.interstitialAd.OnAdLoadFailed -= InterstitialAd_OnAdLoadFailed;
    }

    private void LevelFailedLeaderBoard()
    {
        GameManger.Instance.TogglePauseGame(false);
        LevelManager.Instance.startLevelTimer = false;
        retryScreen.SetActive(false);
        nextLevelButton.SetActive(false);
        ScoreboardScreen.SetActive(true);
        ScoreboardScreen.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
        CurrencyManager.ToggleCurrencyPanel(true);
        DOVirtual.DelayedCall(0.3f, () =>
        {
            UpdateLevelFailedUI();
        });

        LevelManager.Instance.LevelFailStatsToProfile();
        SaveLoadManager.Instance.SaveGame();
    }
#pragma warning disable 0618
    private void InterstitialAd_OnAdDisplayFailed(LevelPlayAdDisplayInfoError obj)
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
    Coroutine retryCountdownCoroutine;
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
        //trigger retry countdown
        retryCountdownCoroutine = StartCoroutine(TriggerRetryCountDown());
    }
    private IEnumerator TriggerRetryCountDown()
    {
        float timer = retryCountDownDuration;
        while(timer>0)
        {
            retryImageFill.fillAmount = (float)timer/retryCountDownDuration;
            timer-= Time.unscaledDeltaTime;
            yield return null;
        }

        //
        TriggerLevelFailedScoreboard();
    }
    private int CostToRespawn()
    {
        return GameManger.Instance.gameConfig.RetryNanasCost * LevelManager.Instance.retryCount;
    }
    public void RespawnViaBananas()
    {
        if (retryCountdownCoroutine != null)
            StopCoroutine(retryCountdownCoroutine);

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
        if (retryCountdownCoroutine != null)
            StopCoroutine(retryCountdownCoroutine);

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
        IronSourceAdManager.Instance.rewardedAd.OnAdRewarded += RewardedAd_OnAdRewarded;
#endif
    }

    private void RewardedAd_OnAdRewarded(LevelPlayAdInfo arg1, LevelPlayReward arg2)
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

        IronSourceAdManager.Instance.rewardedAd.OnAdRewarded -= RewardedAd_OnAdRewarded;

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

        //add level stats to profile
        LevelManager.Instance.AddLevelStatsToProfile();

        FindAnyObjectByType<ScoreBoard>().TriggerScoreBoard();

        //check if first play or replay
        SaveLoadManager.Instance.FirstOrReplay(LevelManager.Instance.GetWonStars());

        //unlock next level , if atleast 1 star acquired
        LevelManager.Instance.UnlockNextLevel();

        SaveLoadManager.Instance.SaveGame();
    }
    public void UpdateLevelFailedUI()
    {

        LevelManager levelManager = LevelManager.Instance;

        var nanasAmount = levelManager.collectedBananas;
        var cursedNanasAmount = levelManager.collectedCursedNanas;
        var melonsAmount = levelManager.collectedMelons;

        nanasLevelFailedUI.text = nanasAmount.ToString();
        cursedNanasLevelFailedUI.text = cursedNanasAmount.ToString();
        gemsLevelFailedUI.text = melonsAmount.ToString();
        if(nanasAmount>0)
            CurrencyManager.CurrencyCollectAction(nanasAmount, nanasLevelFailedUI.transform.position, CurrencyType.Nanas);
        if(cursedNanasAmount>0)
            CurrencyManager.CurrencyCollectAction(cursedNanasAmount, cursedNanasLevelFailedUI.transform.position, CurrencyType.cursedNanas);
        if(melonsAmount>0)
            CurrencyManager.CurrencyCollectAction(melonsAmount, gemsLevelFailedUI.transform.position, CurrencyType.Melons);
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
        if (LevelManager.Instance.IsTutorialActive)
            return;
        
        dashButtonAction.Invoke();    
    }
    public void GrappleViaButton()
    {
        if (LevelManager.Instance.IsTutorialActive)
            return;

        grappleButtonAction.Invoke();
    }
    private void OnDisable()
    {
        UpdateMidAirJumpUI -= UpdateDashAbilityUI;

        bananaRespawnButton.onClick.RemoveListener(RespawnViaBananas);
        dashButton.onClick.RemoveListener(DashViaButton);
        grappleButton.onClick.RemoveListener(GrappleViaButton);

        LootDrop.OnCollection -= TweenCollection;
        ShowInventoryPanelAction -= ShowInventory;

    }
}
