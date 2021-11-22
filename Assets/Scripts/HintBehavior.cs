using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wrj;

public class HintBehavior : MonoBehaviour
{
    [SerializeField]
    private TrailRenderer _trail;
    [HideInInspector]
    public Words.Word word;
    private Button _button;
    private BoardManager _board;
    private static Coroutine hintCoroutine;
    private static bool _allowHints = true;
    public static bool AllowHints 
    {
        get => _allowHints;
        set
        {
            // Debug.Log("Setting AllowHints to " + value);
            _allowHints = value;
        }
    }
    private void OnEnable()
    {
        _board ??= FindObjectOfType<BoardManager>();
        _trail ??= FindObjectOfType<TrailRenderer>();
        _button = gameObject.EnsureComponent<Button>();
        _button.onClick.AddListener(() => 
        {
            if (hintCoroutine == null)
            {
                hintCoroutine = StartCoroutine(PerformHint());
            }
        });
    }
    public IEnumerator PerformHint()
    {
        if (!_allowHints || _button == null || word == null || _trail == null)
            yield break;
        // Debug.Log($"Hint for {word.word}");
        _trail.enabled = false;
        _trail.transform.position = word.StartUnit.hintTargetPosition.position;
        float duration = .025f * word.letterUnits.Length;
        Vector3 targetPos = word.EndUnit.hintTargetPosition.position;
        _trail.enabled = true;
        yield return new WaitForEndOfFrame();
        yield return Utils.MapToCurve.Ease.MoveWorld(_trail.transform, targetPos, duration).coroutine;
        hintCoroutine = null;
    }

}
