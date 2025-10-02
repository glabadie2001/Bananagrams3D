using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the game board state and coordinates with visual representation.
/// Handles tile placement validation and grid coordinate transformations.
/// TODO: Consider separating BoardData from BoardRenderer for better SRP.
/// </summary>
[System.Serializable]
public class Board : GridGameZone<LetterData>
{
    private GameRules rules;

    public void Initialize(GameRules gameRules)
    {
        rules = gameRules;

        // Let the base class properly initialize the contents array
        Initialize(Width, Height);

        Renderer.Initialize(container);
    }

    public void Draw()
    {
        Renderer.RenderGrid(this, Width, Height);
    }
    
    private Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        return GridSystem.WorldToGridPosition(worldPos, Renderer.GetBounds, Width, Height);
    }
    
    private bool IsValidGridPosition(Vector2Int gridPos)
    {
        return GridSystem.IsValidGridPosition(gridPos, Width, Height);
    }
    
    public Vector3 SnapToGrid(Vector3 worldPosition)
    {

        return GridSystem.SnapToGrid(worldPosition, Renderer.GetBounds, Width, Height);
    }

    public void RemoveTileAt(Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGridPosition(worldPosition);
        
        if (IsValidGridPosition(gridPos))
        {
            Debug.Log($"Removed at {gridPos.x} {gridPos.y}");
            this[gridPos.x, gridPos.y] = new LetterData(null, 0, null, false);
        }
    }

    /// <summary>
    /// Places tile and repaints the board.
    /// Use this when moving existing tile objects to new positions.
    /// </summary>
    /// <param name="letter">Letter data to place</param>
    /// <param name="worldPosition">World position for placement</param>
    /// <returns>True if placement was successful</returns>
    public bool PlaceTile(LetterData letter, Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGridPosition(worldPosition);
        
        if (!IsValidGridPosition(gridPos) || this[gridPos.x, gridPos.y] != null) return false;

        this[gridPos.x, gridPos.y] = new LetterData(letter, this);
        
        return true;
    }

    private void ProcessTileForWord(LetterData tile, List<LetterData> currWord, List<WordData> words)
    {
        if (tile == null)
        {
            if (currWord.Count >= rules.minWordLength) words.Add(new WordData(currWord));
            currWord.Clear();
        }
        else
        {
            currWord.Add(tile);
        }
    }

    [Button("Get Words")]
    public List<WordData> ScanForWords()
    {
        List<WordData> words = new List<WordData>();
        
        List<LetterData> currWord = new List<LetterData>();
        //Scan downs
        for (int x = 0; x < Width; x++)
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                ProcessTileForWord(this[x, y], currWord, words);
                Debug.Log(currWord.Count);
            }

            if (currWord.Count < rules.minWordLength) continue;
            // Add word at end of column (if we have one) ((literal edge case))
            words.Add(new WordData(currWord));
            Debug.Log(currWord);
            currWord.Clear();
        }
        
        //Scan rights
        for (int y = Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                ProcessTileForWord(this[x, y], currWord, words);
            }

            if (currWord.Count < rules.minWordLength) continue;
            // Add word at end of row (if we have one) ((literal edge case))
            words.Add(new WordData(currWord));
            currWord.Clear();
        }
        
        return words;
    }

    [Button("Score")]
    public float Score()
    {
        float total = 0f;
        List<WordData> words = ScanForWords();

        foreach (WordData word in words)
        {
            total += word.Score();
        }
        
        return total;
    }

    public void Lock()
    {
        foreach(LetterData l in contents)
        {
            if (l == null) continue;
            l.Lock();
        }
        Draw();
    }
}