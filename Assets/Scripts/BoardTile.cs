using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public LetterData letter;

    public bool Filled => letter.name != string.Empty;
}