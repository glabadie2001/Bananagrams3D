using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class HandRenderer : ScriptableObject, IZoneRenderer<LetterData>, IZoneBounds
{
    [Header("Hand Display Settings")]
    private Transform handContainer;
    [SerializeField] private GameObject handTilePrefab;
    [SerializeField] private float tileSpacing = 1.2f;
    [SerializeField] private int tilesPerRow = 7;
    
    [Header("Hand Bounds")]
    [SerializeField] private Rect handBounds = new Rect(-10f, -8f, 8.4f, 4.8f);
    [SerializeField] private bool showBoundsGizmo = true;
    
    public void Initialize(Transform container)
    {
        handContainer = container;
    }

    [Button("Refresh Hand Display")]
    public void Render(IEnumerable<LetterData> letters)
    {
        ClearHand();
        
        int i = 0;
        foreach(LetterData l in letters)
        {
            CreateHandTile(l, i);
            i++;
        }
    }
    
    private void CreateHandTile(LetterData letter, int index)
    {
        Vector3 position = CalculateHandPosition(index);
        GameObject tileObj = TileFactory.CreateHandTile(letter, handTilePrefab, handContainer, position);
    }

    private void DestroyHandTile(Tile tile)
    {
        TileFactory.DestroyTile(tile);
    }

    public Vector3 CalculateHandPosition(int index)
    {
        return GridSystem.CalculateHandPosition(index, handBounds, tileSpacing, 1f, tilesPerRow);
    }

    public void ClearHand()
    {
        foreach (Transform tile in handContainer)
        {
            Object.Destroy(tile.gameObject);
        }
    }
    
    public void OnDrawGizmos()
    {
        if (!showBoundsGizmo) return;

        Gizmos.color = Color.cyan;
        
        Vector3 boundsCenter = new Vector3(
            handBounds.x + handBounds.width * 0.5f,
            1f,
            handBounds.y + handBounds.height * 0.5f
        );
        
        Vector3 boundsSize = new Vector3(handBounds.width, 0.1f, handBounds.height);
        Gizmos.DrawWireCube(boundsCenter, boundsSize);
        
        Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
        Gizmos.DrawCube(boundsCenter, boundsSize);
    }
    
    public bool IsWithinBounds(Vector3 worldPosition)
    {
        Vector2 worldPos2D = new Vector2(worldPosition.x, worldPosition.z);
        Vector2 boundsMin = new Vector2(handBounds.x, handBounds.y);
        Vector2 boundsMax = new Vector2(handBounds.x + handBounds.width, handBounds.y + handBounds.height);
        
        return worldPos2D.x >= boundsMin.x && worldPos2D.x <= boundsMax.x &&
               worldPos2D.y >= boundsMin.y && worldPos2D.y <= boundsMax.y;
    }
}