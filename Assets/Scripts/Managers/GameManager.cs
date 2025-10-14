using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// Central game coordinator that manages game state and coordinates between systems.
/// Handles tile placement validation, hand management, and state transitions.
/// TODO: Consider splitting into separate GameState and GameLogic classes for better SRP.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Inst;
    
    [Header("Game Configuration")]
    public GameRules rules;
    public GameConfig configuration;
    public LayerMask dragLayer;

    [Header("Game Systems")]
    [SerializeField] Camera mainCam;
    [SerializeField] public Board board;

    [Header("Game Feel")]
    [SerializeField] private float dragSpeed = 1;

    [Header("Game State")]
    [SerializeField] private Bag<LetterData> reserve;
    [SerializeField] private List<LetterInstance> discard;
    [SerializeField] public LinearGameZone<LetterInstance> hand;
    [SerializeField] private Tile currentTile;
    
    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(this);

        // Initialize game state containers
        reserve = new Bag<LetterData>();
        discard = new List<LetterInstance>();
        
        // Initialize constants system if configuration is assigned
        if (configuration != null)
            GameConstants.Initialize(configuration);
    }

    void Start()
    {
        InitializeGame();

        // OnSelectStart binding for tile movement
        InputManager.Inst.OnSelectStartEvent += (ctx) =>
        {
            Ray clickRay = mainCam.ScreenPointToRay(InputManager.Inst.mousePos);
            
            // TODO: Faster to do grid calculations? Probably unnecessary but worth considering.
            if (!Physics.Raycast(clickRay, out RaycastHit hitInfo, 100f, dragLayer)) return;
            
            IDraggable dragTarget = hitInfo.transform.GetComponent<IDraggable>();

            if (dragTarget.GetType() == typeof(Tile))
            {
                currentTile = dragTarget as Tile;
                if (currentTile != null && currentTile.Letter.locked)
                {
                    currentTile = null;
                    return;
                }
            }
            EventManager.Inst.Enqueue(new DragStartEvent(dragTarget, dragSpeed));
        };

        InputManager.Inst.OnSelectEndEvent += (ctx) =>
        {
            if (currentTile == null) return;

            Vector3 worldMouse = mainCam.ScreenToWorldPoint(InputManager.Inst.mousePos);
            TryPlaceTile(worldMouse);
            currentTile = null;
        };
    }

    private bool TryPlaceTile(Vector3 worldMouse)
    {
        // Anywhere -> Board
        if (board.IsWithinBounds(worldMouse))
        {
            Vector2Int tileIndex = board.WorldToIndex(worldMouse);
            if (TryPlaceOnBoard(tileIndex)) return true;
        }
        // Board -> Hand
        else if (hand.IsWithinBounds(worldMouse) && currentTile.Letter.owner == board)
        {
            // TODO: Could calculate a hand index based on mouse position if needed
            int handIndex = hand.Count; // Add to end for now
            if (SwapBetweenZones(currentTile.Letter, board, hand, handIndex)) return true;
        }
        // Fallback
        else
        {
            currentTile.RestoreOriginalPosition();
        }

        return false;
    }

    private bool TryPlaceOnBoard(Vector2Int tileIndex)
    {
        LetterInstance target = board[tileIndex.x, tileIndex.y];

        if (target is { locked: true })
        {
            return false;
        }
                
        // Calculate target board index
        int targetIndex = board.GetIndex(tileIndex.x, tileIndex.y);
                
        // OnSwappedTo will be called automatically to update owner
        return SwapBetweenZones(currentTile.Letter, currentTile.Letter.owner, board, targetIndex);
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
        
        // Initialize hand renderer if not set
        hand.Renderer.Initialize(hand.container);
        
        // Draw initial hand
        DrawHand();
    }

    /// <summary>
    /// Draws tiles from reserve to fill player's hand up to maximum size.
    /// Updates both data state and visual representation.
    /// PRECONDITION: An empty hand
    /// POSTCONDITION: A full hand
    /// TODO: Create the board piecemeal rather than redrawing everything.
    /// </summary>
    [Button("Draw Hand")]
    public void DrawHand()
    {
        while (hand.Count < hand.Capacity && reserve.Count > 0)
        {
            var letterData = reserve.Pull();
            var letterInstance = new LetterInstance(letterData, hand);
            hand.Add(letterInstance);
        }

        hand.Renderer.Render(hand);
    }

    [Button("Discard Hand")]
    void DiscardHand()
    {
        while(hand.Count > 0)
        {
            discard.Add(hand[0]);
            hand.RemoveAt(0);
        }

        hand.Renderer.Render(hand);
    }

    /// <summary>
    /// Swaps a source item between two different GameZones, or moves if target position is empty.
    /// Handles cross-zone item movement with optional swap behavior.
    /// Generic method that works with any GameZone type.
    /// Automatically calls OnSwappedTo for items implementing ISwappable.
    /// </summary>
    /// <typeparam name="T">The type of items in the zones (must implement ISwappable)</typeparam>
    /// <param name="sourceItem">The item being moved</param>
    /// <param name="sourceZone">The zone the item is currently in</param>
    /// <param name="targetZone">The zone to move the item to</param>
    /// <param name="targetIndex">The target index in the target zone</param>
    /// <returns>True if the swap/move was successful</returns>
    /// TODO: Beautify
    public static bool SwapBetweenZones<T>(T sourceItem, GameZone<T> sourceZone, GameZone<T> targetZone, int targetIndex) where T : ISwappable<T>
    {
        if (EqualityComparer<T>.Default.Equals(sourceItem, default(T)) || sourceZone == null || targetZone == null)
        {
            Debug.LogWarning("SwapBetweenZones: Source item, source zone, or target zone is null/invalid");
            return false;
        }

        int sourceIndex = sourceZone.GetIndexOf(sourceItem);
        if (sourceIndex < 0)
        {
            Debug.LogWarning($"SwapBetweenZones: Source item not found in source zone. {sourceItem} - {sourceIndex}");
            return false;
        }

        // Optimization: If both zones are the same, use the simpler within-zone swap
        if (sourceZone == targetZone)
        {
            bool success = sourceZone.SwapByIndex(sourceIndex, targetIndex);
            if (success)
            {
                sourceZone.RefreshDisplay();
            }
            return success;
        }

        // Try to get target item (may be null/default)
        T targetItem = targetZone.TryGetItemAt(targetIndex);

        // Remove source item from its zone
        if (!sourceZone.Remove(sourceItem))
        {
            Debug.LogWarning("SwapBetweenZones: Failed to remove source item from source zone");
            return false;
        }

        // If target item exists, remove it from target zone
        bool hasTargetItem = !EqualityComparer<T>.Default.Equals(targetItem, default(T));
        if (hasTargetItem)
        {
            if (!targetZone.Remove(targetItem))
            {
                // Rollback: re-add source item to source zone
                sourceZone.Add(sourceItem);
                Debug.LogWarning("SwapBetweenZones: Failed to remove target item from target zone");
                return false;
            }
        }

        // Place source item at target position
        // For GridGameZone, we need to use indexer assignment
        if (targetZone is GridGameZone<T> gridZone)
        {
            gridZone[targetIndex] = sourceItem;
        }
        // For LinearGameZone, we need to insert at specific position
        // TODO: Squash this into a single operation?
        else if (targetZone is LinearGameZone<T> linearZone)
        {
            linearZone.Add(sourceItem);
        }

        // Notify source item of its new zone
        sourceItem.OnSwappedTo(targetZone);

        // If there was a target item, place it at source's original position
        if (hasTargetItem)
        {
            if (sourceZone is GridGameZone<T> srcGridZone)
            {
                srcGridZone[sourceIndex] = targetItem;
            }
            else if (sourceZone is LinearGameZone<T> srcLinearZone)
            {
                srcLinearZone.Add(targetItem);
            }

            // Notify target item of its new zone
            targetItem.OnSwappedTo(sourceZone);
        }

        // Refresh displays
        sourceZone.RefreshDisplay();
        targetZone.RefreshDisplay();

        return true;
    }

    private void OnDrawGizmos()
    {
        hand.Renderer.OnDrawGizmos();
        board.Renderer.OnDrawGizmos();
    }
}
