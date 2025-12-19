using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureRecoveredUI : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI nameText;
    CreaturesPanel creaturesPanel;
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
    public void EnableButton(CreaturesPanel creaturesPanel, string id)
    {
        button.interactable = true;
        this.creaturesPanel = creaturesPanel;
        this.creatureId = id;

        var creatureList = GameManger.Instance.gameConfig.GetCreatureList(creaturesPanel.activePanel);
        var creatureData = creatureList.Find(x => x.creatureId == creatureId);

        image.sprite = creatureData.sprite;
        image.color = Color.white;
        nameText.text = creatureData.name;

    }
    public void ShowShadowButton(Sprite creatureSprite)
    {
        button.interactable = false;
        
        image.sprite = creatureSprite;
        nameText.text = "??";
    }
    public void OpenCreatureInfoPage()
    {
        creaturesPanel.ShowCreatureInfo(creatureId);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OpenCreatureInfoPage);

    }
}
