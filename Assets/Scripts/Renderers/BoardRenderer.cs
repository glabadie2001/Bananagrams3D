using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable, CreateAssetMenu(menuName = "Bananagrams/Renderer/Board Renderer")]
public class BoardRenderer : ScriptableObject, IGridZoneRenderer<LetterData>
{
    [Header("Display Settings")]
    private Transform handContainer;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Color tileColor;
    [SerializeField] private Color lockedTileColor;

    [Header("Bounds")]
    [SerializeField] private Rect boardBounds = new Rect(0, 0, 15f, 15f);
    [SerializeField] private bool showBoundsGizmo = true;
    [SerializeField] private int zOffset;

    public Rect GetBounds => boardBounds;

    public void Initialize(Transform container)
    {
        handContainer = container;
    }

    public void Render(IEnumerable<LetterData> letters) {
        throw new System.NotImplementedException();
    }

    private void CreateBoardTile(LetterData letter, int x, int y, int width, int height)
    {
        // 1. Calculate the size of a single cell based on the total bounds and grid dimensions.
        float cellWidth = boardBounds.width / width;
        float cellHeight = boardBounds.height / height;

        // 2. Calculate the position of the cell's corner, starting from the board's origin.
        float worldX = boardBounds.x + (x * cellWidth);
        float worldZ = boardBounds.y + (y * cellHeight); // Use grid 'y' for the world 'z' axis

        // 3. Add half a cell's size to center the tile within its grid cell.
        Vector3 centeredPosition = new Vector3(
            worldX + (cellWidth * 0.5f),
            0,
            worldZ + (cellHeight * 0.5f)
        );

        // 4. Create the tile using the correctly calculated position and your vertical offset.
        GameObject tileObj = TileFactory.CreateTile(letter, tilePrefab, handContainer, centeredPosition + Vector3.up * zOffset);

        if (letter.locked)
            tileObj.GetComponent<MeshRenderer>().material.color = lockedTileColor;
        else
            tileObj.GetComponent<MeshRenderer>().material.color = tileColor;
    }

    private void DestroyBoardTile(Tile tile)
    {
        TileFactory.DestroyTile(tile);
    }

    public void ClearBoard()
    {
        foreach (Transform tile in handContainer)
        {
            Destroy(tile.gameObject);
        }
    }
    
    public void OnDrawGizmos()
    {
        if (!showBoundsGizmo) return;

        Gizmos.color = Color.salmon;
        
        Vector3 boundsCenter = new Vector3(
            boardBounds.x + boardBounds.width * 0.5f,
            1f,
            boardBounds.y + boardBounds.height * 0.5f
        );
        
        Vector3 boundsSize = new Vector3(boardBounds.width, 0.1f, boardBounds.height);
        Gizmos.DrawWireCube(boundsCenter, boundsSize);
        
        Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
        Gizmos.DrawCube(boundsCenter, boundsSize);
    }
    
    public bool IsWithinBounds(Vector3 worldPosition)
    {
        Vector2 worldPos2D = new Vector2(worldPosition.x, worldPosition.z);
        Vector2 boundsMin = new Vector2(boardBounds.x, boardBounds.y);
        Vector2 boundsMax = new Vector2(boardBounds.x + boardBounds.width, boardBounds.y + boardBounds.height);
        
        return worldPos2D.x >= boundsMin.x && worldPos2D.x <= boundsMax.x &&
               worldPos2D.y >= boundsMin.y && worldPos2D.y <= boundsMax.y;
    }

    public void RenderGrid(IReadOnlyGrid<LetterData> grid, int width, int height)
    {
        ClearBoard();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null || grid[x,y].name == null) continue;

                CreateBoardTile(grid[x, y], x, y, width, height);
            }
        }
    }
}