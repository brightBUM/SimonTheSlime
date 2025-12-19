using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeDetection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Vector2 startPos, endPos;
    [SerializeField] private float swipeThreshold = 50f; // min distance in px
    private bool shifting;
    public Action<Action> OnOpenPanel;
    public Action<Action> OnClosePanel;
    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        endPos = eventData.position;
        float deltaX = endPos.x - startPos.x;
        if (Mathf.Abs(deltaX) > swipeThreshold && !shifting)
        {
            shifting = true;
            if (deltaX > 0)
                OnClosePanel?.Invoke(() => shifting = false);
            else
                OnOpenPanel?.Invoke(() => shifting = false);
        }
    }

    public void CloseButton()
    {
        OnClosePanel?.Invoke(() => shifting = false);
    }
}
