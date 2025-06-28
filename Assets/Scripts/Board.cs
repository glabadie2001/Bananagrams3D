using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;



/// <summary>
/// Manages the game board state and coordinates with visual representation.
/// Handles tile placement validation and grid coordinate transformations.
/// TODO: Consider separating BoardData from BoardRenderer for better SRP.
/// </summary>
public class Board : SerializedMonoBehaviour
{
    [NonSerialized, OdinSerialize]
    public BoardState state;

    [Header("Board Dimensions")]
    [Tooltip("Board width in grid units")]
    public int width = 20;
    
    [Tooltip("Board height in grid units")]
    public int height = 20;
    
    private GameRules rules;
    private BoardPlaneGenerator planeGenerator;

    private void Awake()
    {
        state = new BoardState(width, height);
        planeGenerator = GetComponent<BoardPlaneGenerator>();
        if (planeGenerator == null)
        {
            planeGenerator = gameObject.AddComponent<BoardPlaneGenerator>();
        }
        Debug.Log(state.tiles.GetLength(0) + "x" + state.tiles.GetLength(1) + " board initialized.");
    }

    /// <summary>
    /// Initializes the board with game rules and sets up visual representation.
    /// Call this after setting up the GameManager and before starting play.
    /// </summary>
    [Button("Init Board")]
    public void Initialize(GameRules gameRules)
    {
        this.rules = gameRules;
        
        // Set board dimensions for plane generator
        if (planeGenerator != null)
        {
            planeGenerator.SetBoardDimensions(state.tiles.GetLength(0), state.tiles.GetLength(1));
        }
        
        // Clear board state
        for (int x = 0; x < state.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < state.tiles.GetLength(1); y++)
            {
                state.tiles[x, y] = new LetterData(string.Empty, 0, null);
            }
        }
    }

    [Button("Generate Grid Lines")]
    public void GeneratePhysicalBoard()
    {
        if (planeGenerator != null)
        {
            planeGenerator.GenerateBoard();
        }
    }

    void Draw()
    {
        // STUB
    }

    /// <summary>
    /// Places a tile on the board, creating both data and visual representation.
    /// Use this for initial tile placement from hand.
    /// </summary>
    /// <param name="letter">Letter data to place</param>
    /// <param name="worldPosition">World position for placement</param>
    /// <returns>True if placement was successful</returns>
    public bool PlaceTile(LetterData letter, Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGridPosition(worldPosition);
        
        if (IsValidGridPosition(gridPos) && state.tiles[gridPos.x, gridPos.y].name == string.Empty)
        {
            state.tiles[gridPos.x, gridPos.y] = letter;
            
            // Create visual representation
            TileFactory.CreateBoardTile(letter, rules.tilePrefab, transform, worldPosition);
            
            return true;
        }
        
        return false;
    }
    
    private Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        if (planeGenerator != null)
        {
            return GridSystem.WorldToGridPosition(worldPos, planeGenerator.GetBoardRect(), state.tiles.GetLength(0), state.tiles.GetLength(1));
        }
        else
        {
            return GridSystem.WorldToGridPosition(worldPos, state.tiles.GetLength(0), state.tiles.GetLength(1));
        }
    }
    
    private bool IsValidGridPosition(Vector2Int gridPos)
    {
        return GridSystem.IsValidGridPosition(gridPos, state.tiles.GetLength(0), state.tiles.GetLength(1));
    }
    
    public Vector3 SnapToGrid(Vector3 worldPosition)
    {
        if (planeGenerator != null)
        {
            return GridSystem.SnapToGrid(worldPosition, planeGenerator.GetBoardRect(), state.tiles.GetLength(0), state.tiles.GetLength(1));
        }
        else
        {
            return GridSystem.SnapToGrid(worldPosition);
        }
    }

    public void RemoveTileAt(Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGridPosition(worldPosition);
        
        if (IsValidGridPosition(gridPos))
        {
            state.tiles[gridPos.x, gridPos.y] = new LetterData(string.Empty, 0, null);
        }
    }

    /// <summary>
    /// Places tile data only without creating visual representation.
    /// Use this when moving existing tile objects to new positions.
    /// </summary>
    /// <param name="letter">Letter data to place</param>
    /// <param name="worldPosition">World position for placement</param>
    /// <returns>True if placement was successful</returns>
    public bool PlaceTileDataOnly(LetterData letter, Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGridPosition(worldPosition);
        
        if (IsValidGridPosition(gridPos) && state.tiles[gridPos.x, gridPos.y].name == string.Empty)
        {
            state.tiles[gridPos.x, gridPos.y] = letter;
            return true;
        }
        
        return false;
    }

    [Button("Get Words")]
    List<WordData> ScanForWords()
    {
        List<WordData> words = new List<WordData>();
        
        List<LetterData> currWord = new List<LetterData>();
        
        //Scan rights
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                if (state.tiles[x, y].name != string.Empty)
                    currWord.Add(state.tiles[x, y]);
                else if (currWord.Count > 0)
                {
                    //Add word on blank
                    words.Add(new WordData(currWord));
                    currWord.Clear();
                }
            }

            if (currWord.Count == 0) continue;
            // Add word at end of row (if we have one) ((literal edge case))
            words.Add(new WordData(currWord));
            currWord.Clear();
        }
        
        //Scan downs
        for (int y = 0; y < this.height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                if (state.tiles[x, y].name != string.Empty)
                    currWord.Add(state.tiles[x, y]);
                else if (currWord.Count > 0)
                {
                    //Add word on blank
                    words.Add(new WordData(currWord));
                    currWord.Clear();
                }
            }

            if (currWord.Count == 0) continue;
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

        Debug.Log(total);

        return total;
    }

    void Clear()
    {
        state.Clear();
        Draw();
    }
}