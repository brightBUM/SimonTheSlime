using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionScreen : MonoBehaviour
{
    [Header("Currency Tab")]
    [SerializeField] TextMeshProUGUI nanasText;
    [SerializeField] TextMeshProUGUI melonsText;
    [SerializeField] TextMeshProUGUI screwsText;
    [SerializeField] TextMeshProUGUI batteryText;

    [Header("Level Select ")]
    [SerializeField] Transform starPrefab;
    [SerializeField] RectTransform panelParent;
    [SerializeField] Ease ease = Ease.InSine;
    [SerializeField] Transform playerPointer;
    [SerializeField] GameObject levelPodPrefab;
    [SerializeField] LevelPage pagePanelPrefab;
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
            pagePanel.Init(i);

            var startIndex = 6 * i;
            var endIndex = Mathf.Min((6 * i + 6),levelCount);

            for (int j = startIndex ; j < endIndex ; j++)
            {
                var levelPod = Instantiate(levelPodPrefab, pagePanel.pagePanelParent);
                levelPod.GetComponent<LevelSelectPod>().Init(j);
            }
        }

        var contentSizeFitter = contentParent.gameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        pageSnapScroll.Init();
    }


   
    public void TweenToPage(float value)
    {
        panelParent.DOAnchorPosX(value,0.5f).SetEase(ease);
    }

    public void UnlockNextPage()
    {
        // no need to check can purchase for this , as the purchase button is interactable only when affordable
        var playerProfile = SaveLoadManager.Instance.playerProfile;
        var costList = GameManger.Instance.gameConfig.UnlockCosts.GetPageUnlockCost(playerProfile.pageUnlockProgress+1);
        
        if(SaveLoadManager.Instance.CanPurchase(costList))
        {
            playerProfile.pageUnlockProgress++;
            SaveLoadManager.Instance.SaveGame();
        }
        
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
