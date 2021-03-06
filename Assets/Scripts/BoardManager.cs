using System.Collections;
using System.Collections.Generic;
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

    private int currentWordlistIndex = 0;
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

    private void Start()
    {
        currentWordList = wordLists[currentWordlistIndex];
        StartCoroutine(BuildBoard());
    }

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
        ClearLetterUnits();
        LineManager.Instance.ClearLines();

        int size = Mathf.Max(columns, rows);
        Camera.main.orthographicSize = Mathf.Clamp(Wrj.Utils.Remap(size, 0, 30, 0, 10), 5, 10);
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
        gridLayout.columns = columns;

        wordReference.Populate(currentWordList);

        Wrj.Utils.DeferPostFrame(() =>
        {
            foreach (Words.Word item in currentWordList.words)
            {
                if (item.treatedWord.Length < Mathf.Min(columns, rows))
                {
                    AddWord(item);
                }
            }
        });
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
                            if (i == word.treatedWord.Length - 1) word.SetRange(GetLetterGridIndex(startCol, startRow), GetLetterGridIndex(x, y));
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
                            if (i == word.treatedWord.Length - 1) word.SetRange(GetLetterGridIndex(startCol, startRow), GetLetterGridIndex(x, y));
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
                            if (i == word.treatedWord.Length - 1) word.SetRange(GetLetterGridIndex(startCol, startRow), GetLetterGridIndex(x, y));
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
                            if (i == word.treatedWord.Length - 1) word.SetRange(GetLetterGridIndex(startCol, startRow), GetLetterGridIndex(x, y));
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
                            if (i == word.treatedWord.Length - 1) word.SetRange(GetLetterGridIndex(startCol, startRow), GetLetterGridIndex(x, y));
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
                            if (i == word.treatedWord.Length - 1) word.SetRange(GetLetterGridIndex(startCol, startRow), GetLetterGridIndex(x, y));
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
                            if (i == word.treatedWord.Length - 1) word.SetRange(GetLetterGridIndex(startCol, startRow), GetLetterGridIndex(x, y));
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
                            if (i == word.treatedWord.Length - 1) word.SetRange(GetLetterGridIndex(startCol, startRow), GetLetterGridIndex(x, y));
                        }
                        x--;
                        y++;
                    }
                    return true;
                }
        }
        return false;
    }

    void AddWord(Words.Word word)
    {
        Direction direction = RandomDirection;
        //Debug.Log(word.treatedWord + " : " + System.Enum.GetName(typeof(Direction), direction));

        int startRow = 0, startColumn = 0;

        switch (direction)
        {
            case Direction.Up:
                {
                    startRow = Random.Range(word.treatedWord.Length, rows);
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
                    startColumn = Random.Range(word.treatedWord.Length, columns);
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
                    startRow = Random.Range(rows - word.treatedWord.Length, rows);
                    startColumn = Random.Range(0, columns - word.treatedWord.Length);
                    break;
                }
            case Direction.DiagUpBack:
                {
                    startRow = Random.Range(word.treatedWord.Length, rows);
                    startColumn = Random.Range(columns - word.treatedWord.Length, columns);
                    break;
                }
            case Direction.DiagDownForward:
                {
                    startRow = Random.Range(0, rows - word.treatedWord.Length);
                    startColumn = Random.Range(0, columns - word.treatedWord.Length);
                    break;
                }
            case Direction.DiagDownBack:
                {
                    startRow = Random.Range(0, rows - word.treatedWord.Length);
                    startColumn = Random.Range(columns - word.treatedWord.Length, columns);
                    break;
                }
            default:
                break;

        }
        if (PlaceLetters(word, startRow, startColumn, direction, false))
        {
            PlaceLetters(word, startRow, startColumn, direction, true);
        }
        else
        {
            AddWord(word);
        }
    }

    public bool CheckWord(LetterUnit a, LetterUnit b)
    {
        //Debug.Log("Checking " + a.Letter + " to " + b.Letter);
        foreach (Words.Word word in currentWordList.words)
        {
            if ((word.start == a && word.end == b) || word.end == a && word.start == b)
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
        currentWordlistIndex = (currentWordlistIndex + 1) % wordLists.Length;
        currentWordList = wordLists[currentWordlistIndex];
        StartCoroutine(BuildBoard());
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
