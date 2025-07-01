using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionScreen : MonoBehaviour
{
    [SerializeField] Transform starPrefab;
    [SerializeField] RectTransform panelParent;
    [SerializeField] Ease ease = Ease.InSine;
    [SerializeField] Transform playerPointer;
    [SerializeField] GameObject levelPodPrefab;
    [SerializeField] GameObject pagePanelPrefab;
    [SerializeField] Transform contentParent;
    [SerializeField] PageSnapScroll pageSnapScroll;
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] int levelCount = 10;
    public static LevelSelectionScreen Instance;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //spawn in 6 as a page 
        var pages = levelCount / 6;
        pages = levelCount % 6 != 0 ? pages + 1 : pages;
        //Debug.Log($"levelCount : {levelCount} , pages : {pages}");
        for (int i = 0; i < pages; i++)
        {
            var pagePanel = Instantiate(pagePanelPrefab,contentParent);
            var startIndex = 6 * i;
            var endIndex = Mathf.Min((6 * i + 6),levelCount);

            for (int j = startIndex ; j < endIndex ; j++)
            {
                var levelPod = Instantiate(levelPodPrefab, pagePanel.transform);
                levelPod.GetComponent<LevelSelectPod>().Init(j);
            }
        }

        var contentSizeFitter = contentParent.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        pageSnapScroll.Init();
    }
    public void TweenToPage(float value)
    {
        panelParent.DOAnchorPosX(value,0.5f).SetEase(ease);
    }
    public Transform GetPlayerPointer()
    {
        return playerPointer;
    }
    public void LoadLevel(int sceneIndex)
    {
        sceneLoader.SceneViaLoadingScreen(sceneIndex);

    }
}
