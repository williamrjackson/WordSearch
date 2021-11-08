using System.Collections;
using UnityEngine;

public class TargetFramerate : MonoBehaviour
{
    [SerializeField]
    private int targetFrameRate = -1;
    [SerializeField]
    private bool logFrameRateInEditor = false;

    void Awake()
    {
        if (targetFrameRate > 0)
        {
            QualitySettings.vSyncCount = 0;  // VSync must be disabled
            Application.targetFrameRate = targetFrameRate;
        }
#if UNITY_EDITOR
        if (logFrameRateInEditor)
        {
            StartCoroutine(PrintFrameRate());
        }
#endif
        Wrj.Utils.SupressUnusedVarWarning(logFrameRateInEditor);
    }

    private IEnumerator PrintFrameRate()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log($"FPS: {1f / Time.deltaTime}");
        }
    }
}
