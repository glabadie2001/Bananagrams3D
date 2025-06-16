using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central game coordinator that manages game state and coordinates between systems.
/// Handles tile placement validation, hand management, and state transitions.
/// TODO: Consider splitting into separate GameState and GameLogic classes for better SRP.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    
    [Header("Game Configuration")]
    public GameRules rules;
    public GameConfig configuration;

    [Header("Game Systems")]
    [SerializeField] public Board board;


    [Header("Game State")]
    [SerializeField] private Bag<LetterData> reserve;
    [SerializeField] private List<LetterData> discard;
    [SerializeField] private List<LetterData> hand;
    
    /// <summary>
    /// Maximum tiles allowed in hand. Consider moving to GameConfig.
    /// </summary>
    public int handSize = 21;

    void Awake()
    {
        // Singleton pattern - consider using dependency injection for better testability
        if (inst == null)
            inst = this;
        else if (inst != this)
            Destroy(this);

        // Initialize game state containers
        reserve = new Bag<LetterData>();
        discard = new List<LetterData>();
        
        // Initialize constants system if configuration is assigned
        if (configuration != null)
            GameConstants.Initialize(configuration);
    }

    void Start()
    {
        InitializeGame();
    }
    
    /// <summary>
    /// Sets up the initial game state and systems.
    /// </summary>
    private void InitializeGame()
    {
        // Populate reserve with letter distribution from rules
        foreach (var letter in rules.GetExpandedBagContents())
        {
            reserve.Add(letter);
        }

        // Initialize game systems
        board.Initialize(rules);
        
        if (HandManager.inst != null)
        {
            HandManager.inst.Initialize(rules.handTilePrefab ?? rules.tilePrefab);
        }
        
        // Draw initial hand
        DrawHand();
    }

    /// <summary>
    /// Draws tiles from reserve to fill player's hand up to maximum size.
    /// Updates both data state and visual representation.
    /// </summary>
    [Button("Draw Hand")]
    void DrawHand()
    {
        // Draw letters from bag to fill hand up to handSize
        while (hand.Count < handSize && reserve.Count > 0)
        {
            var letter = reserve.Pull();
            hand.Add(letter);
            
            // Add to visual hand immediately
            if (HandManager.inst != null)
                HandManager.inst.AddLetterToHand(letter);
        }
        
        if (reserve.Count == 0 && hand.Count < handSize)
        {
            Debug.LogWarning("No more letters in reserve to draw from.");
        }
    }

    [Button("Discard Hand")]
    void DiscardHand()
    {
        while(hand.Count > 0)
        {
            discard.Add(hand[0]);
            hand.RemoveAt(0);
        }
        
        if (HandManager.inst != null)
            HandManager.inst.ClearHand();
    }
    
    /// <summary>
    /// Attempts to place an existing tile object on the board.
    /// Handles state transitions and manager updates without creating new objects.
    /// </summary>
    /// <param name="tile">The tile object to place</param>
    /// <param name="boardPosition">World position on board</param>
    /// <returns>True if placement was successful</returns>
    public bool TryPlaceTileObject(Tile tile, Vector3 boardPosition)
    {
        // Try to place on board (data only, since we're moving existing object)
        if (board.PlaceTileDataOnly(tile.Letter, boardPosition))
        {
            // Remove from hand data if it was in hand
            if (tile.Location == TileLocation.Hand)
            {
                hand.Remove(tile.Letter);
                
                // Remove from hand manager's tracking
                if (HandManager.inst != null)
                {
                    HandManager.inst.handTiles.Remove(tile);
                    HandManager.inst.handLetters.Remove(tile.Letter);
                    HandManager.inst.RepositionHandTiles();
                }
            }
            
            return true;
        }
        return false;
    }

    public bool TryPlaceBoardTileAt(LetterData letter, Vector3 boardPosition)
    {
        // Try to place board tile at new position (don't create new visual - tile already exists)
        return board.PlaceTileDataOnly(letter, boardPosition);
    }

    public void RemoveTileFromBoard(Tile tile, Vector3 position)
    {
        // Remove tile from board state when starting to drag
        board.RemoveTileAt(position);
    }

    public void RestoreBoardTileAt(LetterData letter, Vector3 position)
    {
        // Put tile back in board state at original position
        board.PlaceTileDataOnly(letter, position);
    }

    public void ReturnTileToHand(Tile tile)
    {
        // Add the tile back to hand when dragged to hand area
        hand.Add(tile.Letter);
        
        if (HandManager.inst != null)
        {
            HandManager.inst.handTiles.Add(tile);
            HandManager.inst.handLetters.Add(tile.Letter);
            
            // Calculate new hand position
            Vector3 handPosition = HandManager.inst.CalculateHandPosition(HandManager.inst.handTiles.Count - 1);
            tile.SetOriginalPosition(handPosition);
            tile.transform.position = handPosition;
            tile.transform.SetParent(HandManager.inst.transform);
            
            HandManager.inst.RepositionHandTiles();
        }
        
        // Update tile location
        tile.SetLocation(TileLocation.Hand);
    }
}
