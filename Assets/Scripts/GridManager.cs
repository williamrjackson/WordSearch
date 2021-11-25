using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private float cellXScale = .1f;
    [SerializeField]
    private float cellYScale = 1f;
    [SerializeField]
    [Range(.01f, 1f)]
    private float landscapePercentage = .2f;
    [SerializeField]
    [Range(.01f, 1f)]
    private float portraitPercentage = .1f;
    private RectTransform rect;
    private GridLayoutGroup grid;
 
    void Update()
    {
        rect ??= GetComponent<RectTransform>();
        grid ??= GetComponent<GridLayoutGroup>();

        Rect r = rect.rect;
        Vector2 canvasSize = canvas.pixelRect.size;

        if (canvasSize.x < canvasSize.y)
        {
            r.height = canvasSize.y * portraitPercentage;
        }
        else
        {
            r.height = canvasSize.y * landscapePercentage;
        }
        
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, r.size.y);
        float width = r.width / 4f;
        float height = r.height / 4f;

        grid.cellSize = new Vector2(width * cellYScale, height * cellXScale);
    }
}
