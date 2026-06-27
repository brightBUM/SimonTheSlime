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

    public CreatureReveal creatureReveal;
    public int activePanel; //which tab is currently show - common,rare,epic collections
    public static CreaturesPanel Instance;
    [Header("New Unlock Sticker")]
    [SerializeField] GameObject mainPanelSticker;
    [SerializeField] GameObject[] tabStickers;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        TogglePanel(0); //show common by default
        RefreshAllStickers(); //show new sticker states based on unlock state
    }
    //Called by button tabs click 
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
            var creatureUnlockState = SaveLoadManager.Instance.GetCreatureUnlockState(creatureListData[i].creatureId);
            if (creatureUnlockState !=CreatureUnlockStateType.Locked)
            {
                bool isNewUnlock = creatureUnlockState == CreatureUnlockStateType.UnlockedNew;
                buttons[i].EnableButton(creatureListData[i].creatureId,isNewUnlock);
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
    public void RefreshAllStickers()
    {
        bool anyNewCreature = false;
        for (int i=0;i<3;i++)
        {
            bool tabHasNewCreature = false;
            var creatureListData = GameManger.Instance.gameConfig.GetCreatureList(i);
            for (int j = 0; j < creatureListData.Count; j++)
            {
                var creatureUnlockState = SaveLoadManager.Instance.GetCreatureUnlockState(creatureListData[j].creatureId);
                if(creatureUnlockState == CreatureUnlockStateType.UnlockedNew)
                {
                    //if any one the creature buttons in a tab has a new sticker
                    tabHasNewCreature = true;
                    anyNewCreature = true;
                    break;
                }
            }
            tabStickers[i].SetActive(tabHasNewCreature); 
        }

        mainPanelSticker.SetActive(anyNewCreature); //i.e if any one of the tabs has a new sticker

    }
}

[System.Serializable]
public class CreatureUnlockState
{
    public string id;
    public CreatureUnlockStateType creatureUnlockStateType;
    public CreatureUnlockState(string id, CreatureUnlockStateType creatureUnlockStateType)
    {
        this.id = id;
        this.creatureUnlockStateType = creatureUnlockStateType;
    }
}
public enum CreatureUnlockStateType
{
    Locked = 0,
    UnlockedNew = 1,
    UnlockedSeen = 2
}