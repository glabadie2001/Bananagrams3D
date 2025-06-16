using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles drag and drop operations for tiles between hand and board.
/// Coordinates with GameManager for state changes and validation.
/// Uses unified Tile system for consistent behavior across locations.
/// </summary>
public class DragDropController : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask draggableLayerMask = 1;
    [SerializeField] private LayerMask boardLayerMask = 1;
    [SerializeField] private LayerMask boardTileLayerMask = 1;
    [SerializeField] private LayerMask handAreaLayerMask = 1;
    
    [Header("Area Detection")]
    [SerializeField] private Transform handArea;
    [SerializeField] private Transform boardArea;
    
    private InputManager inputManager;
    private Tile currentDragTile;
    private GameObject currentDragObject;
    private Vector3 dragOffset;
    private Vector3 originalPosition;
    private bool isDragging;
    

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
            
        inputManager = FindFirstObjectByType<InputManager>();
        if (inputManager == null)
        {
            Debug.LogWarning("No InputManager found, input may not work properly");
        }
    }

    private void OnEnable()
    {
        if (inputManager != null)
        {
            inputManager.OnClickStarted += OnClickStarted;
            inputManager.OnClickCanceled += OnClickCanceled;
        }
    }

    private void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.OnClickStarted -= OnClickStarted;
            inputManager.OnClickCanceled -= OnClickCanceled;
        }
    }

    private void Update()
    {
        if (isDragging && currentDragObject != null)
        {
            UpdateDragPosition();
        }
    }

    private void OnClickStarted()
    {
        if (!isDragging && inputManager != null)
        {
            Ray ray = inputManager.GetCameraRay(playerCamera);
            
            // Check for tiles
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, draggableLayerMask))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    StartDrag(tile, hit.point);
                    return;
                }
            }
            
            // Also check board tile layer mask for backward compatibility
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, boardTileLayerMask))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    StartDrag(tile, hit.point);
                    return;
                }
            }
        }
    }

    private void OnClickCanceled()
    {
        if (isDragging)
        {
            EndDrag();
        }
    }

    private void StartDrag(Tile tile, Vector3 hitPoint)
    {
        currentDragTile = tile;
        currentDragObject = tile.gameObject;
        isDragging = true;
        
        // Store original position in case we need to return
        originalPosition = tile.transform.position;
        
        // If dragging from board, remove from board state
        if (tile.Location == TileLocation.Board)
        {
            var gameManager = GameManager.inst;
            if (gameManager != null)
            {
                gameManager.RemoveTileFromBoard(tile, originalPosition);
            }
        }
        
        // Immediately lift the tile to show it's being dragged
        Vector3 liftedPos = tile.transform.position;
        liftedPos.y = GameConstants.Grid.DRAG_HEIGHT;
        tile.transform.position = liftedPos;
        
        CalculateDragOffset();
        Debug.Log($"Started dragging {tile.Letter.name} from {tile.Location}");
    }

    private void CalculateDragOffset()
    {
        // Simple approach: start with zero offset and let the tile follow the mouse directly
        dragOffset = Vector3.zero;
        Debug.Log("Using zero drag offset for direct mouse following");
    }

    private void UpdateDragPosition()
    {
        if (currentDragObject == null || inputManager == null) 
        {
            Debug.LogWarning("currentDragObject or inputManager is null!");
            return;
        }
        
        Ray ray = inputManager.GetCameraRay(playerCamera);
        
        // Get cursor position at board level for preview calculation
        Plane boardPlane = new Plane(Vector3.up, Vector3.zero);
        
        if (boardPlane.Raycast(ray, out float distance))
        {
            Vector3 cursorWorldPos = ray.GetPoint(distance);
            Vector3 targetPos = GetPreviewPosition(cursorWorldPos);
            
            // Smoothly lerp to the target position
            float snapSpeed = GameConstants.Visual.SNAP_SPEED;
            currentDragObject.transform.position = Vector3.Lerp(
                currentDragObject.transform.position, 
                targetPos, 
                snapSpeed * Time.deltaTime
            );
        }
    }
    
    private Vector3 GetPreviewPosition(Vector3 cursorWorldPos)
    {
        var gameManager = GameManager.inst;
        
        // Check if cursor is within board bounds
        if (gameManager?.board != null)
        {
            var boardPlaneGenerator = gameManager.board.GetComponent<BoardPlaneGenerator>();
            if (boardPlaneGenerator != null)
            {
                Rect boardRect = boardPlaneGenerator.GetBoardRect();
                
                // Check if cursor is within board bounds
                if (cursorWorldPos.x >= boardRect.x && cursorWorldPos.x <= boardRect.x + boardRect.width &&
                    cursorWorldPos.z >= boardRect.y && cursorWorldPos.z <= boardRect.y + boardRect.height)
                {
                    // Within board bounds - snap to grid position at drag height
                    Vector3 snappedBoardPos = gameManager.board.SnapToGrid(cursorWorldPos);
                    snappedBoardPos.y = GameConstants.Grid.DRAG_HEIGHT;
                    return snappedBoardPos;
                }
            }
        }
        
        // Outside board bounds - follow cursor directly at drag height
        return new Vector3(cursorWorldPos.x, GameConstants.Grid.DRAG_HEIGHT, cursorWorldPos.z);
    }
    
    private bool WouldPlaceOnBoard(Vector3 boardPosition)
    {
        var gameManager = GameManager.inst;
        if (gameManager?.board == null || currentDragTile == null) return false;
        
        // This is a preview check - we don't actually place the tile
        // Just check if the position would be valid
        return gameManager.board != null; // Simplified check for now
    }

    private void EndDrag()
    {
        if (currentDragObject == null || inputManager == null) return;
        
        // Get cursor world position at board level
        Ray ray = inputManager.GetCameraRay(playerCamera);
        Plane boardPlane = new Plane(Vector3.up, Vector3.zero);
        
        if (!boardPlane.Raycast(ray, out float distance))
        {
            ReturnToOriginal();
            ClearDragState();
            return;
        }
        
        Vector3 cursorWorldPos = ray.GetPoint(distance);
        Debug.Log($"Cursor world position: {cursorWorldPos}");
        
        var gameManager = GameManager.inst;
        
        // Check if cursor is within board bounds
        if (gameManager?.board != null)
        {
            var boardPlaneGenerator = gameManager.board.GetComponent<BoardPlaneGenerator>();
            if (boardPlaneGenerator != null)
            {
                Rect boardRect = boardPlaneGenerator.GetBoardRect();
                
                if (cursorWorldPos.x >= boardRect.x && cursorWorldPos.x <= boardRect.x + boardRect.width &&
                    cursorWorldPos.z >= boardRect.y && cursorWorldPos.z <= boardRect.y + boardRect.height)
                {
                    // Within board bounds - try to place on board
                    if (TryPlaceOnBoard(cursorWorldPos))
                    {
                        Debug.Log("Successfully placed on board");
                        ClearDragState();
                        return;
                    }
                }
            }
        }
        
        // Check if cursor is within hand bounds
        if (HandManager.inst != null && HandManager.inst.IsWithinHandBounds(cursorWorldPos))
        {
            // Within hand bounds - return to hand
            if (TryReturnToHand(cursorWorldPos))
            {
                Debug.Log("Successfully returned to hand");
                ClearDragState();
                return;
            }
        }
        
        // Not in board bounds or hand bounds - return to original position
        Debug.Log("Drop outside valid areas - returning to original");
        ReturnToOriginal();
        ClearDragState();
    }
    
    private bool TryPlaceOnBoard(Vector3 cursorWorldPos)
    {
        var gameManager = GameManager.inst;
        if (gameManager?.board == null || currentDragTile == null) return false;
        
        // Get snapped board position
        Vector3 boardPosition = gameManager.board.SnapToGrid(cursorWorldPos);
        Debug.Log($"Snapped position: {boardPosition}");
        
        if (currentDragTile.Location == TileLocation.Hand)
        {
            if (gameManager.TryPlaceTileObject(currentDragTile, boardPosition))
            {
                // Set location and position
                currentDragTile.SetLocation(TileLocation.Board);
                currentDragObject.transform.position = boardPosition;
                currentDragObject.transform.SetParent(gameManager.board.transform);
                return true;
            }
        }
        else if (currentDragTile.Location == TileLocation.Board)
        {
            if (gameManager.TryPlaceBoardTileAt(currentDragTile.Letter, boardPosition))
            {
                // Update position
                currentDragObject.transform.position = boardPosition;
                return true;
            }
        }
        
        return false;
    }
    
    private bool TryReturnToHand(Vector3 cursorWorldPos)
    {
        var gameManager = GameManager.inst;
        if (currentDragTile == null) return false;
        
        if (currentDragTile.Location == TileLocation.Board && gameManager != null)
        {
            gameManager.ReturnTileToHand(currentDragTile);
            return true;
        }
        else if (currentDragTile.Location == TileLocation.Hand)
        {
            currentDragTile.SetOriginalPosition(currentDragTile.transform.position);
            return true;
        }
        
        return false;
    }
    
    private void ReturnToOriginal()
    {
        var gameManager = GameManager.inst;
        if (currentDragTile == null) return;
        
        currentDragTile.transform.position = originalPosition;
        
        if (currentDragTile.Location == TileLocation.Board && gameManager != null)
        {
            gameManager.RestoreBoardTileAt(currentDragTile.Letter, originalPosition);
        }
    }
    
    
    private void ClearDragState()
    {
        currentDragTile = null;
        currentDragObject = null;
        isDragging = false;
    }


    public void SetDraggableLayerMask(LayerMask mask)
    {
        draggableLayerMask = mask;
    }

    public void SetBoardLayerMask(LayerMask mask)
    {
        boardLayerMask = mask;
    }
}