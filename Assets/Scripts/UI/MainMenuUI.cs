using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;
    [SerializeField] Transform contentParent;
    [SerializeField] GameObject registrationPanel;
    [SerializeField] TMP_InputField nameField;
    [SerializeField] TMP_InputField ageField;

    private void OnEnable()
    {
        //check if game loaded for first time
        if(SaveLoadManager.Instance.firstLoad)
        {
            //show profile registration page
            registrationPanel.SetActive(true);

        }
    }

    public void ActivatePanel(int index)
    {
        foreach(Transform child in contentParent)
        {
            child.gameObject.SetActive(false);
        }

        panels[index].SetActive(true);
    }

    public void SaveProfileInfo()
    {
        if(Int32.TryParse(ageField.text, out int age))
        {
            SaveLoadManager.Instance.SaveProfileInfo(nameField.text, age);

        }
        else
        {
            Debug.Log("int parse failed");
        }
    }
}
