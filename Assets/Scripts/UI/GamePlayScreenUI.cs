using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GamePlayScreenUI : MonoBehaviour
{
    [Header("Ability UI")]
    [SerializeField] Image dashFillImage;
    [SerializeField] Image bulletTimeIcon;
    [SerializeField] TextMeshProUGUI bulletTimeText;
    [SerializeField] TextMeshProUGUI bananaText;
    [SerializeField] TextMeshProUGUI bananasLevelCompleteUI;
    [Header("Bullet time")]
    [SerializeField] Image timerFillUI;
    [SerializeField] Transform greenWheelUI;
    [SerializeField] GameObject aimReticleObject;
    [SerializeField] Color timeOverColor;
    [SerializeField] float duration = 0.5f;
    [Header("Collectibles")]
    [SerializeField] TextMeshProUGUI bananaUI;
    [Header("Pause/Setting Menu")]

    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject gameplayScreen;
    [SerializeField] GameObject levelCompleteScreen;

    Color defaultColor;
    public static GamePlayScreenUI instance;
    public Action<float> UpdateMidAirJumpUI;
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
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            TogglePauseMenu(paused);
        }
    }
    private void TogglePauseMenu(bool paused)
    {
        if(paused)
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
            Time.timeScale = 1f;
        }
    }

    public void ToggleGamePlayScreen(bool value)
    {
        gameplayScreen.SetActive(value);
    }
    public void ToggleLevelCompleteScreen(bool value)
    {
        if(value)
        {
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
    public void UpdateBananasLevelComplete()
    {
        bananasLevelCompleteUI.text = LevelManager.Instance.GetLevelBananasCount();
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
    private void ResetScales()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        timerFillUI.fillAmount = 1f;
        timerFillUI.color = defaultColor;
    }

    public void GotoLevelSelectionScreen()
    {
        SceneLoader.Instance.LoadScene(1);
    }
    public void GotoNextLevel()
    {
        SceneLoader.Instance.LoadNextScene();
    }
    public void ReplayScene()
    {
        SceneLoader.Instance.ReloadCurrentScreen();
    }
    private void OnDisable()
    {
        UpdateMidAirJumpUI -= UpdateDashAbilityUI;

    }
}
