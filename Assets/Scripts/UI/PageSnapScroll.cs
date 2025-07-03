using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

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
        pagePositions = new float[totalPages];

        var layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
        var item = content.GetChild(0) as RectTransform;
        float itemWidth = item.rect.width;
        float spacing = layoutGroup.spacing;

        float viewWidth = scrollRect.viewport.rect.width;
        float pageWidth = (itemWidth + spacing) * itemsPerPage;

        float totalContentWidth = (itemWidth + spacing) * totalItems - spacing;
        float scrollableWidth = totalContentWidth - viewWidth;

        //Debug.Log($" Pagepositions : {pagePositions[i]}");

        for (int i = 0; i < totalPages; i++)
        {
            // Calculate center of each page
            float pageCenter = ((itemWidth + spacing) * itemsPerPage * i) + (pageWidth / 2f) - (viewWidth / 2f);
            float normalized = scrollableWidth <= 0 ? 0 : pageCenter / scrollableWidth;
            pagePositions[i] = Mathf.Clamp01(normalized);
        }

        StartCoroutine(SnapToStartAfterLayout());
    }
    private System.Collections.IEnumerator SnapToStartAfterLayout()
    {
        // Wait one frame for layout to finish
        yield return null;

        // Now set position immediately (no smooth scroll)
        scrollRect.horizontalNormalizedPosition = pagePositions[0];
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

                //Debug.Log($"closest : {closest} , dist : {dist} , currentPage : {currentPage}");
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
        MoveTopage(targetPage);
    }
    public void MoveTopage(int num)
    {
        num = Mathf.Clamp(num, 0, pagePositions.Length-1);
        StartCoroutine(SmoothScrollTo(pagePositions[num]));
    }
    System.Collections.IEnumerator SmoothScrollTo(float target)
    {
        //Debug.Log("Scroll rect HNP target: " + target);

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
