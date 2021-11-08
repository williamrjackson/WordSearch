using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WordReference : MonoBehaviour
{
    public List<TextMeshProUGUI> textList;

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
    private void AlertHintClick(string word)
    {
        Debug.Log($"TEST: {word}");
    }
}
