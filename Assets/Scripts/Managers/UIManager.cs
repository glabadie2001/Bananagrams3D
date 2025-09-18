using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager inst;

    public TMP_Text wordDisplay;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else if (inst != this)
            Destroy(this);
    }

    /// <summary>
    /// Debug function for displaying all scored words.
    /// </summary>
    public void DrawWords()
    {
        GameManager.inst.board.Lock();

        if (GameManager.inst == null || GameManager.inst.board == null)
            throw new MissingReferenceException("Could not find the active board. Make sure there is a GameManager in the scene.");

        wordDisplay.text = "";

        List<WordData> words = GameManager.inst.board.ScanForWords();
        foreach (WordData word in words)
        {
            string append = $"{word.Text()} ({word.Score()})\n";;
            wordDisplay.text += append;
        }

        wordDisplay.text += $"\n{GameManager.inst.board.Score()} total points";
    }
}
