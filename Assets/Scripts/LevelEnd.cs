using Unity.Services.LevelPlay;
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

            LevelManager.Instance.startLevelTimer = false;
            //play level complete music 
            //spawn scoreboard menu
            Debug.Log("level end called");

            DOVirtual.DelayedCall(2f, () =>
            {
                //Debug.Log("level end delayed called");
                SaveLoadManager.Instance.playerProfile.interStitialAdCount++;

                //check interstitial ad condition
                if(SaveLoadManager.Instance.playerProfile.interStitialAdCount>=2)
                {
                    IronSourceAdManager.Instance.ShowInterstitialAd();
                    IronSourceAdManager.Instance.interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;

                }
                else
                {
                    SoundManager.Instance.PlayLevelCompleteSFx();
                    GamePlayScreenUI.Instance.ShowLevelCompleteScreen();
                }

            });
        }
    }

    private void InterstitialOnAdClosedEvent(LevelPlayAdInfo info)
    {
        SaveLoadManager.Instance.playerProfile.interStitialAdCount = 0;

        SoundManager.Instance.PlayLevelCompleteSFx();
        GamePlayScreenUI.Instance.ShowLevelCompleteScreen();
        IronSourceAdManager.Instance.interstitialAd.OnAdClosed -= InterstitialOnAdClosedEvent;
    }
}
