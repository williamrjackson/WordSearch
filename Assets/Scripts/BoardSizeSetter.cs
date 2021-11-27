using UnityEngine.UI;
using UnityEngine;

public class BoardSizeSetter : MonoBehaviour
{
    [SerializeField]
    RectTransform parentRect;
    [SerializeField]
    RawImage rawImage;

    public static BoardSizeSetter Instance;
    void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple BoardSizeSetter's instantiated. Component removed from " + gameObject.name + ". Instance already found on " + Instance.gameObject.name + "!");
            Destroy(this);
        }
    }
    public void Fit()
    {
        float aspectRatio = rawImage.texture.width / rawImage.texture.height;
        var fitter = GetComponent<UnityEngine.UI.AspectRatioFitter>();
        fitter.aspectRatio = aspectRatio;
    }
}
