using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public LineRenderer linePrototype;
    public BoardManager board;
    public Color[] colors;
    private LetterUnit currentLineStart;
    private LetterUnit currentLineEnd;
    private LineRenderer currentLine;

    private static int lastColorIndex = 0;
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
            currentLineEnd = LetterUnit.over;
            currentLine.SetPosition(1, LetterUnit.over.lineTarget.position);
            if (IsValidLine())
            {
                currentLine.gameObject.SetActive(true);
            }
            else
            {
                currentLine.gameObject.SetActive(false);
            }
        }
        else if (Input.GetMouseButtonUp(0) && currentLine != null)
        {
            if (!board.CheckWord(currentLineStart, currentLineEnd))
            {
                GameObject.Destroy(currentLine);
            }
            currentLineStart = null;
            currentLine = null;
        }
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
