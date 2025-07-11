using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [Header("Tween Tansforms")]
    [SerializeField] Transform scoreboardTitleUI;
    [SerializeField] Transform[] starShadowTransforms;
    [SerializeField] Transform[] leftPanelTransforms;
    [SerializeField] Transform[] rightPanelTransforms;
    [SerializeField] Transform[] starAwardedTransforms;
    [SerializeField] Transform[] buttonTransforms;
    [SerializeField] Button levelsButton;
    [SerializeField] Button replayButton;
    [SerializeField] Button NextButton;
    [Header("Text values")]
    [SerializeField] TextMeshProUGUI collectedBananas;
    [SerializeField] TextMeshProUGUI targetBananas;
    [SerializeField] TextMeshProUGUI timeTaken;
    [SerializeField] TextMeshProUGUI targetTime;
    [SerializeField] TextMeshProUGUI totalRespawns;
    [SerializeField] TextMeshProUGUI perfectJumpCount;
    [SerializeField] TextMeshProUGUI perfectJumpMultipler;
    [SerializeField] TextMeshProUGUI melonsCollected;
    [SerializeField] TextMeshProUGUI screwsText;
    [SerializeField] TextMeshProUGUI batteryText;

    [SerializeField] GameObject nextLevelButton;
    [SerializeField] float levelCompleteTextDelay = 0.2f;
    [SerializeField] float scoreCountTime = 2f;
    private List<TextMeshProUGUI> levelCompleteTexts;

    private void Awake()
    {
        levelsButton.onClick.AddListener(OnLevelClick);
        replayButton.onClick.AddListener(OnReplayClick);
        NextButton.onClick  .AddListener(OnNextClick);
    }
    // Start is called before the first frame update
    void Start()
    {
        AnimateCounter(collectedBananas, 0, 65, 0.3f);

    }
    #region LevelButton
    private void OnLevelClick()
    {
#if UNITY_ANDROID && !UNITY_EDITOR //check interstitial ad condition
        SaveLoadManager.Instance.playerProfile.interStitialAdCount++;

        if (SaveLoadManager.Instance.CheckInterstitialAdCondition())
        {
            IronSourceAdManager.Instance.ShowInterstitialAd();
            IronSourceAdManager.Instance.interstitialAd.OnAdClosed += Level_InterstitialOnAdClosedEvent;
            IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed += Level_InterstitialAd_OnAdDisplayFailed;
            return;
        }
#endif

        GamePlayScreenUI.Instance.GotoLevelSelectionScreen();

    }
    private void Level_InterstitialAd_OnAdDisplayFailed(com.unity3d.mediation.LevelPlayAdDisplayInfoError obj)
    {
        //incase ad load fails , continue with level complete
        Debug.Log("scoreboard Level button interstitial ad display failed");
        GamePlayScreenUI.Instance.GotoLevelSelectionScreen();

        IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed -= Level_InterstitialAd_OnAdDisplayFailed;
    }

    private void Level_InterstitialOnAdClosedEvent(LevelPlayAdInfo info)
    {
        // on intersitial ad watched and closed , reset count
        SaveLoadManager.Instance.playerProfile.interStitialAdCount = 0;

        GamePlayScreenUI.Instance.GotoLevelSelectionScreen();

        IronSourceAdManager.Instance.LoadInterstitialAd();
        IronSourceAdManager.Instance.interstitialAd.OnAdClosed -= Level_InterstitialOnAdClosedEvent;
    }

    #endregion

    #region ReplayButton
    private void OnReplayClick()
    {

#if UNITY_ANDROID && !UNITY_EDITOR //check interstitial ad condition
        SaveLoadManager.Instance.playerProfile.interStitialAdCount++;

        if (SaveLoadManager.Instance.CheckInterstitialAdCondition())
        {
            IronSourceAdManager.Instance.ShowInterstitialAd();
            IronSourceAdManager.Instance.interstitialAd.OnAdClosed += Replay_InterstitialOnAdClosedEvent;
            IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed += Replay_InterstitialAd_OnAdDisplayFailed;
            return;
        }
#endif
        GamePlayScreenUI.Instance.ReplayScene();
    }
    private void Replay_InterstitialAd_OnAdDisplayFailed(com.unity3d.mediation.LevelPlayAdDisplayInfoError obj)
    {
        //incase ad load fails 
        Debug.Log("scoreboard Replay button interstitial ad display failed");

        GamePlayScreenUI.Instance.ReplayScene();

        IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed -= Replay_InterstitialAd_OnAdDisplayFailed;
    }

    private void Replay_InterstitialOnAdClosedEvent(LevelPlayAdInfo info)
    {
        // on intersitial ad watched and closed , reset count
        SaveLoadManager.Instance.playerProfile.interStitialAdCount = 0;

        GamePlayScreenUI.Instance.ReplayScene();

        IronSourceAdManager.Instance.LoadInterstitialAd();
        IronSourceAdManager.Instance.interstitialAd.OnAdClosed -= Replay_InterstitialOnAdClosedEvent;
    }
    #endregion

    #region NextButton
    private void OnNextClick()
    {
        if(LevelManager.Instance.levelIndex == 14)
        {
            //show thanks screen for last level
            GamePlayScreenUI.Instance.EnableThanksScreen?.Invoke();
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR //check interstitial ad condition
        SaveLoadManager.Instance.playerProfile.interStitialAdCount++;

        if (SaveLoadManager.Instance.CheckInterstitialAdCondition())
        {
            IronSourceAdManager.Instance.ShowInterstitialAd();
            IronSourceAdManager.Instance.interstitialAd.OnAdClosed += Next_InterstitialOnAdClosedEvent;
            IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed += Next_InterstitialAd_OnAdDisplayFailed;
            return;
        }
#endif

        GotoNextAfterAd();
    }
    private void Next_InterstitialAd_OnAdDisplayFailed(com.unity3d.mediation.LevelPlayAdDisplayInfoError obj)
    {
        //incase ad load fails , continue with level complete
        Debug.Log("scoreboard Next button interstitial ad display failed");
        GotoNextAfterAd();

        IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed -= Next_InterstitialAd_OnAdDisplayFailed;
    }

    private void Next_InterstitialOnAdClosedEvent(LevelPlayAdInfo info)
    {
        // on intersitial ad watched and closed , reset count
        SaveLoadManager.Instance.playerProfile.interStitialAdCount = 0;

        GotoNextAfterAd();

        IronSourceAdManager.Instance.LoadInterstitialAd();
        IronSourceAdManager.Instance.interstitialAd.OnAdClosed -= Next_InterstitialOnAdClosedEvent;
    }

    private void GotoNextAfterAd()
    {
        //if last level of a page goto level select screen instead of next level
        var levelIndex = LevelManager.Instance.levelIndex;
        var pageUnlock = SaveLoadManager.Instance.playerProfile.pageUnlockProgress;
        if ((levelIndex == 5 && pageUnlock == 0) || (levelIndex == 11 && pageUnlock == 1))
        {
            LevelManager.Instance.UnlockNextLevel();
            GamePlayScreenUI.Instance.GotoLevelSelectionScreen();
        }
        else
        {
            GamePlayScreenUI.Instance.GotoNextLevel();
        }
    }
    #endregion
    public void AnimateCounter(TextMeshProUGUI counterText, int from, int to, float time)
    {
        int currentValue = from;
        counterText.text = currentValue.ToString();

        DOTween.To(() => currentValue, x => 
        {
            currentValue = x;
            counterText.text = currentValue.ToString();
        }
        , to, time);
    }
    
    private IEnumerator AssignValues()
    {
        var levelManager = LevelManager.Instance;

        yield return new WaitForSeconds(0.65f);
        
        //animate timer panel (left panel)
        targetTime.text = "Target : "+levelManager.TimeFormatConversion(levelManager.targetTime);

        int timerValue = 0;
        timeTaken.text = levelManager.TimeFormatConversion(timerValue);

        int target = (int)levelManager.levelTimer;
        DOTween.To(() => timerValue, x => {
            timerValue = x;
            timeTaken.text = levelManager.TimeFormatConversion(timerValue);
        }, target, 1f).SetEase(Ease.OutCubic);

        //animate perfect jumps (right panel)
        perfectJumpCount.text = levelManager.comboCount.ToString()+"x";
        var baseValue = GameManger.Instance.gameConfig.perfectJumpBase;
        var perfectJumpBonus = levelManager.comboCount;
        perfectJumpMultipler.text = levelManager.GetPerfectJumpBonus() ;

        yield return new WaitForSeconds(0.25f);
        //animate bananas panel
        targetBananas.text = "Target : "+levelManager.targetbananas.ToString();
        AnimateCounter(collectedBananas, 0, levelManager.collectedBananas, 1f);

        //animate gems text (right panel)
        melonsCollected.text = levelManager.GetMelonsCount();

        yield return new WaitForSeconds(0.25f);
        //animate respawn panel
        totalRespawns.text = (levelManager.TotalRespawnCount-1).ToString();

        //animate parts text (right panel)
        screwsText.text = levelManager.collectedScrews.ToString();
        batteryText.text = levelManager.collectedBatteries.ToString();
    }
    [ContextMenu("Tween ScoreBoard")]
    public void TriggerScoreBoard()
    {
        StartCoroutine(AssignValues());

        scoreboardTitleUI.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.OutBack);
        scoreboardTitleUI.DOScale(1f, 0.4f).SetEase(Ease.OutFlash).OnComplete(() =>
        {
            float delay = 0;
            foreach(Transform star in starShadowTransforms)
            {
                DOVirtual.DelayedCall(delay, () =>
                {
                    star.DOScale(2.2f, 0.25f).SetEase(Ease.OutQuad);
                    star.DORotate(Vector3.zero, 0.25f).SetEase(Ease.OutQuad);
                });
                delay += 0.1f;
            }
                
        });

        //0.4f + 0.2 + 0.25f - time to complete tween 

        DOVirtual.DelayedCall(0.65f, () =>
        {
            //tween left and right panels;
            float delay = 0;
            foreach (Transform panel in leftPanelTransforms)
            {
                DOVirtual.DelayedCall(0.65f+delay, () =>
                {
                    panel.DOLocalMoveX(0, 0.25f).SetEase(Ease.OutQuad);
                    panel.DOScale(1, 0.25f).SetEase(Ease.OutExpo);
                    
                });
                delay += 0.1f;
            }

            delay = 0;
            foreach (Transform panel in rightPanelTransforms)
            {
                DOVirtual.DelayedCall(0.65f + delay, () =>
                {
                    panel.DOLocalMoveX(0, 0.25f).SetEase(Ease.OutQuad);
                    panel.DOScale(1, 0.25f).SetEase(Ease.OutExpo);

                });
                delay += 0.1f;
            }
        });

        //0.65f + 0.2f + 0.25f

        //tween awarded stars

        DOVirtual.DelayedCall(1.05f, () =>
        {
            //get awarded stars
            int starsCount = LevelManager.Instance.GetWonStars();
            float delay = 0;
            for(int i=0;i<starsCount;i++)
            {
                ShakeScaleStar(starAwardedTransforms[i],delay);
                delay += 0.2f;
            }
            
        });

        //1.05f + 0.4 + 0.5f;

        DOVirtual.DelayedCall(1.95f,() =>
        {
            //tween buttons
            var buttons = new List<Button> { levelsButton, replayButton, NextButton };
            float delay = 0;
            foreach (Transform button in buttonTransforms)
            {
                DOVirtual.DelayedCall(delay,() =>
                {
                    button.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
                    delay += 0.1f;
                });
                
            }
        });
    }
    private void ShakeScaleStar(Transform transform,float delay)
    {
        DOVirtual.DelayedCall(1.05f + delay, () =>
        {
            transform.gameObject.SetActive(true);
            transform.DOShakeScale(0.5f, 2.2f).SetEase(Ease.InOutBounce);
        });
        
    }
    private void OnDestroy()
    {
        levelsButton.onClick.RemoveListener(OnLevelClick);
        replayButton.onClick.RemoveListener(OnReplayClick);
        NextButton.onClick  .RemoveListener(OnNextClick);
    }
}
