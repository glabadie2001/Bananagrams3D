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
    [SerializeField] private List<LetterData> discard;
    [SerializeField] public LinearGameZone<LetterData> hand;
    [SerializeField] private Tile currentTile;
    [SerializeField] private Vector3 lastTilePos;
    
    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
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

        InputManager.Inst.OnSelectStartEvent += (ctx) =>
        {
            Ray clickRay = mainCam.ScreenPointToRay(InputManager.Inst.mousePos);
            
            // TODO: Faster to do grid calculations? Probably unnecessary but worth considering.
            if (!Physics.Raycast(clickRay, out RaycastHit hitInfo, 100f, dragLayer)) return;
            
            lastTilePos = hitInfo.transform.position;
            currentTile = hitInfo.transform.GetComponent<Tile>();

            if (!currentTile.Letter.locked) return;

            currentTile = null;
        };

        InputManager.Inst.OnSelectEndEvent += (ctx) =>
        {
            if (currentTile == null) return;
            
            Vector3 worldMouse = mainCam.ScreenToWorldPoint(InputManager.Inst.mousePos);
            
            // Anywhere -> Board
            if (board.IsWithinBounds(worldMouse))
            {
                if (currentTile.Letter.owner == hand)
                {
                    hand.Remove(currentTile.Letter);
                    hand.Draw();
                    currentTile.Letter.SetOwner(board);
                }
                else if (currentTile.Letter.owner == board)
                {
                    board.Remove(currentTile.Letter);
                }

                board.PlaceTile(currentTile.Letter, board.SnapToGrid(worldMouse));
                board.Draw();
            }
            // Board -> Hand
            else if (currentTile.Letter.owner == board && hand.IsWithinBounds(worldMouse))
            {
                hand.Add(currentTile.Letter);
                hand.Draw();
                board.Remove(currentTile.Letter);
                board.Draw();
                currentTile.Letter.SetOwner(hand);
            }
            // Fallback
            else {
                currentTile.transform.position = lastTilePos;
            }
            
            currentTile = null;
        };

        Debug.Log(board.Count);
    }

    private void Update()
    {
        if (currentTile != null)
        {
            MoveCurrentTile(currentTile.transform);
        }
    }

    private void MoveCurrentTile(Transform tile)
    {
        Vector3 target;
        Vector3 worldMouse = mainCam.ScreenToWorldPoint(InputManager.Inst.mousePos);

        if (board.IsWithinBounds(worldMouse))
        {
            target = board.SnapToGrid(worldMouse);
        }
        else
        {
            target = worldMouse;
            target.y = configuration.dragHeight;
        }
            
        tile.position = Vector3.Lerp(tile.position, target, dragSpeed * Time.deltaTime);
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
            var letter = reserve.Pull();
            hand.Add(new LetterData(letter, hand));
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

    private void OnDrawGizmos()
    {
        hand.Renderer.OnDrawGizmos();
        board.Renderer.OnDrawGizmos();
    }
}
