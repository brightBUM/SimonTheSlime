using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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
    [SerializeField] Color timeOverColor;
    [SerializeField] float duration = 0.5f;

    [Header("Level Complete")]
    [SerializeField] TextMeshProUGUI bananasLevelCompleteUI;
    [SerializeField] TextMeshProUGUI levelTimerCompleteUI;
    [SerializeField] TextMeshProUGUI launchesUI;
    [SerializeField] List<GameObject> starItem;

    [Header("Panel")]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameplayScreen;
    [SerializeField] GameObject levelCompleteScreen;

    Color defaultColor;
    public static GamePlayScreenUI instance;
    public Action<float> UpdateMidAirJumpUI;
    public Action slamButtonAction;
    public Action dashButtonAction;
    public Action grappleButtonAction;
    private TweenerCore<float, float, FloatOptions> tween;
    [HideInInspector] public bool paused;
    public bool BulletTimeActive => timerFillUI.fillAmount < 1f;
    private void OnEnable()
    {
        UpdateMidAirJumpUI += UpdateDashAbilityUI;
    }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        UpdateDashAbilityUI(0f);
        UpdateBananaCount(LevelManager.Instance.GetLevelBananasCount());
        defaultColor = timerFillUI.color;
        levelCompleteScreen.transform.localScale = Vector3.zero;
        Time.timeScale = 1f;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    public void TogglePauseMenu()
    {
        if (levelCompleteScreen.activeInHierarchy)
            return;

        paused = !paused;

        if (paused)
        {
            gameplayScreen.SetActive(false);
            pauseScreen.SetActive(true);
            pauseScreen.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce).SetUpdate(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseScreen.transform.localScale = Vector3.zero;
            pauseScreen.SetActive(false);
            gameplayScreen.SetActive(true);
            GameManger.Instance?.SwapCursor(false);
            Time.timeScale = 1f;
        }
    }
    
    public void ToggleGamePlayScreen(bool value)
    {
        gameplayScreen.SetActive(value);
    }
    public void ToggleLevelCompleteScreen(bool value)
    {
        Debug.Log("toggle level complete ");

        if (value)
        {
            Debug.Log("toggle level complete true");
            levelCompleteScreen.SetActive(value);
            levelCompleteScreen.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
        }
        else
        {
            levelCompleteScreen.transform.localScale = Vector3.zero;
            levelCompleteScreen.SetActive(false);
        }
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
        SoundManager.instance.PlayOutofBulletTimeSFX();
    }
    
    public void UpdateBananaCount(string text)
    {
        bananaUI.text = text;
    }
    public void UpdateTimerText(string time)
    {
        levelTimerText.text = time;
    }
    public void UpdateLevelCompleteUI()
    {
        LevelManager levelManager = LevelManager.Instance;

        bananasLevelCompleteUI.text = levelManager.GetLevelBananasCount();
        levelTimerCompleteUI.text   = levelManager.GetLevelTimerText();
        launchesUI.text             = levelManager.GetLevelLaunches();
                                      
        //spawn stars and store if best score achieved      
        var currentStars            = levelManager.GetWonStars();

        for(int i=0;i<currentStars;i++)
        {
            starItem[i].SetActive(true);
            StartCoroutine(DelayedStarScale(0.2f+(0.2f*i),starItem[i].gameObject.transform));
        }

        if (SaveLoadManager.Instance == null)
            return;

        if (currentStars > SaveLoadManager.Instance.GetLevelStarData(levelManager.levelIndex))
        {
            SaveLoadManager.Instance.SetLevelStats(levelManager.levelIndex, currentStars);
        }
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
        GameManger.Instance.ToggleMenuMusic(true);
    }
    public void GotoNextLevel()
    {
        SceneLoader.Instance.LoadNextScene();
    }
    public void ReplayScene()
    {
        SceneLoader.Instance.ReloadCurrentScreen();
    }
    public void LoadMenu()
    {
        SceneLoader.Instance.LoadScene(1);
        GameManger.Instance.ToggleMenuMusic(true);
    }
    public void QuitGame()
    {
        SceneLoader.Instance.QuitGame();
    }

    public void SlamViaButton()
    {
        slamButtonAction.Invoke();
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

    }
}
