using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class WordListLoader : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public WordListCreatorPan wordListCreatorPanel;
    public BoardManager board;
    [SerializeField]
    bool _loadOnStart = true;
    private string _directory = Application.streamingAssetsPath;
    private Dictionary<int, Words> _wordlistMap = new Dictionary<int, Words>();
    private Coroutine _autoLoadRoutine;

    void Start()
    {
        dropdown.onValueChanged.AddListener(WordListChanged);
        board.OnBoardCompletion += BoardCompleted;
        List<string> wordListNames = new List<string>();

        for (int i = 0; i < board.wordLists.Length; i++)
        {
            wordListNames.Add(board.wordLists[i].name);
            _wordlistMap.Add(i, board.wordLists[i]);
        }
        wordListNames.Add("Random Words");
        dropdown.AddOptions(wordListNames);

        dropdown.value = 0;
        if (_loadOnStart)
        {
            WordListChanged(0);
        }
    }

    public bool LoadOnStart
    {
        get => _loadOnStart;
        set => _loadOnStart = value;
    }

    private void BoardCompleted()
    {
        if (dropdown.value == dropdown.options.Count - 1)
        {
            WordListChanged(dropdown.value);
            return;
        }
        int currentIndex = dropdown.value;
        int nextIndex = (currentIndex + 1) % dropdown.options.Count;
        _autoLoadRoutine = StartCoroutine(AutoLoadRoutine(nextIndex));
    }

    private IEnumerator AutoLoadRoutine(int index)
    {
        yield return new WaitForSeconds(2f);
        dropdown.value = index;
        _autoLoadRoutine = null;
    }

    private void WordListChanged(int index)
    {
        if (_autoLoadRoutine != null)
        {
            StopCoroutine(_autoLoadRoutine);
            _autoLoadRoutine = null;
        }
        if (_wordlistMap.ContainsKey(index))
        {
            board.CurrentWordList = _wordlistMap[index];
        }
        else
        {
            LoadRandomWords();
        }
    }
    private void LoadRandomWords()
    {
        Wrj.WordList.Instance.Init(Wrj.WordList.WordSource.Common3000);
        var words = Wrj.WordList.GetRandomWords(16, 5, 8);
        string[] workingWords = words.ToArray();
        for (int i = 0; i < workingWords.Length; i++)
        {
            workingWords[i] = workingWords[i].Capitalize();
        }
        wordListCreatorPanel.FillWorkingWords(workingWords);
    }
}
