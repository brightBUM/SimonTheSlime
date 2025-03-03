using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;

public class ActuatorPlatform : MonoBehaviour,IPoundable
{
    [SerializeField] SpriteRenderer spriteRenderer; //extendable platform's sprite renderer
    [SerializeField] Transform[] gears;
    [SerializeField] float stepAmount;
    [SerializeField] float gearRotateAmount = 35f;
    [SerializeField] float stepTime = 0.5f;
    [SerializeField] Ease easeType = Ease.Linear;
    private TweenerCore<Vector2, Vector2, VectorOptions> tween;
    private int hitCount = 5;
    
    public void OnPlayerPounded(Action<IPoundable> ContinuePound)
    {
        ContinuePound.Invoke(this);

        if (tween.IsPlaying() || hitCount<=0)
            return;

        hitCount--;
        StepIncrease(stepAmount);
        GetComponentInChildren<BouncyDeform>().HitDeform();
        SoundManager.Instance.PlayErectPlatformSFx();

        var currentEulerAngle = gears[0].rotation.eulerAngles;
        currentEulerAngle += Vector3.forward * gearRotateAmount;
        foreach (var gear in gears)
        {
            gear.DORotate(currentEulerAngle, stepTime);
            //gear.DOScale(1.2f, stepTime).SetLoops(LoopType.Yoyo);
        }
    }

    private void StepIncrease(float stepAmount)
    {
        var size = spriteRenderer.size;
        var targetSize = size + Vector2.right * stepAmount; // right because the platform is rotated vertically

        tween = DOTween.To(() => size, x => size = x, targetSize, stepTime).SetEase(easeType).OnUpdate(() =>
        {
            spriteRenderer.size = size;
        });

        
    }
    // Start is called before the first frame update
    void Start()
    {
        StepIncrease(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
