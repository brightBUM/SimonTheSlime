using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WatchAdRewardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rewardText;
    [SerializeField] TextMeshProUGUI afterrewardText;
    [SerializeField] Image bananaIcon;
    [SerializeField] GameObject AdImage;
    [SerializeField] GameObject lockedImage;
    [SerializeField] Button button;
    [SerializeField] GameObject rewardPamel;
    const string afterRewardstring = " Bananas\r\nRewarded";
    public bool rewardReady; /*{ get; set; }*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        CalculateRewardLockUnlock();
    }

    private void CalculateRewardLockUnlock()
    {
        //calculate last elapsed time reward ad was opened
        var span = (DateTime.Now - SaveLoadManager.Instance.GetLastRewardedAdTime());

        //Debug.Log($"now time : {DateTime.Now},last time = " +
        //    $"{SaveLoadManager.Instance.GetLastRewardedAdTime()},span days : {span.Days} ");
        var rewardValue = GameManger.Instance.gameConfig.mainMenuRewardedAdNanas;


        if (span.Days >= 1)
        {
            Debug.Log("main menu reward call");
            //trigger flashing tween
            rewardReady = true;
            rewardText.text = rewardValue.ToString() + " Nanas";
            rewardText.transform.DOScale(1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
            button.interactable = true;
        }
        else
        {
            rewardReady = false;
            button.interactable = false;
            GetUnlockProgress(span);
        }
        ToggleUnlockImages();
    }

    public void ShowMainMenuRewardedAd()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        IronSourceAdManager.Instance.ShowRewardedAd();
        IronSourceRewardedVideoEvents.onAdRewardedEvent += IronSourceRewardedVideoEvents_onAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdOpenedEvent += IronSourceRewardedVideoEvents_onAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += IronSourceRewardedVideoEvents_onAdClosedEvent;
        return;
#endif
        var rewardValue = GameManger.Instance.gameConfig.mainMenuRewardedAdNanas;
        afterrewardText.text = rewardValue.ToString() + afterRewardstring;
        rewardPamel.SetActive(true);

        SaveLoadManager.Instance.MainMenuAdRewarded();
        CalculateRewardLockUnlock();

        Debug.Log("main menu rewarded ad complete");

    }

    private void IronSourceRewardedVideoEvents_onAdClosedEvent(IronSourceAdInfo obj)
    {
        GameManger.Instance.RestoreAudio();
        IronSourceRewardedVideoEvents.onAdClosedEvent -= IronSourceRewardedVideoEvents_onAdClosedEvent;

    }

    private void IronSourceRewardedVideoEvents_onAdOpenedEvent(IronSourceAdInfo obj)
    {

        GameManger.Instance.SetAudioMute();
        IronSourceRewardedVideoEvents.onAdOpenedEvent -= IronSourceRewardedVideoEvents_onAdOpenedEvent;
    }

    private void IronSourceRewardedVideoEvents_onAdRewardedEvent(IronSourcePlacement arg1, IronSourceAdInfo arg2)
    {
        rewardPamel.SetActive(true);

        SaveLoadManager.Instance.MainMenuAdRewarded();
        CalculateRewardLockUnlock();

        Debug.Log("main menu rewarded ad complete");

        IronSourceRewardedVideoEvents.onAdRewardedEvent -= IronSourceRewardedVideoEvents_onAdRewardedEvent;

    }


    private void ToggleUnlockImages()
    {
        lockedImage.SetActive(!rewardReady);
        AdImage.SetActive(rewardReady);
    }

    public void GetUnlockProgress(TimeSpan timeSpan)
    {
        //gets time remain from Last unlock 
        rewardText.text = (24 - timeSpan.Hours)+ "H";
        //show that as fill amount for banana icon
        bananaIcon.fillAmount = (float)(timeSpan.Hours) / (float)24;
    }

    private void OnDisable()
    {
        if(DOTween.IsTweening(rewardText.transform))
        {
            DOTween.Kill(rewardText.transform);
            rewardText.transform.localScale = Vector3.one;
        }
    }
}
