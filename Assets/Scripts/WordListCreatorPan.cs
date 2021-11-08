using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WordListCreatorPan : MonoBehaviour
{
    public BoardManager boardManager;
    public InputField[] inputFields;
    public Words workingWords;


    private string[] changeCheck = new string[16];

    private void Update()
    {
        // Tab through fields
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InputField selectedField = EventSystem.current.currentSelectedGameObject.GetComponent<InputField>();
            if (selectedField != null)
            {
                List<InputField> fields = new List<InputField>(inputFields);
                if (fields.Contains(selectedField))
                {
                    int nextIndex = fields.IndexOf(selectedField);
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        nextIndex = (nextIndex + fields.Count - 1) % fields.Count;
                    }
                    else
                    {
                        nextIndex = (nextIndex + 1) % fields.Count;
                    }
                    fields[nextIndex].Select();
                }
            }
        }
    }

    void OnEnable()
    {
        if (inputFields.Length < 1) return;
        int characterLimit = Mathf.Max(boardManager.columns, boardManager.rows) - 1;
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
            inputFields[i].characterLimit = characterLimit;
            changeCheck[i] = inputFields[i].text;
        }
        inputFields[0].Select();
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
