using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
            SoundManager.instance.PlayAcidSplashSFx();
            //ObjectPoolManager.Instance.Spawn(4,transform.position,Quaternion.Euler(90, 0, 0));
            //change player to roll/sleep state 
            playerController.gameObject.SetActive(false);
            sleepingPlayer.gameObject.SetActive(true);
            sleepingPlayer.DOLocalMoveY(yValue, 1f).SetEase(Ease.OutCubic);

            LevelManager.Instance.startLevelTimer = true;
            //play level complete music 
            //spawn scoreboard menu
            DOVirtual.DelayedCall(2f, () =>
            {
                SoundManager.instance.PlayLevelCompleteSFx();
                GamePlayScreenUI.instance.UpdateLevelCompleteUI();
                GamePlayScreenUI.instance.ToggleGamePlayScreen(false);
                GamePlayScreenUI.instance.ToggleLevelCompleteScreen(true);
            });
        }
    }
}
