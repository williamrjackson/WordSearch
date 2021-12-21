using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WordReference : MonoBehaviour
{
    public List<TextMeshProUGUI> textList;
    public ScreenSizeNotifier screenSizeNotifier;
    private Coroutine _updateSizeRoutine;

    private void Start()
    {
        screenSizeNotifier.OnScreenChange += UpdateSizes;
    }

    public void Populate(Words wordlist)
    {
        for (int i = 0; i < textList.Count; i++)
        {
            HintBehavior hintBehavior = textList[i].EnsureComponent<HintBehavior>();
            textList[i].fontStyle = FontStyles.Normal;
            textList[i].faceColor = Color.white;
            if (i < wordlist.words.Length)
            {
                textList[i].text = wordlist.words[i].word;
                hintBehavior.word = wordlist.words[i];
            }
            else
            {
                textList[i].text = string.Empty;
            }
        }
        UpdateSizes();
    }
    public void Strike(string word)
    {
        foreach (TextMeshProUGUI item in textList)
        {
            if (item.text.ToLower() == word.ToLower())
            {
                item.fontStyle = FontStyles.Strikethrough;
                item.faceColor = Color.gray;
            }
        }
    }

    public void UpdateSizes()
    {
        if (_updateSizeRoutine != null)
        {
            StopCoroutine(_updateSizeRoutine);
        }
        _updateSizeRoutine = StartCoroutine(SizeRoutine());
    }
    private IEnumerator SizeRoutine()
    {
        Debug.Log("UpdateSizes");
        yield return new WaitForEndOfFrame();
        float smallestTextSize = float.MaxValue;
        for (int i = 0; i < textList.Count; i++)
        {
            textList[i].enableAutoSizing = true;
        }
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < textList.Count; i++)
        {
            if (string.IsNullOrEmpty(textList[i].text)) continue;
            
            smallestTextSize = Mathf.Min(smallestTextSize, textList[i].fontSize);
            Debug.Log(smallestTextSize);
        }
        for (int i = 0; i < textList.Count; i++)
        {
            textList[i].enableAutoSizing = false;
            textList[i].fontSize = smallestTextSize;
        }
        _updateSizeRoutine = null;
    }
private void AlertHintClick(string word)
    {
        Debug.Log($"TEST: {word}");
    }
}
