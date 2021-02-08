using UnityEngine;

[CreateAssetMenu(fileName = "WordList", menuName = "ScriptableObjects/WordList", order = 1)]
public class Words : ScriptableObject
{
    public Word[] words;

    [System.Serializable]
    public class Word
    {
        public string word;
        [HideInInspector]
        public LetterUnit start;
        [HideInInspector]
        public LetterUnit end;
        [HideInInspector]
        public bool isFound = false;
        public void SetRange(LetterUnit start, LetterUnit end)
        {
            //Debug.Log("Setting " + word + " to " + start.Letter + " - " + end.Letter);
            this.start = start;
            this.end = end;
        }
    }
}
