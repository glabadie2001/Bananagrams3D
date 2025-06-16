using UnityEngine;

/// <summary>
/// Represents the current location context of a tile.
/// Extensible for future game modes (e.g., Deck, Discard, etc.)
/// </summary>
public enum TileLocation
{
    Hand,
    Board
}

/// <summary>
/// Unified tile component that can exist in any game location.
/// Handles visual state, hover effects, and basic tile behavior.
/// Location-specific logic is handled by managers, not the tile itself.
/// </summary>
public class Tile : MonoBehaviour
{
    [Header("Tile Data")]
    [SerializeField] private LetterData letterData;
    
    [Header("Visual Settings")]
    [SerializeField] private Camera playerCamera;
    
    // Cached components for performance
    private Vector3 originalPosition;
    private bool isHovering = false;
    private Rigidbody rb;
    private Collider col;
    private MeshRenderer meshRenderer;
    
    /// <summary>
    /// The letter this tile represents
    /// </summary>
    public LetterData Letter => letterData;
    
    /// <summary>
    /// Current location context of this tile
    /// </summary>
    public TileLocation Location { get; set; } = TileLocation.Hand;
    
    // Events for decoupled communication with managers
    public event System.Action<Tile> OnTileSelected;
    public event System.Action<Tile, Vector3> OnTilePlaced;
    public event System.Action<Tile> OnTileReturned;

    private void Awake()
    {
        // Cache components for performance
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Start()
    {
        originalPosition = transform.position;
    }

    /// <summary>
    /// Initialize tile with letter data and optional camera reference.
    /// Call after instantiation to set up tile state.
    /// </summary>
    public void Initialize(LetterData letter, Camera camera = null)
    {
        letterData = letter;
        if (camera != null)
            playerCamera = camera;
            
        // Apply visual representation
        if (meshRenderer && letter.baseMat != null)
            meshRenderer.material = letter.baseMat;
    }

    private void OnMouseEnter()
    {
        if (!isHovering)
        {
            StartHover();
        }
    }

    private void OnMouseExit()
    {
        if (isHovering)
        {
            EndHover();
        }
    }

    private void StartHover()
    {
        isHovering = true;
        float hoverHeight = GameConstants.Visual.HOVER_HEIGHT;
        transform.position = originalPosition + Vector3.up * hoverHeight;
    }

    private void EndHover()
    {
        isHovering = false;
        transform.position = originalPosition;
    }

    /// <summary>
    /// Sets the tile's rest position and moves it there if not currently hovering.
    /// Used when repositioning tiles in hand or placing on board.
    /// </summary>
    public void SetOriginalPosition(Vector3 position)
    {
        originalPosition = position;
        if (!isHovering)
            transform.position = position;
    }

    /// <summary>
    /// Enable/disable tile interaction and collision.
    /// Used to prevent interaction during drag operations.
    /// </summary>
    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        if (col != null)
            col.enabled = enabled;
    }

    /// <summary>
    /// Updates the tile's location context.
    /// Used by managers to track where tiles belong.
    /// </summary>
    public void SetLocation(TileLocation location)
    {
        Location = location;
    }
}