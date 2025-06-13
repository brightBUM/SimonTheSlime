using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class LevelStart : BaseRespawn
{
    [SerializeField] Transform liquidTransform;
    [SerializeField] Transform entryPoint;
    [SerializeField] Transform sleepingPlayerTransform;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] float liquidRiseDuration = 2.5f;
    [SerializeField] float liquidfallDuration = 0.5f;
    [SerializeField] float sleepingPlayerRiseDuration = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        var virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
        virtualCamera.m_Lens.OrthographicSize = 12f;
        var orthoSize = virtualCamera.m_Lens.OrthographicSize;
        DOTween.To(() => orthoSize, x => orthoSize = x, 18, 3.5f).SetUpdate(true).OnUpdate(() =>
        {
            virtualCamera.m_Lens.OrthographicSize = orthoSize;

        });

        liquidTransform.DOScaleY(1.1f, liquidRiseDuration).OnComplete(() =>
        {
            //tween sleeping player 
            sleepingPlayerTransform.DOMoveY(entryPoint.position.y, sleepingPlayerRiseDuration).OnComplete(() =>
            {
                //spawn player Prefab
                sleepingPlayerTransform.gameObject.SetActive(false);
                playerPrefab.gameObject.SetActive(true);
                LevelManager.Instance.StartLevel();

                liquidTransform.DOScaleY(0, liquidfallDuration);
                
                LevelManager.Instance.SetRespawn(this);
            });
        });

       
    }
}
