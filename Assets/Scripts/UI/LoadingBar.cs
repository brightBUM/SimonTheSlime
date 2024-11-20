using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LineRenderer progressLineRenderer;
    [SerializeField] float radius;
    [SerializeField] int samplePoints = 20;
    [SerializeField] Transform loadingHandle;
    
    [Range(0,20)] public int progress;
    [Range(0,1)] public float normRange;
    float angleStep;

    //bool ready = false;
    //bool loaded = false;
    AsyncOperation AsyncOp;
    private void Awake()
    {

    }
    void Start()
    {
        //lineRenderer.positionCount = samplePoints+1;
        //angleStep = 180 / samplePoints;

        //DrawArc();
        StartCoroutine(ShowLoadingProgress());

    }

    void SmoothLoading()
    {
        //lerp loading screen
        DOTween.To(() => progress, x => progress = x, 20, 1f).OnUpdate(() =>
        {
            UpdateLoadingProgress(progress);

        }).OnComplete(() =>
        {
            AsyncOp.allowSceneActivation = true;
        });
        //allow screen activation
    }

    public IEnumerator ShowLoadingProgress()
    {
        UpdateLoadingProgress(0);

        AsyncOp = SceneManager.LoadSceneAsync(GameManger.Instance.selectedIndex);

        AsyncOp.allowSceneActivation = false;


        while (!AsyncOp.isDone)
        {
            //UpdateLoadingProgress((int)(AsyncOp.progress * 20.0f));

            if (AsyncOp.progress >= 0.9f)
            {
                SmoothLoading();
                yield break;
            }
            

            yield return null;
        }
    }
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        //int newRange = (int)(normRange * 20.0f);
        //UpdateLoadingProgress(newRange);
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

    public void UpdateLoadingProgress(int progress)
    {

        loadingHandle.position = lineRenderer.GetPosition((samplePoints) - progress);

        if (progress < 19)
        {
            var dir = lineRenderer.GetPosition((samplePoints - 1) - progress - 1) - loadingHandle.position;

            var zRotation = Mathf.Atan(dir.y / dir.x) * Mathf.Rad2Deg;
            loadingHandle.rotation = Quaternion.Euler(0, 0, zRotation);
        }


        UpdateProgressLineRenderer(progress);
    }
    void UpdateProgressLineRenderer(int progress)
    {
        progressLineRenderer.positionCount = progress+1;

        for(int i = 0;i<progress+1;i++)
        {
            var pos = lineRenderer.GetPosition((samplePoints) - i);
            progressLineRenderer.SetPosition(i, pos);
        }
    }
}
