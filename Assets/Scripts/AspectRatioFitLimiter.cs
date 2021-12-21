using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(AspectRatioFitter))]
[RequireComponent(typeof(RectTransform))]
public class AspectRatioFitLimiter : MonoBehaviour
{
    [SerializeField]
    private Vector2 limiter;
    [SerializeField]
    private ScreenSizeNotifier screenSizeNotifier;
    private AspectRatioFitter _fitter;
    private RectTransform _rect;
    private bool _checksEnabled = false;
    private bool ChecksEnabled
    {
        get 
        {
            return _fitter.enabled && _checksEnabled;
        }
        set
        {
            _checksEnabled = value;
            _fitter.enabled = value;
        }
    }
    private void Awake()
    {
        _fitter = GetComponent<AspectRatioFitter>();
        _rect = GetComponent<RectTransform>();
        screenSizeNotifier.OnScreenChange += () => ChecksEnabled = true;
    }
    void LateUpdate()
    {
        if (ChecksEnabled)
        {
            if (_rect.sizeDelta.x < limiter.x || _rect.sizeDelta.y < limiter.y)
            {
                ChecksEnabled = false;
                _fitter.enabled = false;
                _rect.sizeDelta = new Vector2(Mathf.Max(_rect.sizeDelta.x, limiter.x), Mathf.Max(_rect.sizeDelta.y, limiter.y));
            }
        }
    }
}
