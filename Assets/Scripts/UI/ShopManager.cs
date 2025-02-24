using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] Transform[] buttonsTabs;
    [SerializeField] Transform[] ScreenTabs;
    // Start is called before the first frame update
    

    public void SelectPage(int index)
    {
        for (int i = 0; i < buttonsTabs.Length; i++)
        {
            if(i==index)
            {
                buttonsTabs[i].DOScale(Vector3.one, 0.1f).SetEase(Ease.OutFlash).SetUpdate(true);
                ScreenTabs[i].gameObject.SetActive(true);
            }
            else
            {
                buttonsTabs[i].localScale = Vector3.zero;
                ScreenTabs[i].gameObject.SetActive(false);
            }
        }
    }
        
    
}
