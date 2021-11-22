using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Curtain : MonoBehaviour
{
    [SerializeField]
    private bool shownOnStart = false;
    [SerializeField] [Range(0f, 2f)]
    private float duration = 1f;
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private string[] superlatives;
    private Image m_CurtainPanel;
    private bool m_IsVisible = false;

    public string Text
    {
        set
        {
            _text.SetText(value);
        }
        get => _text.text;
    }
    void Awake()
    {

        m_CurtainPanel = transform.EnsureComponent<Image>();
        if (shownOnStart)
        {
            m_IsVisible = true;
            canvasGroup.alpha = 1f;
        }
    }

    public void RandomSuperlative()
    {
        Text = superlatives.GetRandom();
    }
    public bool IsVisible
    {
        set
        {
            if (m_IsVisible == value) return;

            m_IsVisible = value;
            if (m_IsVisible)
            {
                m_CurtainPanel.raycastTarget = true;
                // m_CurtainPanel.transform.Alpha(1f, duration);
                StartCoroutine(FadeIn(duration));
            }
            else
            {
                m_CurtainPanel.raycastTarget = false;
                // m_CurtainPanel.transform.Alpha(0f, duration);
                StartCoroutine(FadeOut(duration));
            }
        }
        get
        {
            return m_IsVisible;
        }
    }
    public float Duration => duration;

    private IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.InverseLerp(0f, duration, elapsedTime);
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = 1f;
    }
    private IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.InverseLerp(duration, 0f, elapsedTime);
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.alpha = 0f;
        Text = "";
    }
}
