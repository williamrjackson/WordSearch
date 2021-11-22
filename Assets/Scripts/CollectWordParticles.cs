using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrj;

public class CollectWordParticles : MonoBehaviour
{
    private ParticleSystem _particles;
    private Coroutine _collectRoutine;
    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
        _particles.Stop();
    }

    public void CollectWord(Words.Word word)
    {
        if (_collectRoutine != null)
        {
            StopCoroutine(_collectRoutine);
        }
        _collectRoutine = StartCoroutine(PerformAnim(word));
    }

        public IEnumerator PerformAnim(Words.Word word)
    {
        // Debug.Log($"Hint for {word.word}");
        _particles.Stop();
        _particles.transform.position = word.StartUnit.hintTargetPosition.position;
        float duration = .05f * word.letterUnits.Length;
        Vector3 targetPos = word.EndUnit.hintTargetPosition.position;
        yield return new WaitForEndOfFrame();
        _particles.Play();
        yield return new WaitForEndOfFrame();
        yield return Utils.MapToCurve.Ease.MoveWorld(_particles.transform, targetPos, duration).coroutine;
        _particles.Stop();
        _collectRoutine = null;
    }
}
