using DG.Tweening;
using System;
using TMPro;
using Unity.Services.LevelPlay;
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
            rewardText.transform.DOScale(1.1f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetLink(this.gameObject);
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
#if UNITY_ANDROID && !UNITY_EDITOR
        IronSourceAdManager.Instance.ShowRewardedAd();
        IronSourceAdManager.Instance.rewardedAd.OnAdRewarded += RewardedAd_OnAdRewarded;
        return;
#endif
        var rewardValue = GameManger.Instance.gameConfig.mainMenuRewardedAdNanas;
        afterrewardText.text = rewardValue.ToString() + afterRewardstring;
        rewardPamel.SetActive(true);

        SaveLoadManager.Instance.MainMenuAdRewarded();
        CalculateRewardLockUnlock();

        Debug.Log("main menu rewarded ad complete - editor");

    }

    private void RewardedAd_OnAdRewarded(LevelPlayAdInfo arg1, LevelPlayReward arg2)
    {
        rewardPamel.SetActive(true);

        SaveLoadManager.Instance.MainMenuAdRewarded();
        CalculateRewardLockUnlock();

        Debug.Log("main menu rewarded ad complete - mobile");

        IronSourceAdManager.Instance.LoadRewardedAd();
        IronSourceAdManager.Instance.rewardedAd.OnAdRewarded -= RewardedAd_OnAdRewarded;
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
