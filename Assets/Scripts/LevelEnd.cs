using Cinemachine;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [SerializeField] Transform sleepingPlayer;
    [SerializeField] Transform camCentre;
    [SerializeField] GameObject inventoryFullCanvas;
    [SerializeField] Transform invFullSpawnTransform;
    [SerializeField] float yValue;
    Action OnInventoryFullAction;
    private void OnEnable()
    {
        OnInventoryFullAction += InventoryFullTween;
    }
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
            var chainTargetPos = sleepingPlayer.transform.position;
            sleepingPlayer.DOLocalMoveY(yValue, 1f).SetEase(Ease.OutCubic);

            LevelManager.Instance.InvokeLevelCompleteAnalytics();

            //play level complete music 
            //spawn scoreboard menu

            SoundManager.Instance.PlayLevelCompleteSFx();
            GamePlayScreenUI.Instance.ToggleGameplayScreen(false);
            //check if creature chain >0 

            var creatureChain = playerController.GetComponent<CreatureChain>();
            if (creatureChain.segments.Count>0)
            {
                GamePlayScreenUI.Instance.ShowInventoryPanelAction?.Invoke();
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    GetComponent<ChainToInventory>().CollectChain(creatureChain, chainTargetPos, OnInventoryFullAction);

                    //trigger the level complete scoreboard after the creature chain collect
                    // 0.25f+0.25f = 0.5f for each collect tween 
                    float totalDuration = creatureChain.segments.Count * 0.25f + 0.5f;
                    DOVirtual.DelayedCall(totalDuration, () =>
                    {
                        GamePlayScreenUI.Instance.ShowLevelCompleteScreen();

                    });

                });
            }
            else
            {
                GamePlayScreenUI.Instance.ShowLevelCompleteScreen();
            }

        }
    }

    private void InventoryFullTween()
    {
        //Debug.Break();
        var inventoryCanvasObject = Instantiate(inventoryFullCanvas,invFullSpawnTransform.position,Quaternion.identity);

        var text =inventoryCanvasObject.GetComponentInChildren<TextMeshProUGUI>();

        var currentY = inventoryCanvasObject.transform.position.y;

        Sequence seq = DOTween.Sequence();
        seq.SetLink(inventoryCanvasObject);

        seq.Join(inventoryCanvasObject.transform.DOMoveY(currentY + 0.25f,0.5f));

        seq.Join(text.DOFade(0f, 0.5f));

        seq.OnComplete(() =>
        {
            Destroy(inventoryCanvasObject);
        });
    }

    private void OnDisable()
    {
        OnInventoryFullAction -= InventoryFullTween;

    }
}
