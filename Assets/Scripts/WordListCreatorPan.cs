using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordListCreatorPan : MonoBehaviour
{
    public BoardManager boardManager;
    public InputField[] inputFields;
    public Words workingWords;

    private string[] changeCheck = new string[16];
    void OnEnable()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (boardManager.CurrentWordList.words.Length > i)
            {
                inputFields[i].text = boardManager.CurrentWordList.words[i].word;
            }
            else
            {
                inputFields[i].text = string.Empty;
            }
            changeCheck[i] = inputFields[i].text;
        }
    }
    void OnDisable()
    {
        bool shouldRefreshWords = false;
        for (int i = 0; i < inputFields.Length; i++)
        {
            workingWords.words[i].word = inputFields[i].text;
            if (inputFields[i].text.ToLower() != changeCheck[i].ToLower())
                shouldRefreshWords = true;
        }

        if (shouldRefreshWords)
            boardManager.CurrentWordList = workingWords;
    }

}
