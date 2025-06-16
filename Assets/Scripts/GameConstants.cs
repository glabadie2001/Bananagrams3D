using UnityEngine;

/// <summary>
/// Static access to game configuration values.
/// Provides compile-time constants and runtime configuration access.
/// </summary>
public static class GameConstants
{
    private static GameConfig _config;
    
    /// <summary>
    /// Initialize the constants system with a configuration asset.
    /// Should be called once at game startup.
    /// </summary>
    public static void Initialize(GameConfig config)
    {
        _config = config;
    }
    
    // Fallback to reasonable defaults if config not initialized
    public static GameConfig Config => _config ?? Resources.Load<GameConfig>("Default");
    
    // Commonly used constants with static access for performance
    public static class Grid
    {
        public static float SIZE => Config?.gridSize ?? 0.5f;
        public static float TILE_SIZE => Config?.tileSize ?? 0.47f;
        public static float BOARD_HEIGHT => Config?.boardYPosition ?? 0.1f;
        public static float DRAG_HEIGHT => Config?.dragHeight ?? 1.0f;
    }
    
    public static class Board
    {
        public static int WIDTH => Config?.boardWidth ?? 20;
        public static int HEIGHT => Config?.boardHeight ?? 20;
    }
    
    public static class Hand
    {
        public static int MAX_SIZE => Config?.handSize ?? 21;
        public static float TILE_SPACING => Config?.tileSpacing ?? 1.2f;
        public static int TILES_PER_ROW => Config?.tilesPerRow ?? 7;
    }
    
    public static class Visual
    {
        public static float HOVER_HEIGHT => Config?.hoverHeight ?? 0.5f;
        public static float SNAP_SPEED => Config?.snapLerpSpeed ?? 5f;
    }
}