using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordReference : MonoBehaviour
{
    public List<TextMeshPro> textList;
    // Update is called once per frame
    public void Populate(Words wordlist)
    {
        for (int i = 0; i < textList.Count; i++)
        {
            textList[i].fontStyle = FontStyles.Normal;
            textList[i].faceColor = Color.white;
            if (i < wordlist.words.Length - 1)
            {
                textList[i].text = wordlist.words[i].word;
            }
            else
            {
                textList[i].text = string.Empty;
            }
        }
    }
    public void Strike(string word)
    {
        foreach (TextMeshPro item in textList)
        {
            if (item.text.ToLower() == word.ToLower())
            {
                item.fontStyle = FontStyles.Strikethrough;
                item.faceColor = Color.gray;
            }
        }
    }
}
