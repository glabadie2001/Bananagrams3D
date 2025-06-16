using UnityEngine;
using Sirenix.OdinInspector;

public class BoardPlaneGenerator : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private int boardWidth = 15;
    [SerializeField] private int boardHeight = 15;
    [SerializeField] private Rect boardRect = new Rect(-7.5f, -7.5f, 15f, 15f);
    [SerializeField] private Material gridLineMaterial;
    [SerializeField] private float gridLineHeight = 0.01f;
    
    [Header("Auto Generation")]
    [SerializeField] private bool generateOnStart = true;
    
    private GameObject gridLinesParent;

    private void Start()
    {
        if (generateOnStart)
        {
            GenerateBoard();
        }
    }

    [Button("Generate Grid")]
    public void GenerateBoard()
    {
        ClearExistingBoard();
        CreateGridLines();
    }

    [Button("Clear Grid")]
    public void ClearExistingBoard()
    {
        if (gridLinesParent != null)
        {
            DestroyImmediate(gridLinesParent);
            gridLinesParent = null;
        }
    }


    private void CreateGridLines()
    {
        gridLinesParent = new GameObject("GridLines");
        gridLinesParent.transform.parent = transform;
        gridLinesParent.transform.localPosition = Vector3.zero;
        
        float cellWidth = boardRect.width / boardWidth;
        float cellHeight = boardRect.height / boardHeight;
        
        // Create vertical lines
        for (int x = 0; x <= boardWidth; x++)
        {
            float xPos = boardRect.x + x * cellWidth;
            CreateGridLine(
                start: new Vector3(xPos, gridLineHeight, boardRect.y),
                end: new Vector3(xPos, gridLineHeight, boardRect.y + boardRect.height),
                name: $"GridLine_Vertical_{x}"
            );
        }
        
        // Create horizontal lines
        for (int z = 0; z <= boardHeight; z++)
        {
            float zPos = boardRect.y + z * cellHeight;
            CreateGridLine(
                start: new Vector3(boardRect.x, gridLineHeight, zPos),
                end: new Vector3(boardRect.x + boardRect.width, gridLineHeight, zPos),
                name: $"GridLine_Horizontal_{z}"
            );
        }
        
        Debug.Log($"Generated grid lines: {boardWidth}x{boardHeight} within rect {boardRect}");
    }

    private void CreateGridLine(Vector3 start, Vector3 end, string name)
    {
        GameObject lineObj = new GameObject(name);
        lineObj.transform.parent = gridLinesParent.transform;
        
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.material = gridLineMaterial ?? CreateDefaultLineMaterial();
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.positionCount = 2;
        line.useWorldSpace = false;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
        
        line.SetPositions(new Vector3[] { start, end });
    }

    private Material CreateDefaultLineMaterial()
    {
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.color = Color.gray;
        return mat;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // Draw gizmo representation of the board rect and grid in editor
            Gizmos.color = Color.green;
            Vector3 center = new Vector3(
                boardRect.x + boardRect.width * 0.5f,
                gridLineHeight,
                boardRect.y + boardRect.height * 0.5f
            );
            Vector3 size = new Vector3(boardRect.width, 0.1f, boardRect.height);
            Gizmos.DrawWireCube(center, size);
            
            // Draw grid lines as gizmos
            Gizmos.color = Color.gray;
            float cellWidth = boardRect.width / boardWidth;
            float cellHeight = boardRect.height / boardHeight;
            
            // Vertical lines
            for (int x = 0; x <= boardWidth; x++)
            {
                float xPos = boardRect.x + x * cellWidth;
                Vector3 start = new Vector3(xPos, gridLineHeight, boardRect.y);
                Vector3 end = new Vector3(xPos, gridLineHeight, boardRect.y + boardRect.height);
                Gizmos.DrawLine(start, end);
            }
            
            // Horizontal lines
            for (int z = 0; z <= boardHeight; z++)
            {
                float zPos = boardRect.y + z * cellHeight;
                Vector3 start = new Vector3(boardRect.x, gridLineHeight, zPos);
                Vector3 end = new Vector3(boardRect.x + boardRect.width, gridLineHeight, zPos);
                Gizmos.DrawLine(start, end);
            }
        }
    }

    public Vector2Int GetBoardDimensions()
    {
        return new Vector2Int(boardWidth, boardHeight);
    }

    public void SetBoardDimensions(int width, int height)
    {
        boardWidth = width;
        boardHeight = height;
        
        if (Application.isPlaying && gridLinesParent != null)
        {
            GenerateBoard();
        }
    }
    
    public void SetBoardRect(Rect rect)
    {
        boardRect = rect;
        
        if (Application.isPlaying && gridLinesParent != null)
        {
            GenerateBoard();
        }
    }
    
    public Rect GetBoardRect()
    {
        return boardRect;
    }
}