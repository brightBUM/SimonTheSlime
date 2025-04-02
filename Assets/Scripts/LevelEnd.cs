using DG.Tweening;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] Transform sleepingPlayer;
    [SerializeField] float yValue;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            SoundManager.Instance.PlaySlimeSplashSFX();
            //ObjectPoolManager.Instance.Spawn(4,transform.position,Quaternion.Euler(90, 0, 0));
            //change player to roll/sleep state 
            playerController.gameObject.SetActive(false);
            sleepingPlayer.gameObject.SetActive(true);
            sleepingPlayer.DOLocalMoveY(yValue, 1f).SetEase(Ease.OutCubic);

            LevelManager.Instance.InvokeLevelCompleteAnalytics();

            //play level complete music 
            //spawn scoreboard menu
            Debug.Log("level end called");

            DOVirtual.DelayedCall(2f, () =>
            {
                

#if UNITY_EDITOR

                SoundManager.Instance.PlayLevelCompleteSFx();
                GamePlayScreenUI.Instance.ShowLevelCompleteScreen();

#elif UNITY_ANDROID //check interstitial ad condition

                SoundManager.Instance.PlayLevelCompleteSFx();
                GamePlayScreenUI.Instance.ShowLevelCompleteScreen();

                Debug.Log("level end - android code");
                //SaveLoadManager.Instance.playerProfile.interStitialAdCount++;

                //if (SaveLoadManager.Instance.CheckInterstitialAdCondition())
                //{
                //    IronSourceAdManager.Instance.ShowInterstitialAd();
                //    IronSourceAdManager.Instance.interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
                //}
#endif
            });
        }
    }

    //private void InterstitialOnAdClosedEvent(LevelPlayAdInfo info)
    //{
    //    SaveLoadManager.Instance.playerProfile.interStitialAdCount = 0;

        

    //    IronSourceAdManager.Instance.LoadInterstitialAd();
    //    IronSourceAdManager.Instance.interstitialAd.OnAdClosed -= InterstitialOnAdClosedEvent;
    //}
}
