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
    public static GameObject CreateTile(LetterInstance letter, GameObject prefab, Transform parent, Vector3 position)
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

        tile.Initialize(letter);
        tile.SaveOriginalPosition();
        //tile.Letter.SetOwner(location);

        ApplyLetterVisuals(tileObj, letter);

        return tileObj;
    }

    private static void ApplyLetterVisuals(GameObject tileObj, LetterInstance letter)
    {
        var renderer = tileObj.GetComponent<MeshRenderer>();
        if (renderer && letter.data.baseMat != null)
            renderer.material = letter.data.baseMat;
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