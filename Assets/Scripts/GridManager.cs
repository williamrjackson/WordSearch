using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private float cellXScale = .1f;
    [SerializeField]
    private float cellYScale = 1f;
    private RectTransform rect;
    private GridLayoutGroup grid;
    void Update()
    {
        rect ??= GetComponent<RectTransform>();
        grid ??= GetComponent<GridLayoutGroup>();

        float width = rect.rect.width / 4f;
        grid.cellSize = new Vector2(width * cellYScale, width * cellXScale);
    }
}
