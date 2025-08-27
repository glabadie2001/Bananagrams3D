using UnityEngine;

/// <summary>
/// Handles all grid-based coordinate transformations and positioning logic.
/// Centralizes grid calculations for consistency across the game.
/// </summary>
public static class GridSystem
{
    // Legacy constants for backward compatibility - use GameConstants instead
    public const float GRID_SIZE = 0.5f;
    public const float BOARD_HEIGHT = 0.1f;
    public const float DRAG_HEIGHT = 1.0f;
    
    /// <summary>
    /// Snaps a world position to the nearest grid point.
    /// </summary>
    /// <param name="worldPosition">Position to snap</param>
    /// <param name="height">Y coordinate for snapped position</param>
    /// <returns>Snapped position aligned to grid</returns>
    public static Vector3 SnapToGrid(Vector3 worldPosition, float height = BOARD_HEIGHT)
    {
        float gridSize = GameConstants.Grid.SIZE;
        float snappedX = Mathf.Round(worldPosition.x / gridSize) * gridSize;
        float snappedZ = Mathf.Round(worldPosition.z / gridSize) * gridSize;
        return new Vector3(snappedX, height, snappedZ);
    }
    
    /// <summary>
    /// Converts world position to grid coordinates (legacy method).
    /// Prefer using the overload with Rect for more flexible board positioning.
    /// </summary>
    public static Vector2Int WorldToGridPosition(Vector3 worldPos, int boardWidth = 15, int boardHeight = 15)
    {
        float gridSize = GameConstants.Grid.SIZE;
        int x = Mathf.RoundToInt(worldPos.x / gridSize);
        int z = Mathf.RoundToInt(worldPos.z / gridSize);
        
        x += boardWidth / 2;
        z += boardHeight / 2;
        
        return new Vector2Int(x, z);
    }
    
    public static Vector3 GridToWorldPosition(Vector2Int gridPos, int boardWidth = 15, int boardHeight = 15, float height = BOARD_HEIGHT)
    {
        int x = gridPos.x - boardWidth / 2;
        int z = gridPos.y - boardHeight / 2;
        
        return new Vector3(x * GRID_SIZE, height, z * GRID_SIZE);
    }
    
    public static bool IsValidGridPosition(Vector2Int gridPos, int boardWidth = 15, int boardHeight = 15)
    {
        return gridPos.x >= 0 && gridPos.x < boardWidth &&
               gridPos.y >= 0 && gridPos.y < boardHeight;
    }
    
    /// <summary>
    /// Calculates optimal positioning for tiles in hand display.
    /// Automatically adjusts spacing to fit within bounds.
    /// </summary>
    public static Vector3 CalculateHandPosition(int index, Rect handBounds, float tileSpacing, float handHeight, int tilesPerRow)
    {
        float tileSize = GameConstants.Grid.TILE_SIZE;
        
        int row = index / tilesPerRow;
        int col = index % tilesPerRow;
        
        // Ensure minimum spacing between tiles (tile size + small gap)
        float effectiveSpacing = Mathf.Max(tileSpacing, tileSize * 1.1f);
        
        // Calculate the total width needed for a full row
        float fullRowWidth = (tilesPerRow - 1) * effectiveSpacing;
        
        // Check if full row fits within bounds with margins
        float margin = tileSize * 0.5f;
        float availableWidth = handBounds.width - (2 * margin);
        
        // Adjust spacing if row doesn't fit
        if (fullRowWidth > availableWidth && tilesPerRow > 1) {
            effectiveSpacing = availableWidth / (tilesPerRow - 1);
        }
        
        // Recalculate row width with adjusted spacing
        float rowWidth = (tilesPerRow - 1) * effectiveSpacing;
        
        // Center the row within the hand bounds
        float centerX = handBounds.x + handBounds.width * 0.5f;
        float startX = centerX - rowWidth * 0.5f;
        
        // Position from top of rect bounds with margin
        float topMargin = tileSize * 0.5f;
        float startZ = handBounds.y + handBounds.height - topMargin - (row * effectiveSpacing);
        
        Vector3 position = new Vector3(startX + col * effectiveSpacing, handHeight, startZ);
        
        return position;
    }
    
    public static Vector3 SnapToGrid(Vector3 worldPosition, Rect boardRect, int boardWidth, int boardHeight, float height = BOARD_HEIGHT)
    {
        float cellWidth = boardRect.width / boardWidth;
        float cellHeight = boardRect.height / boardHeight;
        
        // Calculate relative position within the rect
        float relativeX = worldPosition.x - boardRect.x;
        float relativeZ = worldPosition.z - boardRect.y;
        
        // Find which grid cell the position falls into
        int gridX = Mathf.FloorToInt(relativeX / cellWidth);
        int gridZ = Mathf.FloorToInt(relativeZ / cellHeight);
        
        // Clamp to valid grid bounds
        gridX = Mathf.Clamp(gridX, 0, boardWidth - 1);
        gridZ = Mathf.Clamp(gridZ, 0, boardHeight - 1);
        
        // Convert back to world position at cell centers
        float snappedX = boardRect.x + (gridX + 0.5f) * cellWidth;
        float snappedZ = boardRect.y + (gridZ + 0.5f) * cellHeight;
        
        return new Vector3(snappedX, height, snappedZ);
    }
    
    public static Vector2Int WorldToGridPosition(Vector3 worldPos, Rect boardRect, int boardWidth, int boardHeight)
    {
        return WorldToGridPositionSimple(worldPos, boardRect, boardWidth, boardHeight);
    }
    
    public static Vector2Int WorldToGridPositionSimple(Vector3 worldPos, Rect boardRect, int boardWidth, int boardHeight)
    {
        float cellWidth = boardRect.width / boardWidth;
        float cellHeight = boardRect.height / boardHeight;
        
        // Calculate relative position within the board rect
        float relativeX = worldPos.x - boardRect.x;
        float relativeZ = worldPos.z - boardRect.y;
        
        // Convert to grid coordinates - find which cell the position falls into
        int gridX = Mathf.FloorToInt(relativeX / cellWidth);
        int gridZ = Mathf.FloorToInt(relativeZ / cellHeight);
        
        // Clamp to valid grid bounds
        gridX = Mathf.Clamp(gridX, 0, boardWidth - 1);
        gridZ = Mathf.Clamp(gridZ, 0, boardHeight - 1);
        
        return new Vector2Int(gridX, gridZ);
    }
    
    public static Vector2Int WorldToGridPositionByOverlap(Vector3 cursorPos, Rect boardRect, int boardWidth, int boardHeight)
    {
        float cellWidth = boardRect.width / boardWidth;
        float cellHeight = boardRect.height / boardHeight;
        
        // Create a tile-sized rect centered on the cursor position
        Rect tileRect = new Rect(
            cursorPos.x - cellWidth * 0.5f,
            cursorPos.z - cellHeight * 0.5f,
            cellWidth,
            cellHeight
        );
        
        // Find which grid cells could potentially overlap
        // Convert tile rect bounds to grid space and expand search area
        float minGridX = (tileRect.xMin - boardRect.x) / cellWidth;
        float maxGridX = (tileRect.xMax - boardRect.x) / cellWidth;
        float minGridZ = (tileRect.yMin - boardRect.y) / cellHeight;
        float maxGridZ = (tileRect.yMax - boardRect.y) / cellHeight;
        
        int startX = Mathf.Max(0, Mathf.FloorToInt(minGridX));
        int endX = Mathf.Min(boardWidth - 1, Mathf.FloorToInt(maxGridX));
        int startZ = Mathf.Max(0, Mathf.FloorToInt(minGridZ));
        int endZ = Mathf.Min(boardHeight - 1, Mathf.FloorToInt(maxGridZ));
        
        float maxOverlap = 0f;
        Vector2Int bestCell = new Vector2Int(0, 0);
        
        // Check each potentially overlapping cell
        for (int x = startX; x <= endX; x++)
        {
            for (int z = startZ; z <= endZ; z++)
            {
                // Create rect for this grid cell
                Rect cellRect = new Rect(
                    boardRect.x + x * cellWidth,
                    boardRect.y + z * cellHeight,
                    cellWidth,
                    cellHeight
                );
                
                // Calculate overlap area
                float overlap = CalculateRectOverlap(tileRect, cellRect);
                
                if (overlap > maxOverlap)
                {
                    maxOverlap = overlap;
                    bestCell = new Vector2Int(x, z);
                }
            }
        }
        
        return bestCell;
    }
    
    private static float CalculateRectOverlap(Rect rect1, Rect rect2)
    {
        // Calculate intersection boundaries
        float leftEdge = Mathf.Max(rect1.xMin, rect2.xMin);
        float rightEdge = Mathf.Min(rect1.xMax, rect2.xMax);
        float bottomEdge = Mathf.Max(rect1.yMin, rect2.yMin);
        float topEdge = Mathf.Min(rect1.yMax, rect2.yMax);
        
        // Check if there's any overlap
        if (leftEdge >= rightEdge || bottomEdge >= topEdge)
            return 0f;
        
        // Calculate overlap area
        float overlapWidth = rightEdge - leftEdge;
        float overlapHeight = topEdge - bottomEdge;
        return overlapWidth * overlapHeight;
    }
    
    public static Vector3 GridToWorldPosition(Vector2Int gridPos, Rect boardRect, int boardWidth, int boardHeight, float height = BOARD_HEIGHT)
    {
        float cellWidth = boardRect.width / boardWidth;
        float cellHeight = boardRect.height / boardHeight;
        
        // Center the position within the cell
        float worldX = boardRect.x + (gridPos.x + 0.5f) * cellWidth;
        float worldZ = boardRect.y + (gridPos.y + 0.5f) * cellHeight;
        
        return new Vector3(worldX, height, worldZ);
    }
    
}