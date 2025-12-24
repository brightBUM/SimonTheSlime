using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ScrollPageSizer : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    void OnEnable() => Apply();
    void OnRectTransformDimensionsChange() => Apply();

    public void Apply()
    {
        if (!scrollRect) return;
        var viewport = scrollRect.viewport;
        var content = scrollRect.content;
        if (!viewport || !content) return;

        float w = viewport.rect.width;
        float h = viewport.rect.height;

        for (int i = 0; i < content.childCount; i++)
        {
            var child = content.GetChild(i) as RectTransform;
            if (!child) continue;

            // Make it fill viewport
            child.anchorMin = new Vector2(0, 1);
            child.anchorMax = new Vector2(1, 1);
            child.pivot = new Vector2(0.5f, 1);

            child.offsetMin = new Vector2(0, -h); // left, bottom
            child.offsetMax = new Vector2(0, 0);  // right, top
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }
}
