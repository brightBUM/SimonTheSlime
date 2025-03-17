using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WatchAdRewardUI : MonoBehaviour
{
    [SerializeField] int rewardAmount = 50;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image bananaIcon;
    [SerializeField] GameObject AdImage;
    [SerializeField] GameObject lockedImage;
    [SerializeField] Button button;
    [SerializeField] GameObject rewardPamel;
    const string rewardText = "50 Nanas";
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
        Debug.Log("calculate rewarded ad span hours :" + span.Hours);
        if (span.Days >= 1)
        {
            //trigger flashing tween
            rewardReady = true;
            text.text = rewardText;
            text.transform.DOScale(1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo);
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
        IronSourceAdManager.Instance.ShowRewardedAd();
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;

    }

    private void RewardedVideoOnAdClosedEvent(IronSourceAdInfo info)
    {
        rewardPamel.SetActive(true);
        SaveLoadManager.Instance.playerProfile.nanas += rewardAmount;
        Debug.Log("main menu rewarded ad complete");

        SaveLoadManager.Instance.SetLastRewardedAdTime(DateTime.Now);
        CalculateRewardLockUnlock();

        IronSourceRewardedVideoEvents.onAdClosedEvent -= RewardedVideoOnAdClosedEvent;
    }

    private void ToggleUnlockImages()
    {
        lockedImage.SetActive(!rewardReady);
        AdImage.SetActive(rewardReady);
    }

    public void GetUnlockProgress(TimeSpan timeSpan)
    {
        //gets time remain from Last unlock 
        text.text = timeSpan.Hours+"H";
        //show that as fill amount for banana icon
        bananaIcon.fillAmount = (float)(24- timeSpan.Hours) / (float)24;
    }

    private void OnDisable()
    {
        if(DOTween.IsTweening(text.transform))
        {
            DOTween.Kill(text.transform);
            text.transform.localScale = Vector3.one;
        }
    }
}
