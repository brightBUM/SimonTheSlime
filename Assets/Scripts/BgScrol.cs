using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScrol : MonoBehaviour
{
    [SerializeField] Material material;
    float loopValue = 1;
    // Start is called before the first frame update
    void Start()
    {
        DOTween.To(() => loopValue, x => loopValue = x, 0, 2).SetLoops(-1).OnUpdate(() =>
        {
            material.mainTextureOffset = Vector2.right * loopValue;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
