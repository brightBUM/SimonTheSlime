using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreaturesPanel : MonoBehaviour
{
    [SerializeField] GameObject commonPanel;
    [SerializeField] GameObject rarePanel;
    [SerializeField] GameObject epicPanel;
    [SerializeField] Button[] commonButtons;
    [SerializeField] Button[] rareButtons;
    [SerializeField] Button[] epicButtons;
    

    private void Start()
    {
        //populate the creature pages based on saveload data
    }
    public void TogglePanel(int index)
    {
        var panels = new List<GameObject>() { commonPanel, rarePanel, epicPanel };

        foreach (GameObject go in panels)
        {
            go.SetActive(false);
        }

        panels[index].SetActive(true);
    }

    public void GetCreaturesDataByType(int creatureType)
    {
        //unlock map
        var unlockMap = new Dictionary<string, bool>();
        foreach(var item in SaveLoadManager.Instance.playerProfile.creatureUnlockStates)
        {
            unlockMap[item.id] = item.acquired;
        }
        
        //show only unlocked one's
        var commonData = GameManger.Instance.gameConfig.commonData;
        for(int i=0; i< commonData.Count; i++)
        {
            var acquired = unlockMap[commonData[i].creatureId];   
            commonButtons[i].interactable = acquired;
            commonButtons[i].image.sprite = commonData[i].sprite;
            commonButtons[i].image.color = Color.white;
        }

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
