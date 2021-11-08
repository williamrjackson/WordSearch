using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Range(15, 30)]
    public int columns = 25;
    [Range(15, 30)]
    public int rows = 25;
    public Wrj.GridLayout3d gridLayout;
    public LetterUnit letterUnitPrototype;
    public Words[] wordLists;
    public WordReference wordReference;
    public bool allowBackwards;
    public Curtain curtain;
    public AnimationCurve scaleCurve;

    public UnityAction OnBoardCompletion;
    private Words currentWordList;
    private List<LetterUnit> letterUnits = new List<LetterUnit>();

    public Words CurrentWordList
    {
        get
        {
            return currentWordList;
        }
        set
        {
            currentWordList = value;
            StartCoroutine(BuildBoard());
        }
    }

    private enum Direction { Up, Down, Back, Forward, DiagUpForward, DiagUpBack, DiagDownForward, DiagDownBack}
    private Direction[] allDirections =
    {
        Direction.Up,
        Direction.Down,
        Direction.Back,
        Direction.Forward,
        Direction.DiagUpForward,
        Direction.DiagUpBack,
        Direction.DiagDownForward,
        Direction.DiagDownBack
    };
    private Direction[] forwardDirections =
    {
        Direction.Down,
        Direction.Forward,
        Direction.DiagUpForward,
        Direction.DiagDownForward,
    };

    // private void Start()
    // {
    //     currentWordList = wordLists[currentWordlistIndex];
    //     StartCoroutine(BuildBoard());
    // }

    void ClearLetterUnits()
    {
        while (letterUnits.Count > 0)
        {
            RemoveLetterUnit(letterUnits.GetRandom());
        }
    }

    IEnumerator BuildBoard()
    {
        if (!curtain.IsVisible)
        {
            curtain.IsVisible = true;
            yield return new WaitForSeconds(curtain.Duration + .5f);
        }
        wordReference.Populate(currentWordList);

        int successfullyAddedWords = 0;
        ClearLetterUnits();
        LineManager.Instance.ClearLines();

        int unitCount = columns * rows;

        if (unitCount < 1) yield return null;

        while(unitCount > letterUnits.Count)
        {
            AddLetterUnit();
        }
        while(unitCount > letterUnits.Count)
        {
            RemoveLetterUnit(letterUnits.GetRandom());
        }
        int letterIndex = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                letterUnits[letterIndex].row = i;
                letterUnits[letterIndex].column = j;
                letterIndex++;
            }
        }

        transform.localScale = Vector3.one;

        gridLayout.columns = columns;
        
        while (successfullyAddedWords < currentWordList.words.Length)
        {
            successfullyAddedWords = 0;
            Debug.Log($"Building {currentWordList.name}");
            
            foreach (LetterUnit letter in letterUnits)
            {
                letter.Reset();
            }
            yield return new WaitForEndOfFrame();
            foreach (Words.Word item in currentWordList.words)
            {
                if (item.treatedWord.Length < Mathf.Min(columns, rows))
                {
                    if (AddWord(item))
                        successfullyAddedWords++;
                }
            }
        }
                
        int size = Mathf.Max(columns, rows);
        float sizeToScaleScrub = Mathf.InverseLerp(15f, 30f, (float)size);
        if (scaleCurve.length > 1)
        {
            sizeToScaleScrub = scaleCurve.Evaluate(sizeToScaleScrub);
        }
        float scaleAmount = Mathf.Lerp(1f, .5f, sizeToScaleScrub);
        transform.localScale = Vector3.one * scaleAmount;

        curtain.IsVisible = false;
    }

    void AddLetterUnit()
    {
        var newUnit = Instantiate(letterUnitPrototype);
        newUnit.transform.parent = gridLayout.transform;
        newUnit.gameObject.SetActive(true);
        letterUnits.Add(newUnit);
    }
    void RemoveLetterUnit(LetterUnit unit)
    {
        letterUnits.Remove(unit);
        Destroy(unit.gameObject);
    }

    private LetterUnit GetLetterGridIndex(int column, int row)
    {
        int unitIndex = (columns * row) + column;
        //Debug.Log("Getting letter [" + column + "][" + row + "]");
        //Debug.Log(unitIndex);
        return letterUnits[unitIndex];
    }

    bool PlaceLetters(Words.Word word, int startRow, int startCol, Direction dir, bool apply)
    {
        //if (apply) Debug.Log("Appling " + word.word);
        int x = startCol;
        int y = startRow;
        LetterUnit[] usedLetters = new LetterUnit[word.treatedWord.Length];
        switch (dir)
        {
            case Direction.Up:
                {
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        if (replaceLetter.isPartOfWord && replaceLetter.Letter != word.treatedWord[i])
                        {
                            return false;
                        }
                        else if (apply)
                        {
                            replaceLetter.Letter = word.treatedWord[i];
                            usedLetters[i] = replaceLetter;
                            if (i == word.treatedWord.Length - 1)
                            {
                                word.SetRange(usedLetters);
                            } 
                        }
                        y--;
                    }
                    return true;
                }
            case Direction.Down:
                {
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        if (replaceLetter.isPartOfWord && replaceLetter.Letter != word.treatedWord[i])
                        {
                            return false;
                        }
                        else if (apply)
                        {
                            replaceLetter.Letter = word.treatedWord[i];
                            usedLetters[i] = replaceLetter;
                            if (i == word.treatedWord.Length - 1)
                            {
                                word.SetRange(usedLetters);
                            } 
                        }
                        y++;
                    }
                    return true;
                }
            case Direction.Back:
                {
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        if (replaceLetter.isPartOfWord && replaceLetter.Letter != word.treatedWord[i])
                        {
                            return false;
                        }
                        else if (apply)
                        {
                            replaceLetter.Letter = word.treatedWord[i];
                            usedLetters[i] = replaceLetter;
                            if (i == word.treatedWord.Length - 1)
                            {
                                word.SetRange(usedLetters);
                            } 
                        }
                        x--;
                    }
                    return true;
                }
            case Direction.Forward:
                {
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        if (replaceLetter.isPartOfWord && replaceLetter.Letter != word.treatedWord[i])
                        {
                            return false;
                        }
                        else if (apply)
                        {
                            replaceLetter.Letter = word.treatedWord[i];
                            usedLetters[i] = replaceLetter;
                            if (i == word.treatedWord.Length - 1)
                            {
                                word.SetRange(usedLetters);
                            } 
                        }
                        x++;
                    }
                    return true;
                }
            case Direction.DiagUpForward:
                {
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        if (replaceLetter.isPartOfWord && replaceLetter.Letter != word.treatedWord[i])
                        {
                            return false;
                        }
                        else if (apply)
                        {
                            replaceLetter.Letter = word.treatedWord[i];
                            usedLetters[i] = replaceLetter;
                            if (i == word.treatedWord.Length - 1)
                            {
                                word.SetRange(usedLetters);
                            } 
                        }
                        x++;
                        y--;
                    }
                    return true;
                }
            case Direction.DiagUpBack:
                {
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        if (replaceLetter.isPartOfWord && replaceLetter.Letter != word.treatedWord[i])
                        {
                            return false;
                        }
                        else if (apply)
                        {
                            replaceLetter.Letter = word.treatedWord[i];
                            usedLetters[i] = replaceLetter;
                            if (i == word.treatedWord.Length - 1)
                            {
                                word.SetRange(usedLetters);
                            } 
                        }
                        x--;
                        y--;
                    }
                    return true;
                }
            case Direction.DiagDownForward:
                {
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        if (replaceLetter.isPartOfWord && replaceLetter.Letter != word.treatedWord[i])
                        {
                            return false;
                        }
                        else if (apply)
                        {
                            replaceLetter.Letter = word.treatedWord[i];
                            usedLetters[i] = replaceLetter;
                            if (i == word.treatedWord.Length - 1)
                            {
                                word.SetRange(usedLetters);
                            } 
                        }
                        x++;
                        y++;
                    }
                    return true;
                }
            case Direction.DiagDownBack:
                {
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        if (replaceLetter.isPartOfWord && replaceLetter.Letter != word.treatedWord[i])
                        {
                            return false;
                        }
                        else if (apply)
                        {
                            replaceLetter.Letter = word.treatedWord[i];
                            usedLetters[i] = replaceLetter;
                            if (i == word.treatedWord.Length - 1)
                            {
                                word.SetRange(usedLetters);
                            } 
                        }
                        x--;
                        y++;
                    }
                    return true;
                }
        }
        return false;
    }

    private static Words.Word _currentAssigningWord;
    private static int _currentWordAssignAttempts;
    bool AddWord(Words.Word word)
    {
        Direction direction = RandomDirection;
        if (_currentAssigningWord != word)
        {
            _currentAssigningWord = word;
            _currentWordAssignAttempts = 0;
        }
        _currentWordAssignAttempts++;
        if (_currentWordAssignAttempts > 100)
        {
            return false;
        }
        //Debug.Log(word.treatedWord + " : " + System.Enum.GetName(typeof(Direction), direction));

        int startRow = 0, startColumn = 0;

        switch (direction)
        {
            case Direction.Up:
            {
                startRow = Random.Range(word.treatedWord.Length + 1, rows);
                startColumn = Random.Range(0, columns);
                break;
            }
            case Direction.Down:
            {
                startRow = Random.Range(0, rows - word.treatedWord.Length + 1);
                startColumn = Random.Range(0, columns);
                break;
            }
            case Direction.Back:
            {
                startRow = Random.Range(0, rows);
                startColumn = Random.Range(word.treatedWord.Length + 1, columns);
                break;
            }
            case Direction.Forward:
            {
                startRow = Random.Range(0, rows);
                startColumn = Random.Range(0, columns - word.treatedWord.Length + 1);
                break;
            }
            case Direction.DiagUpForward:
            {
                startRow = Random.Range(rows - word.treatedWord.Length + 1, rows);
                startColumn = Random.Range(0, columns - word.treatedWord.Length + 1);
                break;
            }
            case Direction.DiagUpBack:
            {
                startRow = Random.Range(word.treatedWord.Length + 1, rows);
                startColumn = Random.Range(columns - word.treatedWord.Length + 1, columns);
                break;
            }
            case Direction.DiagDownForward:
            {
                startRow = Random.Range(0, rows - word.treatedWord.Length + 1);
                startColumn = Random.Range(0, columns - word.treatedWord.Length + 1);
                break;
            }
            case Direction.DiagDownBack:
            {
                startRow = Random.Range(0, rows - word.treatedWord.Length + 1);
                startColumn = Random.Range(columns - word.treatedWord.Length + 1, columns);
                break;
            }
            default:
            {
                Debug.Log("Random Direction Switch Failed");
                return false;
            }
        }
        if (PlaceLetters(word, startRow, startColumn, direction, false))
        {
            return PlaceLetters(word, startRow, startColumn, direction, true);
        }
        else
        {
            return AddWord(word);
        }
    }

    public bool CheckWord(LetterUnit a, LetterUnit b)
    {
        //Debug.Log("Checking " + a.Letter + " to " + b.Letter);
        foreach (Words.Word word in currentWordList.words)
        {
            if ((word.StartUnit == a && word.EndUnit == b) || word.EndUnit == a && word.StartUnit == b)
            {
                //Debug.Log(word.word + " FOUND!");
                wordReference.Strike(word.word);
                word.isFound = true;
                return true;
            }
        }
        return false;
    }

    public bool CheckForWin()
    {
        foreach (Words.Word word in currentWordList.words)
        {
            if (!word.isFound) return false;
        }

        OnBoardCompletion();
        // currentWordlistIndex = (currentWordlistIndex + 1) % wordLists.Length;
        // currentWordList = wordLists[currentWordlistIndex];
        // StartCoroutine(BuildBoard());

        return true;
    }

    Direction RandomDirection
    {
        get
        {
            if (allowBackwards)
            {
                return allDirections.GetRandom();
            }
            return forwardDirections.GetRandom();
        }
    }
}
