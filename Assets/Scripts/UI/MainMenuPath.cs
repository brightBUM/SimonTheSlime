using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPath : MonoBehaviour
{
    [SerializeField] Transform[] wayPoints;
    [SerializeField] float duration = 0.5f;
    [SerializeField] Ease easeType;
    [SerializeField] RectTransform rectTransform;

    private Vector3[] positions = new Vector3[]
        {
            Vector3.zero, Vector3.zero, Vector3.zero,Vector3.zero
        };
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < wayPoints.Length; i++)
        {
            positions[i] = wayPoints[i].position;
        }

        rectTransform.DOPath(positions, duration,PathType.CatmullRom).SetEase(easeType).SetLoops(-1,LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
