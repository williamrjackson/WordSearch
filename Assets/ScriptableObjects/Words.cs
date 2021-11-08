using UnityEngine;

[CreateAssetMenu(fileName = "WordList", menuName = "ScriptableObjects/WordList", order = 1)]
public class Words : ScriptableObject
{
    public Word[] words = new Word[16];

    [System.Serializable]
    public class Word
    {
        public string word;
        [HideInInspector]
        public LetterUnit[] letterUnits;
        [HideInInspector]
        public bool isFound = false;
        public LetterUnit StartUnit
        {
            get
            {
                if (letterUnits.Length < 1) return null;
                return letterUnits[0];
            }
        }
        public LetterUnit EndUnit
        {
            get
            {
                if (letterUnits.Length < 1) return null;
                return letterUnits[letterUnits.Length - 1];
            }
        }
        public string treatedWord
        {
            get 
            {
                string s = word.ToUpper();
                string result = string.Empty;
                for (int i = 0; i < s.Length; i++)
                {
                    if (LetterUnit.Alphabet.Contains(s[i].ToString()))
                    {
                        result += s[i].ToString();
                    }
                }
                return result;
            }

        }
        public void SetRange(LetterUnit[] letters)
        {
            this.isFound = false;
            letterUnits = letters;
            //Debug.Log("Setting " + word + " to " + StartUnit.Letter + " - " + EndUnit.Letter);
        }
    }
}
