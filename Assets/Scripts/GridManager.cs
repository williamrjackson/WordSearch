using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    void Update()
    {
        RectTransform rect = GetComponent<RectTransform>();
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();

        float width = rect.rect.width / 4f;
        grid.cellSize = new Vector2(width, width * .1f);
    }
}
