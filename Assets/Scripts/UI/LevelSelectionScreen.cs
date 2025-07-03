using DG.Tweening;
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
        //update currency UI
        UpdateCurrencyUI();

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


    private void UpdateCurrencyUI()
    {
        var playerProfile = SaveLoadManager.Instance.playerProfile;
        nanasText.text    = playerProfile.nanas.ToString();
        melonsText.text   = playerProfile.melons.ToString();
        screwsText.text   = playerProfile.screws.ToString();
        batteryText.text  = playerProfile.batteries.ToString();
    }
    public void TweenToPage(float value)
    {
        panelParent.DOAnchorPosX(value,0.5f).SetEase(ease);
    }

    public void UnlockNextPage()
    {
        var playerProfile = SaveLoadManager.Instance.playerProfile;

        playerProfile.screws -= GameManger.Instance.gameConfig.parts[2 * playerProfile.pageUnlockProgress];
        playerProfile.batteries -= GameManger.Instance.gameConfig.parts[2 * playerProfile.pageUnlockProgress+1];

        playerProfile.pageUnlockProgress++;
        UpdateCurrencyUI();
        SaveLoadManager.Instance.SaveGame();
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
