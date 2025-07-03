using Unity.Services.LevelPlay;
using DG.Tweening;
using UnityEngine;
using Cinemachine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] Transform sleepingPlayer;
    [SerializeField] Transform camCentre;
    [SerializeField] float yValue;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            //camZoom
            var virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();

            // set the cam follow to level end camCentre as player prefab will be disabled
            virtualCamera.Follow = camCentre; 

            var confiner = FindAnyObjectByType<CinemachineConfiner2D>();
            var orthoSize = virtualCamera.m_Lens.OrthographicSize;
            DOTween.To(() => orthoSize, x => orthoSize = x, 12, 0.5f).SetUpdate(true).OnUpdate(() =>
            {
                virtualCamera.m_Lens.OrthographicSize = orthoSize;
                confiner.InvalidateCache();
            });
            

            SoundManager.Instance.PlaySlimeSplashSFX();
            //ObjectPoolManager.Instance.Spawn(4,transform.position,Quaternion.Euler(90, 0, 0));
            //change player to roll/sleep state 
            playerController.gameObject.SetActive(false);
            sleepingPlayer.gameObject.SetActive(true);
            sleepingPlayer.DOLocalMoveY(yValue, 1f).SetEase(Ease.OutCubic);

            LevelManager.Instance.InvokeLevelCompleteAnalytics();

            //play level complete music 
            //spawn scoreboard menu

            DOVirtual.DelayedCall(0.5f, () =>
            {
                SoundManager.Instance.PlayLevelCompleteSFx();
                GamePlayScreenUI.Instance.ShowLevelCompleteScreen();

#if UNITY_ANDROID //check interstitial ad condition
                Debug.Log("level end - android code");
                SaveLoadManager.Instance.playerProfile.interStitialAdCount++;

                if (SaveLoadManager.Instance.CheckInterstitialAdCondition())
                {
                    IronSourceAdManager.Instance.ShowInterstitialAd();
                    IronSourceAdManager.Instance.interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
                    IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed += InterstitialAd_OnAdDisplayFailed;
                }
                else
                {
                    SoundManager.Instance.PlayLevelCompleteSFx();
                    GamePlayScreenUI.Instance.ShowLevelCompleteScreen();
                }
#endif
            });
        }
    }

    private void InterstitialAd_OnAdDisplayFailed(com.unity3d.mediation.LevelPlayAdDisplayInfoError obj)
    {
        //incase ad load fails , continue with level complete
        Debug.Log("level end interstitial ad display failed");
        //SoundManager.Instance.PlayLevelCompleteSFx();
        //GamePlayScreenUI.Instance.ShowLevelCompleteScreen();

        IronSourceAdManager.Instance.interstitialAd.OnAdDisplayFailed -= InterstitialAd_OnAdDisplayFailed;
    }

    private void InterstitialOnAdClosedEvent(LevelPlayAdInfo info)
    {
        // on intersitial ad watched and closed , display level complete
        SaveLoadManager.Instance.playerProfile.interStitialAdCount = 0;

        //SoundManager.Instance.PlayLevelCompleteSFx();
        //GamePlayScreenUI.Instance.ShowLevelCompleteScreen();

        IronSourceAdManager.Instance.LoadInterstitialAd();
        IronSourceAdManager.Instance.interstitialAd.OnAdClosed -= InterstitialOnAdClosedEvent;
    }
}
