using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreaturesPanel : MonoBehaviour
{
    [SerializeField] GameObject commonPanel;
    [SerializeField] GameObject rarePanel;
    [SerializeField] GameObject epicPanel;
    [SerializeField] List<CreatureRecoveredUI> commonButtons;
    [SerializeField] List<CreatureRecoveredUI> rareButtons;
    [SerializeField] List<CreatureRecoveredUI> epicButtons;
    [SerializeField] CreatureInfoPanel creatureInfoPanel;
    Dictionary<string, bool> unlockMap;
    public int activePanel; //which tab is currently show - common,rare,epic collections
    private void Start()
    {
        //populate the creature pages based on saveload data
        unlockMap = new Dictionary<string, bool>();
        foreach (var item in SaveLoadManager.Instance.playerProfile.creatureUnlockStates)
        {
            unlockMap[item.id] = item.acquired;
        }
        TogglePanel(0); //show common by default
    }
    public void TogglePanel(int index)
    {
        activePanel = index;
        var panels = new List<GameObject>() { commonPanel, rarePanel, epicPanel };

        foreach (GameObject go in panels)
        {
            go.SetActive(false);
        }
        ShowCreaturesUnlockedByType(index);
        panels[index].SetActive(true);
    }

    public void ShowCreaturesUnlockedByType(int creatureType)
    {
        CreatureType creature = (CreatureType)creatureType;
        //show only unlocked one's
        switch(creature)
        {
            case CreatureType.Common:
                ActivateButtons(commonButtons);
                break;
            case CreatureType.Rare:
                ActivateButtons(rareButtons);
                break;
            case CreatureType.Epic:
                ActivateButtons(epicButtons);
                break;
            default: 
                break;
        }

    }
    public void ActivateButtons(List<CreatureRecoveredUI> buttons)
    {
        var creatureListData = GameManger.Instance.gameConfig.GetCreatureList(activePanel);

        for (int i = 0; i < creatureListData.Count; i++)
        {
            if (unlockMap[creatureListData[i].creatureId])
            {
                buttons[i].EnableButton(this, creatureListData[i].creatureId);
            }
            else
            {
                buttons[i].ShowShadowButton(creatureListData[i].sprite);
            }
            
        }
    }
    public void ShowCreatureInfo(string creatureId)
    {

        var creatureList = GameManger.Instance.gameConfig.GetCreatureList(activePanel);
        var creatureData = creatureList.Find(x => x.creatureId == creatureId);
        creatureInfoPanel.SetInfoData(creatureData);

        creatureInfoPanel.gameObject.SetActive(true);   
    }
}

[System.Serializable]
public class CreatureUnlockState
{
    public string id;
    public bool acquired;
    public CreatureUnlockState(string id, bool acquired)
    {
        this.id = id;
        this.acquired = acquired;
    }
}
