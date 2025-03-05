using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;
    [SerializeField] Transform contentParent;
    
    public void ActivatePanel(int index)
    {
        foreach(Transform child in contentParent)
        {
            child.gameObject.SetActive(false);
        }

        panels[index].SetActive(true);
    }
}
