using UnityEngine;

public class RemapOrthographicSize : MonoBehaviour
{
    [SerializeField]
    bool _enableLogging = false;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private float _aspectMin = 0f;
    [SerializeField]
    private float _aspectMax = 2f;
    [SerializeField]
    private float _sizeMin = 8f;
    [SerializeField]
    private float _sizeMax = 5.75f;

    private float _lastRatio = 0f;

    private void Awake()
    {
        _camera ??= Camera.main;
    }

    void Update()
    {
        CheckForScreenUpdate();
    }

    private void CheckForScreenUpdate()
    {
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        if (aspectRatio != _lastRatio)
        {
            _lastRatio = aspectRatio;
            Log($"Ratio = {_lastRatio}");
            UpdateOrthographicSize();
        }
    }

    void UpdateOrthographicSize()
    {
        float size = Mathf.Lerp(_sizeMin, _sizeMax, Mathf.InverseLerp((float)_aspectMin, (float)_aspectMax, _lastRatio));
        Log($"Updating Size: {size}");   
        _camera.orthographicSize = size;
    }

    private void OnValidate()
    {
        UpdateOrthographicSize();
    }

    private void Log(string msg)
    {
        if (_enableLogging)
            Debug.Log(msg);
    }
}
