using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.UI;

public class IronSourceAdManager : MonoBehaviour
{
    [SerializeField] Image rewaredStatus;
    [SerializeField] Image interstitialStatus;
    [SerializeField] Image bannerStatus;
    private LevelPlayBannerAd bannerAd;
    public LevelPlayInterstitialAd interstitialAd;
    public LevelPlayRewardedAd rewardedAd;
    public static IronSourceAdManager Instance;
    public bool NoAdsPurchased { get; set; }

    public bool sdkInitialized;

#if UNITY_ANDROID && !UNITY_EDITOR
    string appKey = "21c87ea5d";
    string bannerAdUnitId = "rq9jn6t8h4mdqh43";
    string interstitialAdUnitId = "wgtkbxwatw27k8bb";
    string rewardedAdUnitId = "jy008whzgkdtz97k";
#else
    string appKey = "unexpected_platform";
    string bannerAdUnitId = "unexpected_platform";
    string interstitialAdUnitId = "unexpected_platform";
    string rewardedAdUnitId = "unexpected_platform";
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
    private void Update()
    {
        if(sdkInitialized)
        {
            rewaredStatus.color = IsRewardedAdReady()?Color.green:Color.red;
            interstitialStatus.color = IsInterstitialAdReady()?Color.green:Color.red;
        }
    }
    public void Start()
    {
        LevelPlay.ValidateIntegration();

        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        LevelPlay.Init(appKey);

        bannerStatus.color = Color.red;

    }
    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent with config: " + config);
        EnableAds();
        sdkInitialized = true;
    }

    private void EnableAds()
    {
        bannerAd = new LevelPlayBannerAd(bannerAdUnitId);
        //bannerAd.LoadAd();
        //bannerAd.OnAdLoaded += BannerAd_OnAdLoaded;

        //keep the rewarded video ad ready for main menu daily reward
        interstitialAd = new LevelPlayInterstitialAd(interstitialAdUnitId);
        interstitialAd.LoadAd();
        interstitialAd.OnAdLoaded += InterstitialAd_OnAdLoaded;
        interstitialAd.OnAdLoadFailed += InterstitialAd_OnAdLoadFailed;

        rewardedAd = new LevelPlayRewardedAd(rewardedAdUnitId);
        rewardedAd.LoadAd();
        rewardedAd.OnAdLoadFailed += RewardedAd_OnAdLoadFailed;
        rewardedAd.OnAdLoaded += RewardedAd_OnAdLoaded;

    }

   

    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log("unity-script: I got SdkInitializationFailedEvent with error: " + error);
        sdkInitialized = false;
    }
   
   
    
    #region BannerAds
    public void LoadBannerAd()
    {
        if (NoAdsPurchased)
            return;

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
    private void BannerAd_OnAdLoaded(com.unity3d.mediation.LevelPlayAdInfo obj)
    {
        bannerStatus.color = Color.green;

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
    private void InterstitialAd_OnAdLoadFailed(LevelPlayAdError obj)
    {
        Debug.Log("interstitial ad load failed " + obj.ErrorMessage);
    }

    private void InterstitialAd_OnAdLoaded(LevelPlayAdInfo obj)
    {
        Debug.Log("interstitial ad load successfully ");
    }
    public void ShowInterstitialAd()
    {
        if(NoAdsPurchased)
            return;

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

    private void RewardedAd_OnAdLoaded(LevelPlayAdInfo obj)
    {
        Debug.Log("rewarded ad loaded successfully ");

    }

    private void RewardedAd_OnAdLoadFailed(LevelPlayAdError obj)
    {
        Debug.Log("rewarded ad load failed " + obj.ErrorMessage);

    }
    public bool IsRewardedAdReady()
    {
       return rewardedAd.IsAdReady();
    }
    public void LoadRewardedAd()
    {
        rewardedAd.LoadAd();
    }
    public void ShowRewardedAd()
    {
        if (rewardedAd.IsAdReady())
        {
            rewardedAd.ShowAd();
        }
        else
        {
            Debug.Log("Iron source - rewared ad not ready");
            rewardedAd.LoadAd();
        }
    }

    #endregion

    private void OnDisable()
    {
        bannerAd?.DestroyAd();
        interstitialAd?.DestroyAd();
        rewardedAd.DestroyAd();

        interstitialAd.OnAdLoaded       -= InterstitialAd_OnAdLoaded;
        interstitialAd.OnAdLoadFailed   -= InterstitialAd_OnAdLoadFailed;
        rewardedAd.OnAdLoadFailed       -= RewardedAd_OnAdLoadFailed;
        rewardedAd.OnAdLoaded           -= RewardedAd_OnAdLoaded;
    }
}
