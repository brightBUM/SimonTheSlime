using Unity.Services.LevelPlay;
using UnityEngine;

public class IronSourceAdManager : MonoBehaviour
{
    private LevelPlayBannerAd bannerAd;
    public LevelPlayInterstitialAd interstitialAd;
    public static IronSourceAdManager Instance;

#if UNITY_ANDROID && !UNITY_EDITOR
    string appKey = "85460dcd";
    string bannerAdUnitId = "thnfvcsog13bhn08";
    string interstitialAdUnitId = "aeyqi3vqlv6o8sh9";
#else
    string appKey = "unexpected_platform";
    string bannerAdUnitId = "unexpected_platform";
    string interstitialAdUnitId = "unexpected_platform";
#endif
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        IronSource.Agent.validateIntegration();

        LevelPlay.Init(appKey, adFormats: new[] { com.unity3d.mediation.LevelPlayAdFormat.REWARDED });

        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
    }
    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent with config: " + config);
        EnableAds();
    }

    private void EnableAds()
    {
        bannerAd = new LevelPlayBannerAd(bannerAdUnitId);

        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);
        interstitialAd.LoadAd();
    }
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo);
    }
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        //Debug.Log("unity-script: I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
        Debug.Log("rewarded ad complete");
    }
    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo);
    }
    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log("unity-script: I got SdkInitializationFailedEvent with error: " + error);
    }
   
   
    private void OnApplicationPause(bool pause)
    {
        IronSource.Agent.onApplicationPause(pause);
    }
    #region BannerAds
    public void LoadBannerAd()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        bannerAd.LoadAd();
#endif
        
    }
    public void HideBannerAd()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        bannerAd.HideAd();
#endif

    }
#endregion

    #region InterstitialAds
    public void LoadInterstitialAd()
    {
        interstitialAd.LoadAd();
        Debug.Log("iron source load interstitial ad");
    }
    public bool IsInterstitialAdReady()
    {
        return interstitialAd.IsAdReady();
    }
    public void ShowInterstitialAd()
    {
        if (interstitialAd.IsAdReady())
        {
            interstitialAd.ShowAd();
        }
        else
        {
            Debug.Log("Iron source - Interstitial ad not ready");
        }
    }
   
    #endregion

    #region RewardedAds
    
    public void ShowRewardedAd()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("Iron source - rewared ad not ready");
        }
    }

    #endregion

    private void OnDisable()
    {
        bannerAd?.DestroyAd();
        interstitialAd?.DestroyAd();
    }
}
