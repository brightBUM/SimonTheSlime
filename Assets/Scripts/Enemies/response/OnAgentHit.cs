using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResponseType
{
    scale, position, rotation
}
public class OnAgentHit : OnHit
{
    public ResponseType responseType = ResponseType.scale;
    public Transform onHitResponseTransform;
    [ShowIf(nameof(responseType),ResponseType.scale)]
    public float scaleMagnitude;
    [ShowIf(nameof(responseType),ResponseType.position)]
    public float positionMagnitude;
    [ShowIf(nameof(responseType),ResponseType.rotation)]
    public float rotationMagnitude;
    [Range(0, .5f)] public float responseTime;


    protected override void Start()
    {
        base.Start();
        scaleMagnitude = (onHitResponseTransform.localScale.x * scaleMagnitude);
        
    }

    protected override void OnDamage(DamageInfo info)
    {
        if (!CanResponse(info))
            return;
        base.OnDamage(info);
        switch (responseType)
        {
            case ResponseType.scale: 
            ScaleFeedback(scaleMagnitude);
                break;  
                case ResponseType.position: 
            PositionFeedback(info.dir);
                break;  


        }

    }

    [ContextMenu("Scale feedback")]
    void ScaleFeedback(float magnitude)
    {
        onHitResponseTransform.DOBlendableScaleBy(Vector3.one * magnitude, responseTime).OnComplete(() =>
        {
            onHitResponseTransform.DOBlendableScaleBy(-Vector3.one * magnitude, responseTime / 2f);
        });
    }

    void PositionFeedback(Vector3 dir)
    {

        Vector3 projectedDir = Vector3.ProjectOnPlane(dir, onHitResponseTransform.up).normalized;

        onHitResponseTransform.DOBlendableLocalMoveBy(projectedDir * positionMagnitude, responseTime)
            .OnComplete(() =>
            {
                onHitResponseTransform.DOBlendableLocalMoveBy(-projectedDir * positionMagnitude, responseTime / 2f);
            });

    }
    [ContextMenu("RotationFeedback")]
    void RotationFeedback(Vector3 dir)
    {

         onHitResponseTransform.DOBlendableLocalRotateBy(dir.normalized * rotationMagnitude, responseTime).
        OnComplete(() =>
        {
            onHitResponseTransform.DOBlendableLocalMoveBy(-dir.normalized * rotationMagnitude, responseTime / 2f);
        });

    }
    public Vector3 direction;
    [ContextMenu("position feedback")]
    void positionfeedbackcheck() => PositionFeedback(direction);

    [ContextMenu("scale feedback")]
    void scalefeedbackcheck() => ScaleFeedback(scaleMagnitude);


}
