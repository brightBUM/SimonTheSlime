using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGrid : MonoBehaviour
{
    public float cellWidth = 500f;
    public float cellHeight = 425f;
    public int columns = 3;
    public int rows = 2;
    
    GridLayoutGroup grid;
    RectTransform rt;
    void Start()
    {
        grid = GetComponent<GridLayoutGroup>();
        rt = GetComponent<RectTransform>();

        float screenWidth = rt.rect.width;
        float screenHeight = rt.rect.height;

        float spacingX = (screenWidth - (columns * cellWidth)) / (columns - 1);
        float spacingY = (screenHeight - (rows * cellHeight)) / (rows - 1);

        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.spacing = new Vector2(spacingX, spacingY);

        grid.padding = new RectOffset(0, 0, 0, 0); // optional — spacing handles the centering
        grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        grid.constraintCount = rows;

        var cont = gameObject.AddComponent<ContentSizeFitter>();
        cont.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
    private void Update()
    {
        

       
    }
}
