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
    public CollectWordParticles _collectParticles;
    private Words currentWordList;
    private List<LetterUnit> letterUnits = new List<LetterUnit>();
    private Coroutine boardRoutine;
    public Words CurrentWordList
    {
        get
        {
            return currentWordList;
        }
        set
        {
            currentWordList = value;
            if (boardRoutine != null)
            {
                StopCoroutine(boardRoutine);
            }
            _boardRebuildRequired = true;
        }
    }

    public void AllowBackwards(string allow)
    {
        if (allow.ToLower() == "false")
        {
            allowBackwards = false;
        }
        else
        {
            allowBackwards = true;
        }
    }

    private bool _boardRebuildRequired = false;
    public void SetRows(int num)
    {
        num = Mathf.Clamp(num, 15, 30);
        rows = num;
        _boardRebuildRequired = true;
    }
    public void SetColumns(int num)
    {
        num = Mathf.Clamp(num, 15, 30);
        columns = num;
        _boardRebuildRequired = true;
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
        Direction.DiagDownForward
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

    private void LateUpdate()
    {
        if (_boardRebuildRequired)
        {
            boardRoutine = StartCoroutine(BuildBoard());
            _boardRebuildRequired = false;
        }
    }
    IEnumerator BuildBoard()
    {
        Debug.Log("Build");
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
            Debug.Log("Resetting letters");
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
                    {
                        Debug.Log($"Added {item.word}");
                        successfullyAddedWords++;
                    }
                }
            }
            boardRoutine = null;
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
        if (column < 0 || column > columns - 1 ||
            row    < 0 || row    > rows    - 1)
        {
            return null;
        }
        int unitIndex = (columns * row) + column;
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
                    // Don't allow side by side things. 
                    // Check for 3 neighbors in the same word
                    int leftNeighborCount = 0;
                    int rightNeighborCount = 0;
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        var leftNeighbor = GetLetterGridIndex(x - 1, y);
                        var rightNeighbor = GetLetterGridIndex(x + 1, y);
                        if (leftNeighbor != null && leftNeighbor.isPartOfWord) leftNeighborCount++;
                        if (rightNeighbor != null && rightNeighbor.isPartOfWord) rightNeighborCount++;
                        if (leftNeighborCount > 3 || rightNeighborCount > 3)
                        {
                            return false;
                        }
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
                    int leftNeighborCount = 0;
                    int rightNeighborCount = 0;
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        var leftNeighbor = GetLetterGridIndex(x - 1, y);
                        var rightNeighbor = GetLetterGridIndex(x + 1, y);
                        if (leftNeighbor != null && leftNeighbor.isPartOfWord) leftNeighborCount++;
                        if (rightNeighbor != null && rightNeighbor.isPartOfWord) rightNeighborCount++;
                        if (leftNeighborCount > 3 || rightNeighborCount > 3)
                        {
                            return false;
                        }
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
                    int topNeighborCount = 0;
                    int bottomNeighborCount = 0;
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        var topNeighbor = GetLetterGridIndex(x, y - 1);
                        var bottomNeighbor = GetLetterGridIndex(x, y + 1);
                        if (topNeighbor != null && topNeighbor.isPartOfWord) topNeighborCount++;
                        if (bottomNeighbor != null && bottomNeighbor.isPartOfWord) bottomNeighborCount++;
                        if (topNeighborCount > 3 || bottomNeighborCount > 3)
                        {
                            return false;
                        }
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
                    int topNeighborCount = 0;
                    int bottomNeighborCount = 0;
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        var topNeighbor = GetLetterGridIndex(x, y - 1);
                        var bottomNeighbor = GetLetterGridIndex(x, y + 1);
                        if (topNeighbor != null && topNeighbor.isPartOfWord) topNeighborCount++;
                        if (bottomNeighbor != null && bottomNeighbor.isPartOfWord) bottomNeighborCount++;
                        if (topNeighborCount > 3 || bottomNeighborCount > 3)
                        {
                            return false;
                        }
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
                    int leftNeighborCount = 0;
                    int rightNeighborCount = 0;
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        var leftNeighbor = GetLetterGridIndex(x - 1, y);
                        var rightNeighbor = GetLetterGridIndex(x + 1, y);
                        if (leftNeighbor != null && leftNeighbor.isPartOfWord) leftNeighborCount++;
                        if (rightNeighbor != null && rightNeighbor.isPartOfWord) rightNeighborCount++;
                        if (leftNeighborCount > 3 || rightNeighborCount > 3)
                        {
                            return false;
                        }
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
                    int leftNeighborCount = 0;
                    int rightNeighborCount = 0;
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        var leftNeighbor = GetLetterGridIndex(x - 1, y);
                        var rightNeighbor = GetLetterGridIndex(x + 1, y);
                        if (leftNeighbor != null && leftNeighbor.isPartOfWord) leftNeighborCount++;
                        if (rightNeighbor != null && rightNeighbor.isPartOfWord) rightNeighborCount++;
                        if (leftNeighborCount > 3 || rightNeighborCount > 3)
                        {
                            return false;
                        }
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
                    int topNeighborCount = 0;
                    int bottomNeighborCount = 0;
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        var topNeighbor = GetLetterGridIndex(x, y - 1);
                        var bottomNeighbor = GetLetterGridIndex(x, y + 1);
                        if (topNeighbor != null && topNeighbor.isPartOfWord) topNeighborCount++;
                        if (bottomNeighbor != null && bottomNeighbor.isPartOfWord) bottomNeighborCount++;
                        if (topNeighborCount > 3 || bottomNeighborCount > 3)
                        {
                            return false;
                        }
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
                    int topNeighborCount = 0;
                    int bottomNeighborCount = 0;
                    for (int i = 0; i < word.treatedWord.Length; i++)
                    {
                        var replaceLetter = GetLetterGridIndex(x, y);
                        var topNeighbor = GetLetterGridIndex(x, y - 1);
                        var bottomNeighbor = GetLetterGridIndex(x, y + 1);
                        if (topNeighbor != null && topNeighbor.isPartOfWord) topNeighborCount++;
                        if (bottomNeighbor != null && bottomNeighbor.isPartOfWord) bottomNeighborCount++;
                        if (topNeighborCount > 3 || bottomNeighborCount > 3)
                        {
                            return false;
                        }
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
                startRow = Random.Range(word.treatedWord.Length - 1, rows);
                startColumn = Random.Range(0, columns);
                break;
            }
            case Direction.Down:
            {
                startRow = Random.Range(0, rows - word.treatedWord.Length);
                startColumn = Random.Range(0, columns);
                break;
            }
            case Direction.Back:
            {
                startRow = Random.Range(0, rows);
                startColumn = Random.Range(word.treatedWord.Length - 1, columns);
                break;
            }
            case Direction.Forward:
            {
                startRow = Random.Range(0, rows);
                startColumn = Random.Range(0, columns - word.treatedWord.Length);
                break;
            }
            case Direction.DiagUpForward:
            {
                startRow = Random.Range(word.treatedWord.Length - 1, rows);
                startColumn = Random.Range(0, columns - word.treatedWord.Length);
                break;
            }
            case Direction.DiagUpBack:
            {
                startRow = Random.Range(word.treatedWord.Length - 1, rows);
                startColumn = Random.Range(word.treatedWord.Length - 1, columns);
                break;
            }
            case Direction.DiagDownForward:
            {
                startRow = Random.Range(0, (rows - word.treatedWord.Length) + 1);
                startColumn = Random.Range(0, columns - (word.treatedWord.Length) + 1);
                break;
            }
            case Direction.DiagDownBack:
            {
                startRow = Random.Range(0, (rows - word.treatedWord.Length) + 1);
                startColumn = Random.Range(word.treatedWord.Length - 1, columns);
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
                _collectParticles.CollectWord(word);
                return true;
            }
        }
        Words.Word unexpectedMatch = MatchExistingFromStartEnd(a, b);
        if (unexpectedMatch == null)
        {
            return false;
        }
        else
        {
            wordReference.Strike(unexpectedMatch.word);
            unexpectedMatch.isFound = true;
            return true;
        }
    }

    private Words.Word MatchExistingFromStartEnd(LetterUnit start, LetterUnit end)
    {
        string res = "";
        int row = start.row;
        int col = start.column;
        List<LetterUnit> letters = new List<LetterUnit>();
        LetterUnit current = start;
        while (current != end)
        {
            current = GetLetterGridIndex(col, row);
            letters.Add(current);
            res += current.Letter;

            if (current.column < end.column) col++;
            else if (current.column > end.column) col--;

            if (current.row < end.row) row++;
            else if (current.row > end.row) row--;
        }
        string reverse = "";
        
        for (int i = res.Length - 1; i >= 0; i--)
        {
            reverse += res[i];
        }
        // Debug.Log($"{res}, {reverse}");
        foreach (var word in currentWordList.words)
        {
            if (word.treatedWord == res || word.treatedWord == reverse)
            {
                word.letterUnits = letters.ToArray();
                return word;
            }
        }
        return null;
    }

    public bool CheckForWin()
    {
        foreach (Words.Word word in currentWordList.words)
        {
            if (word.treatedWord == "") break;
            if (!word.isFound) return false;
        }

        OnBoardCompletion();
        // currentWordlistIndex = (currentWordlistIndex + 1) % wordLists.Length;
        // currentWordList = wordLists[currentWordlistIndex];
        // StartCoroutine(BuildBoard());
        curtain.RandomSuperlative();
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
