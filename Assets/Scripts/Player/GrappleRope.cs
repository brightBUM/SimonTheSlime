using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GrappleRope : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] int samplePoints,waveCount,wobbleCount;
    [SerializeField] float waveSize,animSpeed;
    [SerializeField] AnimationCurve ropeAnimationCurve;
    // Start is called before the first frame update
    void Start()
    {
        //SetupLine();
    }

    private void SetupLine()
    {
        lineRenderer.positionCount = samplePoints;
        for(int i = 0; i < samplePoints; i++)
        {
            lineRenderer.SetPosition(i,this.transform.position);
        }
    }
    //public void ShootRope(Vector2 grapplePoint,Action grappleCompleted)
    //{
    //    lineRenderer.enabled = true;
    //    //shoot rope action via animation curve - Todo
    //    Vector2 lastPointPos = transform.position;
    //    DOTween.To(() => lastPointPos, x => lastPointPos = x, grapplePoint, lerpDuration).OnUpdate(() =>
    //    {
    //        lineRenderer.SetPosition(0, transform.position);
    //        lineRenderer.SetPosition(samplePoints - 1, lastPointPos);
    //    }).OnComplete(() =>
    //    {
    //        grappleCompleted.Invoke();
    //        lineRenderer.enabled = false;

    //    });
    //}

    public IEnumerator AnimateRope(Vector2 targetPos,Action completed)
    {
        lineRenderer.enabled = true;
        float angle = LookAtAngle(targetPos - (Vector2)transform.position);
        lineRenderer.positionCount = samplePoints;
        float percent = 0.0f;
        //Debug.Break();
        while(percent<=1f)
        {
            percent += Time.deltaTime*animSpeed;
            SetPoints(targetPos, percent,angle);
            yield return null;

        }
        SetPoints(targetPos, 1f, angle);
        completed.Invoke();
        lineRenderer.enabled = false;

    }

    private void SetPoints(Vector2 targetPos,float percent,float angle)
    {
        Vector2 ropeEnd = Vector2.Lerp(transform.position,targetPos,percent);
        float length = Vector2.Distance(transform.position, ropeEnd);

        for(int i = 0; i < samplePoints; i++)
        {
            float xPos = (float)i / samplePoints * length;
            float revPercent = 1 - percent;
            float amplitude = Mathf.Sin(revPercent*wobbleCount*Mathf.PI)*((1f-(float)i/samplePoints)*waveSize);
            float yPos = Mathf.Sin((float)waveCount * i / samplePoints * 2 * Mathf.PI * revPercent) * amplitude;

            var pos = RotatePoint(new Vector2(xPos, yPos) + (Vector2)transform.position, transform.position,angle);
            lineRenderer.SetPosition(i, pos);
        }
    }

    Vector2 RotatePoint(Vector2 point,Vector2 pivot,float angle)
    {
        var dir = point-pivot;
        dir = Quaternion.Euler(0, 0, angle)*dir;
        point = dir+pivot;
        return point;
    }
    private float LookAtAngle(Vector2 target)
    {
        return Mathf.Atan2(target.y , target.x) * Mathf.Rad2Deg;
    }
}
