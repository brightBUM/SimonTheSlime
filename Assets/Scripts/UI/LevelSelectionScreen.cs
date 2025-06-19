using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionScreen : MonoBehaviour
{
    [SerializeField] Transform starPrefab;
    [SerializeField] RectTransform panelParent;
    [SerializeField] Ease ease = Ease.InSine;
    [SerializeField] Transform playerPointer;
    [SerializeField] GameObject levelPodPrefab;
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
        for (int i = 0; i < levelCount; i++)
        {
            var levelPod = Instantiate(levelPodPrefab, contentParent);
            levelPod.GetComponent<LevelSelectPod>().Init(i);
        }
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
