using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Curtain : MonoBehaviour
{
    [SerializeField]
    private bool shownOnStart = false;
    [SerializeField] [Range(0f, 2f)]
    private float duration = 1f;

    private Image m_CurtainPanel;
    private bool m_IsVisible = false;

    void Awake()
    {

        m_CurtainPanel = transform.EnsureComponent<Image>();
        if (shownOnStart)
        {
            m_IsVisible = true;
            m_CurtainPanel.transform.Alpha(1f, 0f);
        }
    }

    public bool IsVisible
    {
        set
        {
            if (m_IsVisible == value) return;

            m_IsVisible = value;
            if (m_IsVisible)
            {
                m_CurtainPanel.transform.Alpha(1f, duration);
            }
            else
            {
                m_CurtainPanel.transform.Alpha(0f, duration);
            }
        }
        get
        {
            return m_IsVisible;
        }
    }
    public float Duration => duration;
}
