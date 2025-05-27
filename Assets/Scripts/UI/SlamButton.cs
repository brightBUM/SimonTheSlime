using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlamButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool hold = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        hold = true;
        StartCoroutine(WhileTouching());
    }
    private IEnumerator WhileTouching()
    {
        while (hold)
        {
            GamePlayScreenUI.Instance.poundAbilityAction.Invoke();
            yield return null;
        }

        GamePlayScreenUI.Instance.poundReleaseAction.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        hold = false;
    }

    
}
