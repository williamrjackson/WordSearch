using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public LineRenderer linePrototype;
    public BoardManager board;
    public Color[] colors;
    public bool showUnsnappedLine = true;
    private LetterUnit currentLineStart;
    private LetterUnit currentLineEnd;
    private LineRenderer currentLine;

    private List<GameObject> lineList = new List<GameObject>();

    private static int lastColorIndex = 0;
    public static LineManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    Color NextColor
    {
        get
        {
            int colorIndex = (lastColorIndex + 1) % colors.Length;
            lastColorIndex = colorIndex;
            return colors[colorIndex];
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && LetterUnit.over != null)
        {
            currentLineStart = LetterUnit.over;
            currentLine = Instantiate(linePrototype);
            currentLine.startColor = currentLine.endColor = NextColor;
            currentLine.transform.parent = linePrototype.transform.parent;
            currentLine.startWidth = .5f;
            currentLine.endWidth = .5f;
            currentLine.positionCount = 2;
            currentLine.SetPosition(0, currentLineStart.lineTarget.position);
        }
        else if (Input.GetMouseButton(0) && currentLine != null)
        {
            if (LetterUnit.over != null)
                currentLineEnd = LetterUnit.over;

            currentLine.SetPosition(1, currentLineEnd.lineTarget.position);

            if (IsValidLine())
            {
                currentLine.gameObject.SetActive(true);
            }
            else if (!showUnsnappedLine)
            {
                currentLine.gameObject.SetActive(false);
            }
            else
            {
                currentLine.gameObject.SetActive(true);
                Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).With(z: currentLineStart.lineTarget.position.z);
                currentLine.SetPosition(1, targetPos);
            }

        }
        else if (Input.GetMouseButtonUp(0) && currentLine != null)
        {
            if (!board.CheckWord(currentLineStart, currentLineEnd))
            {
                Destroy(currentLine.gameObject);
            }
            else
            {
                lineList.Add(currentLine.gameObject);
                board.CheckForWin();
            }
            currentLineStart = null;
            currentLine = null;
        }
    }

    public void ClearLines()
    {
        foreach (GameObject line in lineList)
        {
            Destroy(line);
        }
        lineList.Clear();
    }

    bool IsValidLine()
    {
        if (currentLineStart == null || currentLineEnd == null) return false;

        if (currentLineStart.row == currentLineEnd.row || currentLineStart.column == currentLineEnd.column)
        {
            return true;
        }
        else if (Mathf.Max(currentLineStart.row, currentLineEnd.row) - Mathf.Min(currentLineStart.row, currentLineEnd.row) ==
            Mathf.Max(currentLineStart.column, currentLineEnd.column) - Mathf.Min(currentLineStart.column, currentLineEnd.column))
        {
            return true;
        }
        return false;
    }
}
