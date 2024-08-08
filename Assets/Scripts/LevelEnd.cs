using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] GameObject splashVFX;
    [SerializeField] GameObject levelCompletePanel;
    // Start is called before the first frame update
    void Start()
    {
        levelCompletePanel.transform.localScale = Vector3.zero;

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
            Instantiate(splashVFX,playerController.transform.position,splashVFX.transform.rotation);
            //change player to roll/sleep state 
            playerController.SetToFirstBounce();
            //play level complete music 
            //spawn scoreboard menu
            DOVirtual.DelayedCall(2f, () =>
            {
                SoundManager.instance.PlayLevelCompleteSFx();
                GamePlayScreenUI.instance.UpdateBananasLevelComplete();
                GamePlayScreenUI.instance.ToggleGamePlayScreen(false);
                GamePlayScreenUI.instance.ToggleLevelCompleteScreen(true);
            });
        }
    }
}
