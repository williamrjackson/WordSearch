using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BoardSizeSetter : MonoBehaviour
{
    [SerializeField]
    RectTransform parentRect;
    [SerializeField]
    RawImage rawImage;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fit();
        }
    }
    public void Fit()
    {
        float aspectRatio = rawImage.texture.width / rawImage.texture.height;
        var fitter = GetComponent<UnityEngine.UI.AspectRatioFitter>();
        fitter.aspectRatio = aspectRatio;
    }
}
