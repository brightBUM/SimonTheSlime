using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlamButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool hold = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        //check to prevent button tap during tutorial 
        if (LevelManager.Instance.IsTutorialActive)
            return;

        hold = true;
        StartCoroutine(WhileTouching());
    }
    private IEnumerator WhileTouching()
    {
        if (LevelManager.Instance.IsTutorialActive)
            yield return null;

        while (hold)
        {
            GamePlayScreenUI.Instance.poundAbilityAction.Invoke();
            yield return null;
        }

        GamePlayScreenUI.Instance.poundReleaseAction.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (LevelManager.Instance.IsTutorialActive)
            return;

        hold = false;
    }

    
}
