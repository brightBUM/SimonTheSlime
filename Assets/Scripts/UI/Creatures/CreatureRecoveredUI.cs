using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureRecoveredUI : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject newUnlockSticker;
    //CreaturesPanel creaturesPanel;
    string creatureId;
    private void OnEnable()
    {
        button.onClick.AddListener(OpenCreatureInfoPage);
    }
    //public void Init(CreaturesPanel creaturesPanel,string id)
    //{
    //    this.creaturesPanel = creaturesPanel;
    //    this.creatureId = id;
    //}
    public void EnableButton(string id,bool value)
    {
        button.interactable = true;
        this.creatureId = id;

        var creatureList = GameManger.Instance.gameConfig.GetCreatureList(CreaturesPanel.Instance.activePanel);
        var creatureData = creatureList.Find(x => x.creatureId == creatureId);

        image.sprite = creatureData.sprite;
        image.color = Color.white;
        nameText.text = creatureData.name;
        SetNewUnlockState(value);
    }
    public void SetNewUnlockState(bool value)
    {
        newUnlockSticker.SetActive(value);
    }
    public void ShowShadowButton(Sprite creatureSprite)
    {
        button.interactable = false;
        
        image.sprite = creatureSprite;
        nameText.text = "??";
    }
    public void OpenCreatureInfoPage()
    {
        if (SaveLoadManager.Instance.CheckIfNewCreatureUnlocked(creatureId))
        {
            SetNewUnlockState(false);
            //refresh all stickers
            CreaturesPanel.Instance.RefreshAllStickers();
        }

        CreaturesPanel.Instance.ShowCreatureInfo(creatureId);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OpenCreatureInfoPage);

    }
}
