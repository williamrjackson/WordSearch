using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public LineRenderer linePrototype;
    public BoardManager board;
    public Color[] colors;
    public bool showUnsnappedLine = true;

    [SerializeField]
    private TextureClickThrough textureClickThrough;
    private LetterUnit currentLineStart;
    private LetterUnit currentLineEnd;
    private LineRenderer currentLine;

    private List<GameObject> lineList = new List<GameObject>();

    private static int lastColorIndex = 0;
    public static LineManager Instance;

    private Color _currentColor;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _currentColor = NextColor;
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

    private bool _block = false;
    void LateUpdate()
    {

        if (Input.GetMouseButtonDown(0) && LetterUnit.over != null)
        {
            if (_block) return;
            _block = true;
            currentLineStart = LetterUnit.over;
            currentLine = Instantiate(linePrototype);
            currentLine.startColor = currentLine.endColor = _currentColor;
            currentLine.transform.parent = linePrototype.transform.parent;
            currentLine.startWidth = .3f * board.transform.localScale.magnitude;
            currentLine.endWidth = .3f * board.transform.localScale.magnitude;
            currentLine.positionCount = 2;
            currentLine.SetPosition(0, currentLineStart.lineTarget.position);
        }
        else if (Input.GetMouseButton(0) && currentLine != null)
        {
            if (LetterUnit.over != null)
                currentLineEnd = LetterUnit.over;

            SetLineEnd(currentLineEnd.lineTarget.position);

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
                Vector3 targetPos = textureClickThrough.RelativeMouse.With(z: currentLineStart.lineTarget.position.z);
                SetLineEnd(targetPos);
            }

        }
        else if (Input.touchCount == 0 || Input.GetMouseButtonUp(0))
        {
            LetterUnit.MouseExit();
            if (currentLine != null)
            {
                _block = false;
                if (!board.CheckWord(currentLineStart, currentLineEnd))
                {
                    Destroy(currentLine.gameObject);
                }
                else
                {
                    lineList.Add(currentLine.gameObject);
                    _currentColor = NextColor;
                    board.CheckForWin();
                }
                currentLineStart = null;
                currentLine = null;
            }
        }
    }

    private void SetLineEnd(Vector3 point)
    {
        point = new Vector3(
            Mathf.Clamp(point.x, board.TopLeft.x, board.BottomRight.x),
            Mathf.Clamp(point.y,  board.BottomRight.y, board.TopLeft.y),
            point.z
        );
        currentLine.SetPosition(1, point);
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
