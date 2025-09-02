using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturesPanel : MonoBehaviour
{
    [SerializeField] GameObject commonPanel;
    [SerializeField] GameObject rarePanel;
    [SerializeField] GameObject epicPanel;

    

    public void TogglePanel(int index)
    {
        var panels = new List<GameObject>() { commonPanel, rarePanel, epicPanel };

        foreach (GameObject go in panels)
        {
            go.SetActive(false);
        }

        panels[index].SetActive(true);
    }
}
