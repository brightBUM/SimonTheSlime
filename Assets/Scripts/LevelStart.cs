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
