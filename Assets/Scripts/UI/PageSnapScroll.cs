using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PageSnapScroll : MonoBehaviour,IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public Transform content;
    public int itemsPerPage = 5;

    private int totalItems;
    private int totalPages;
    private float[] pagePositions;
    private float dragStartPos;

    void Start()
    {
        
    }
    public void Init()
    {
        totalItems = content.childCount;
        totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);
        Debug.Log("total pages : " + totalPages);
        pagePositions = new float[totalPages];

        for (int i = 0; i < totalPages; i++)
        {
            pagePositions[i] = (float)i / (totalPages - 1);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        float dragEndPos = scrollRect.horizontalNormalizedPosition;
        float swipeDelta = dragStartPos-dragEndPos;
        float threshold = 0.05f; // Adjust this value to make swipes more or less sensitive

        int currentPage = 0;
        float closest = float.MaxValue;

        // Find current page
        for (int i = 0; i < pagePositions.Length; i++)
        {
            float dist = Mathf.Abs(dragStartPos - pagePositions[i]);
            if (dist < closest)
            {
                closest = dist;
                currentPage = i;
            }
        }

        // Determine next page based on swipe
        int targetPage = currentPage;
        if (swipeDelta > threshold && currentPage > 0)
        {
            targetPage = currentPage - 1; // swipe right (previous page)
        }
        else if (swipeDelta < -threshold && currentPage < totalPages - 1)
        {
            targetPage = currentPage + 1; // swipe left (next page)
        }

        StopAllCoroutines();
        StartCoroutine(SmoothScrollTo(pagePositions[targetPage]));
    }

    System.Collections.IEnumerator SmoothScrollTo(float target)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        float start = scrollRect.horizontalNormalizedPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = target;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = scrollRect.horizontalNormalizedPosition;
    }
}
