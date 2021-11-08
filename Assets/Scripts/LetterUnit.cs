using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterUnit : MonoBehaviour
{
    public TextMeshPro text;
    public Transform lineTarget;
    public Transform hintTargetPosition;
    public int column;
    public int row;
    public bool isPartOfWord = false;
    public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static LetterUnit over = null;
    public char Letter
    {
        set
        {
            _letter = value;
            text.text = _letter.ToString().ToUpper();
            isPartOfWord = true;
        }
        get
        {
            return _letter;
        }
    }
    private char _letter;
    void Start()
    {
        Reset();
    }
    public void Reset()
    {
        Letter = Alphabet.ToCharArray().GetRandom();
        isPartOfWord = false;
    }

    void OnMouseEnter()
    {
        over = this;
    }
    void OnMouseExit()
    {
        if (over == this)
        {
            over = null;
        }
    }
}
