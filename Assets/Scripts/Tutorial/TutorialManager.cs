using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("UI references")]
    [SerializeField] GameObject UIAnimation;
    [SerializeField] RectTransform titlePanel;
    [SerializeField] RectTransform buttonPanel;
    [SerializeField] RectTransform titleTarget;
    [SerializeField] RectTransform buttonPanelTarget;
    [SerializeField] TextMeshProUGUI tutorialTextUI;
    [SerializeField] string tutorialText;
    [Header("References")]
    [SerializeField] CinemachineVirtualCamera vCamera;
    [SerializeField] TutorialPlayback tutorialPlayback;
    [SerializeField] PlayerInput ghostPlayer;
    [SerializeField] GameObject ghostPlayerCanvas;
    PlayerInput originalPlayer;

    Vector3 originalTitlePos;
    Vector3 originalButtonPanelPos;
    bool tutorialChoiceMade = false; // bug fix - to prevent Multi touch b/w Skip & Next
    bool tutorialSkipped = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //play tutorial only for the first time
        var starsInCurrentLevel = SaveLoadManager.Instance.GetLevelStarData(LevelManager.Instance.levelIndex);
        if (starsInCurrentLevel > 0)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            this.enabled = false;
            return;
        }

        originalTitlePos = titlePanel.anchoredPosition;
        originalButtonPanelPos = buttonPanel.anchoredPosition;
        tutorialTextUI.text = tutorialText;
    }

    private void OnEnable()
    {
        tutorialPlayback.OnPlayFinished += EndTutorial;
        tutorialPlayback.ToggleUIAnimationEvent += ToggleUIAnimation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerInput>(out PlayerInput player))
        {
            if (tutorialPlayback.finished || tutorialSkipped)
                return;

            LevelManager.Instance.IsTutorialActive = true;

            originalPlayer = player;
            originalPlayer.CancelHorizontal.Invoke();
            originalPlayer.enabled = false;
            
            ghostPlayer.gameObject.SetActive(true);
            ghostPlayer.IsGhostControl = true;
            tutorialPlayback.Init();

            vCamera.Follow = ghostPlayer.transform;

            TweenPanels(true);
        }
    }

    private void TweenPanels(bool value)
    {
        if(value)
        {
            titlePanel.DOAnchorPos(titleTarget.anchoredPosition, 0.5f);
            buttonPanel.DOAnchorPos(buttonPanelTarget.anchoredPosition, 0.5f);
        }
        else
        {
            titlePanel.DOAnchorPos(originalTitlePos, 0.5f);
            buttonPanel.DOAnchorPos(originalButtonPanelPos, 0.5f);

        }
       
    }
    public void NextButton()
    {
        if (tutorialChoiceMade) return;
        tutorialChoiceMade = true;

        tutorialPlayback.gameObject.SetActive(true);
        TweenPanels(false);
        ghostPlayerCanvas.SetActive(false);
    }
    public void SkipButton()
    {
        if (tutorialChoiceMade) return;
        tutorialChoiceMade = true;

        ghostPlayerCanvas.SetActive(false);
        TweenPanels(false);
        tutorialSkipped = true;
        EndTutorial();
    }

    private void EndTutorial()
    {
        ghostPlayer.GetComponentInChildren<PlayerAnimation>().DeathEffect();

        originalPlayer.enabled = true;
        vCamera.Follow = originalPlayer.transform;

        DOVirtual.DelayedCall(1.5f, () =>
        {
            ghostPlayer.enabled = false;
            ghostPlayer.gameObject.SetActive(false);
        });

        LevelManager.Instance.IsTutorialActive = false;
    }
    private void ToggleUIAnimation(bool value)
    {
        UIAnimation.SetActive(value);
    }
    private void OnDisable()
    {
        tutorialPlayback.OnPlayFinished -= EndTutorial;
        tutorialPlayback.ToggleUIAnimationEvent -= ToggleUIAnimation;

    }
}
