using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LineRenderer progressLineRenderer;
    [SerializeField] float radius;
    [SerializeField] int samplePoints = 20;
    [SerializeField] Transform loadingHandle;
    [SerializeField]
    [Range(0,20)] int progress;
    float angleStep;
    // Start is called before the first frame update
    void Start()
    {
        //lineRenderer.positionCount = samplePoints+1;
        //angleStep = 180 / samplePoints;

        //DrawArc();
    }

    // Update is called once per frame
    void Update()
    {

        loadingHandle.position = lineRenderer.GetPosition((samplePoints)-progress);

        if(progress<19)
        {
            var dir = lineRenderer.GetPosition((samplePoints - 1) - progress - 1) - loadingHandle.position;

            var zRotation = Mathf.Atan(dir.y / dir.x) * Mathf.Rad2Deg;
            loadingHandle.rotation = Quaternion.Euler(0, 0, zRotation);
        }


        UpdateProgressLineRenderer();
    }

    void DrawArc()
    {
        var angle = 0f;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            var pt = new Vector2(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad));
            angle += angleStep;

            lineRenderer.SetPosition(i, pt);
        }
    }

    void UpdateProgressLineRenderer()
    {
        progressLineRenderer.positionCount = progress+1;

        for(int i = 0;i<progress+1;i++)
        {
            var pos = lineRenderer.GetPosition((samplePoints) - i);
            progressLineRenderer.SetPosition(i, pos);
        }
    }
}
