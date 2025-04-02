//using Unity.Services.LevelPlay;
//using UnityEngine;

//public class IronSourceAdManager : Singleton<IronSourceAdManager>
//{
//    private LevelPlayBannerAd bannerAd;
//    public LevelPlayInterstitialAd interstitialAd;

//#if UNITY_ANDROID
//    string appKey = "85460dcd";
//    string bannerAdUnitId = "thnfvcsog13bhn08";
//    string interstitialAdUnitId = "aeyqi3vqlv6o8sh9";
//#else
//    string appKey = "unexpected_platform";
//    string bannerAdUnitId = "unexpected_platform";
//    string interstitialAdUnitId = "unexpected_platform";
//#endif
//    public void Start()
//    {
//        IronSource.Agent.validateIntegration();

//        LevelPlay.Init(appKey, adFormats: new[] { com.unity3d.mediation.LevelPlayAdFormat.REWARDED });

//        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
//        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
//    }
//    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
//    {
//        Debug.Log("unity-script: I got SdkInitializationCompletedEvent with config: " + config);
//        EnableAds();
//    }

//    private void EnableAds()
//    {
//        //IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
//        //IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
//        bannerAd = new LevelPlayBannerAd(bannerAdUnitId);

//        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);
//        interstitialAd.LoadAd();


//    }
//    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
//    {
//        Debug.Log("unity-script: I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo);
//    }
//    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
//    {
//        //Debug.Log("unity-script: I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
//        Debug.Log("rewarded ad complete");
//    }
//    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
//    {
//        Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo);
//    }
//    void SdkInitializationFailedEvent(LevelPlayInitError error)
//    {
//        Debug.Log("unity-script: I got SdkInitializationFailedEvent with error: " + error);
//    }
   
   
//    private void OnApplicationPause(bool pause)
//    {
//        IronSource.Agent.onApplicationPause(pause);
//    }
//    #region BannerAds
//    public void LoadBannerAd()
//    {
//        bannerAd.LoadAd();
        
//    }
//    public void HideBannerAd()
//    {
//        bannerAd.HideAd();
//    }
//    #endregion

//    #region InterstitialAds
//    public void LoadInterstitialAd()
//    {
//        interstitialAd.LoadAd();
//        Debug.Log("iron source load interstitial ad");
//    }
//    public void ShowInterstitialAd()
//    {
//        if (interstitialAd.IsAdReady())
//        {
//            interstitialAd.ShowAd();
//        }
//        else
//        {
//            Debug.Log("Iron source - Interstitial ad not ready");
//        }
//    }
   
//    #endregion

//    #region RewardedAds
    
//    public void ShowRewardedAd()
//    {
//        if (IronSource.Agent.isRewardedVideoAvailable())
//        {
//            IronSource.Agent.showRewardedVideo();
//        }
//        else
//        {
//            Debug.Log("Iron source - rewared ad not ready");
//        }
//    }

//    #endregion

//    private void OnDisable()
//    {
//        bannerAd?.DestroyAd();
//        interstitialAd?.DestroyAd();
//    }
//}
