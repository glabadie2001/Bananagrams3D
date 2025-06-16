using UnityEngine;

/// <summary>
/// Factory for creating and managing tile objects across the game.
/// Handles unified tile creation with proper component setup and configuration.
/// </summary>
public static class TileFactory
{
    /// <summary>
    /// Creates a unified tile object with specified location and properties.
    /// Central method for all tile creation - prefer using specific methods below.
    /// </summary>
    public static GameObject CreateTile(LetterData letter, GameObject prefab, Transform parent, Vector3 position, TileLocation location, Camera camera = null)
    {
        if (prefab == null)
        {
            Debug.LogError("Tile prefab not assigned!");
            return null;
        }
        
        GameObject tileObj = Object.Instantiate(prefab, parent);
        tileObj.transform.position = position;
        
        // Ensure unified Tile component exists
        Tile tile = tileObj.GetComponent<Tile>();
        if (tile == null)
            tile = tileObj.AddComponent<Tile>();
        
        tile.Initialize(letter, camera);
        tile.SetOriginalPosition(position);
        tile.SetLocation(location);
        
        ApplyLetterVisuals(tileObj, letter);
        ConfigureTileInteraction(tile);
        
        return tileObj;
    }
    
    /// <summary>
    /// Creates a tile for hand display with proper hand-specific configuration.
    /// </summary>
    public static GameObject CreateHandTile(LetterData letter, GameObject prefab, Transform parent, Vector3 position, Camera camera = null)
    {
        return CreateTile(letter, prefab, parent, position, TileLocation.Hand, camera);
    }
    
    /// <summary>
    /// Creates a tile for board placement with proper board-specific configuration.
    /// </summary>
    public static GameObject CreateBoardTile(LetterData letter, GameObject prefab, Transform parent, Vector3 position)
    {
        return CreateTile(letter, prefab, parent, position, TileLocation.Board);
    }
    
    private static void ApplyLetterVisuals(GameObject tileObj, LetterData letter)
    {
        var renderer = tileObj.GetComponent<MeshRenderer>();
        if (renderer && letter.baseMat != null)
            renderer.material = letter.baseMat;
    }
    
    private static void ConfigureTileInteraction(Tile tile)
    {
        var dragController = Object.FindFirstObjectByType<DragDropController>();
        if (dragController != null)
        {
            tile.enabled = false;
        }
        else
        {
            tile.OnTilePlaced += (t, pos) => Debug.Log($"Tile fallback: {t.Letter.name} placed");
            tile.OnTileReturned += (t) => Debug.Log($"Tile fallback: {t.Letter.name} returned");
        }
    }
    
    public static void DestroyTile(GameObject tileObj)
    {
        if (tileObj != null)
        {
            Object.DestroyImmediate(tileObj);
        }
    }
    
    public static void DestroyTile(Tile tile)
    {
        if (tile != null && tile.gameObject != null)
        {
            Object.DestroyImmediate(tile.gameObject);
        }
    }
}