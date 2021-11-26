using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatioMaintainer : MonoBehaviour
{
    [SerializeField]
    [Range(0.01f, 1f)]
    float verticalRatio = 1f;
    [SerializeField]
    [Range(0.0f, 1f)]
    float verticalPosRatio = 1f;
    [SerializeField]
    bool anchorTop = false;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    ScreenSizeNotifier screenSizeNotifier;

    private void Start()
    {
        screenSizeNotifier.OnScreenChange += PerformUpdate;
    }
    private void PerformUpdate()
    {
        float posY = (anchorTop) ? -verticalPosRatio : verticalPosRatio;
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, canvas.pixelRect.height * posY);
        rect.sizeDelta = new Vector2( rect.sizeDelta.x, canvas.pixelRect.height * verticalRatio);
    }
    private void OnValidate()
    {
        PerformUpdate();
    }
}
