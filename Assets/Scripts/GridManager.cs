using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    private RectTransform rect;
    private GridLayoutGroup grid;
 
    void Update()
    {
        rect ??= GetComponent<RectTransform>();
        grid ??= GetComponent<GridLayoutGroup>();
        float width = rect.rect.width / 4f;
        float height = rect.rect.height / 4f;

        grid.cellSize = new Vector2(width * .95f, height * .95f);
    }
}
