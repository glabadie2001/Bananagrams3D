using UnityEngine;

/// <summary>
/// Centralized configuration for all game constants and settings.
/// Create via Assets > Create > Game > Configuration
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config")]
public class GameConfig : ScriptableObject
{
    [Header("Board Settings")]
    [Tooltip("Width of the game board in grid units")]
    public int boardWidth = 20;
    
    [Tooltip("Height of the game board in grid units")]
    public int boardHeight = 20;
    
    [Tooltip("Size of each grid cell in world units")]
    public float gridSize = 0.5f;
    
    [Tooltip("Y position of the board plane")]
    public float boardYPosition = 0.1f;
    
    [Header("Tile Settings")]
    [Tooltip("Physical size of tiles in world units")]
    public float tileSize = 0.47f;
    
    [Tooltip("Height tiles float at when being dragged")]
    public float dragHeight = 1.0f;
    
    [Tooltip("Height tiles lift when hovered")]
    public float hoverHeight = 0.5f;
    
    [Header("Game Rules")]
    [Tooltip("Maximum number of tiles in player's hand")]
    public int handSize = 21;
    
    [Header("Hand Display")]
    [Tooltip("Spacing between tiles in hand")]
    public float tileSpacing = 1.2f;
    
    [Tooltip("Number of tiles per row in hand display")]
    public int tilesPerRow = 7;
    
    [Header("Layer Masks")]
    [Tooltip("Layer for draggable objects")]
    public LayerMask draggableLayer = 1;
    
    [Tooltip("Layer for board area detection")]
    public LayerMask boardLayer = 1;
    
    [Tooltip("Layer for hand area detection")]
    public LayerMask handAreaLayer = 1;
    
    [Header("Performance")]
    [Tooltip("Speed of tile snapping animation")]
    public float snapLerpSpeed = 5f;
}