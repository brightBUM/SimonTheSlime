using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class PageSnapScroll : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public enum ScrollDirection { Horizontal, Vertical }
    [Header("Setup")]
    public ScrollRect scrollRect;
    public Transform content;
    public ScrollDirection scrollDirection = ScrollDirection.Horizontal;
    public int itemsPerPage = 1;
    public int startPageNum = 0;
    public Action<int> OnPageMoved;
    public Action SnapToStartComplete;
    private int totalItems;
    private int totalPages;
    private float[] pagePositions;
    private float dragStartPos;
    private int pageNum;
    private bool shifting;

    public void Init()
    {
        totalItems = content.childCount;
        totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);
        pagePositions = new float[totalPages];

        float itemSize, spacing, viewSize, pageSize, totalContentSize, scrollableSize;

        if (scrollDirection == ScrollDirection.Horizontal)
        {
            var layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
            var item = content.GetChild(0) as RectTransform;
            itemSize = item.rect.width;
            spacing = layoutGroup.spacing;
            viewSize = scrollRect.viewport.rect.width;
        }
        else
        {
            var layoutGroup = content.GetComponent<VerticalLayoutGroup>();
            var item = content.GetChild(0) as RectTransform;
            itemSize = item.rect.height;
            spacing = layoutGroup.spacing;
            viewSize = scrollRect.viewport.rect.height;
        }

        pageSize = (itemSize + spacing) * itemsPerPage;
        totalContentSize = (itemSize + spacing) * totalItems - spacing;
        scrollableSize = totalContentSize - viewSize;

        for (int i = 0; i < totalPages; i++)
        {
            float pageCenter = ((itemSize + spacing) * itemsPerPage * i) + (pageSize / 2f) - (viewSize / 2f);
            float normalized = scrollableSize <= 0 ? 0 : pageCenter / scrollableSize;
            pagePositions[i] = Mathf.Clamp01(normalized);
        }

        StartCoroutine(SnapToStartAfterLayout());
    }

    private IEnumerator SnapToStartAfterLayout()
    {
        yield return null;

        if (scrollDirection == ScrollDirection.Horizontal)
            scrollRect.horizontalNormalizedPosition = pagePositions[startPageNum];
        else
            scrollRect.verticalNormalizedPosition = pagePositions[startPageNum];

        //fire snaptostart event complete
        SnapToStartComplete.Invoke();

}

public void OnEndDrag(PointerEventData eventData)
    {
        if (shifting) return;

        float dragEndPos = (scrollDirection == ScrollDirection.Horizontal) ?
            scrollRect.horizontalNormalizedPosition : scrollRect.verticalNormalizedPosition;

        float swipeDelta = dragStartPos - dragEndPos;
        float threshold = 0.05f;

        int currentPage = 0;
        float closest = float.MaxValue;

        for (int i = 0; i < pagePositions.Length; i++)
        {
            float dist = Mathf.Abs(dragStartPos - pagePositions[i]);
            if (dist < closest)
            {
                closest = dist;
                currentPage = i;
            }
        }

        int targetPage = currentPage;
        if (swipeDelta > threshold && currentPage > 0)
            targetPage = currentPage - 1;
        else if (swipeDelta < -threshold && currentPage < totalPages - 1)
            targetPage = currentPage + 1;

        pageNum = targetPage;

        StopAllCoroutines();
        MoveToPage(targetPage);
    }

    public void NextPage()
    {
        if (shifting) return;
        pageNum = Mathf.Clamp(pageNum + 1, 0, pagePositions.Length - 1);
        MoveToPage(pageNum);
    }

    public void PrevPage()
    {
        if (shifting) return;
        pageNum = Mathf.Clamp(pageNum - 1, 0, pagePositions.Length - 1);
        MoveToPage(pageNum);
    }

    public void MoveToPage(int num)
    {
        OnPageMoved?.Invoke(num);
        StartCoroutine(SmoothScrollTo(pagePositions[num]));
    }

    private IEnumerator SmoothScrollTo(float target)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        float start = (scrollDirection == ScrollDirection.Horizontal) ?
            scrollRect.horizontalNormalizedPosition : scrollRect.verticalNormalizedPosition;

        shifting = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Lerp(start, target, elapsed / duration);

            if (scrollDirection == ScrollDirection.Horizontal)
                scrollRect.horizontalNormalizedPosition = t;
            else
                scrollRect.verticalNormalizedPosition = t;

            yield return null;
        }

        shifting = false;

        if (scrollDirection == ScrollDirection.Horizontal)
            scrollRect.horizontalNormalizedPosition = target;
        else
            scrollRect.verticalNormalizedPosition = target;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = (scrollDirection == ScrollDirection.Horizontal) ?
            scrollRect.horizontalNormalizedPosition : scrollRect.verticalNormalizedPosition;
    }
}
