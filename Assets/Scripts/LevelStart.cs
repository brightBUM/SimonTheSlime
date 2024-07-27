using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour
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
        liquidTransform.DOScaleY(1.92f, liquidRiseDuration).OnComplete(() =>
        {
            //tween sleeping player 
            sleepingPlayerTransform.DOMoveY(entryPoint.position.y, sleepingPlayerRiseDuration).OnComplete(() =>
            {
                //spawn player Prefab
                sleepingPlayerTransform.gameObject.SetActive(false);
                playerPrefab.gameObject.SetActive(true);
                liquidTransform.DOScaleY(0, liquidfallDuration);
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
