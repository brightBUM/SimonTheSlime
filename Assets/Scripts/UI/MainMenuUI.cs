using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject shopPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void EnableShopPanel()
    {
        if(mainMenuPanel.activeInHierarchy)
            mainMenuPanel.SetActive(false);

        shopPanel.SetActive(true);
    }
    public void EnableMainMenuPanel()
    {
        if (shopPanel.activeInHierarchy)
            shopPanel.SetActive(false);

        mainMenuPanel.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
