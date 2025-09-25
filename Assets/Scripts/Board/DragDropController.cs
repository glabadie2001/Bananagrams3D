//using UnityEngine;
//using UnityEngine.InputSystem;

///// <summary>
///// Handles drag and drop operations for tiles between hand and board.
///// Coordinates with GameManager for state changes and validation.
///// Uses unified Tile system for consistent behavior across locations.
///// </summary>
//public class DragDropController : MonoBehaviour
//{
//    [Header("Drag Settings")]
//    [SerializeField] private Camera playerCamera;
//    [SerializeField] private LayerMask draggableLayerMask = 1;
//    [SerializeField] private LayerMask boardLayerMask = 1;
//    [SerializeField] private LayerMask handAreaLayerMask = 1;
    
//    [Header("Area Detection")]
//    [SerializeField] private Transform handArea;
//    [SerializeField] private Transform boardArea;
    
//    private InputManager inputManager;
//    [SerializeField]
//    private Tile currentDragTile;
//    private GameObject currentDragObject;
//    private Vector3 dragOffset;
//    private Vector3 originalPosition;
//    private bool isDragging;
    

//    private void Awake()
//    {
//        if (playerCamera == null)
//            playerCamera = Camera.main;
            
//        inputManager = FindFirstObjectByType<InputManager>();
//        if (inputManager == null)
//        {
//            Debug.LogWarning("No InputManager found, input may not work properly");
//        }
//    }

//    private void OnEnable()
//    {
//        if (inputManager != null)
//        {
//            inputManager.OnClickStarted += StartDrag;
//            inputManager.OnClickCanceled += EndDrag;
//        }
//    }

//    private void OnDisable()
//    {
//        if (inputManager != null)
//        {
//            inputManager.OnClickStarted -= StartDrag;
//            inputManager.OnClickCanceled -= EndDrag;
//        }
//    }

//    private void Update()
//    {
//        if (isDragging && currentDragObject != null)
//        {
//            UpdateDragPosition();
//        }
//    }

//    private void StartDrag()
//    {
//        if (!isDragging)
//        {
//            Ray ray = inputManager.GetCameraRay(playerCamera);
            
//            // Check for tiles
//            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, draggableLayerMask))
//            {
//                Tile tile = hit.collider.GetComponent<Tile>();
//                Debug.Log(tile.Letter.locked);
//                if (tile != null && !tile.Letter.locked)
//                {
//                    StartDrag(tile, hit.point);
//                    return;
//                }
//            }
//        }
//    }

//    private void EndDrag()
//    {
//        if (!isDragging || currentDragObject == null || inputManager == null) return;
        
//        // Get cursor world position at board level
//        Ray ray = inputManager.GetCameraRay(playerCamera);
//        Plane boardPlane = new Plane(Vector3.up, Vector3.zero);
        
//        if (!boardPlane.Raycast(ray, out float distance))
//        {
//            ReturnToOriginal();
//            ClearDragState();
//            return;
//        }
        
//        Vector3 cursorWorldPos = ray.GetPoint(distance);
                
//        // Check if cursor is within board bounds
//        if (GameManager.Inst.board.IsWithinBounds(cursorWorldPos))
//        {
//            if (TryPlaceOnBoard(cursorWorldPos))
//            {
//                Debug.Log("Successfully placed on board");
//                GameManager.Inst.board.Draw();
//                ClearDragState();
//                return;
//            }
//        }
        
//        // Check if cursor is within hand bounds
//        if (GameManager.Inst.hand.IsWithinBounds(cursorWorldPos))
//        {
//            // Within hand bounds - return to hand
//            if (TryReturnToHand(cursorWorldPos))
//            {
//                Debug.Log("Successfully returned to hand");
//                ClearDragState();
//                return;
//            }
//        }
        
//        // Not in board bounds or hand bounds - return to original position
//        Debug.Log("Drop outside valid areas - returning to original");
//        ReturnToOriginal();
//        ClearDragState();
//    }

//    private void StartDrag(Tile tile, Vector3 hitPoint)
//    {
//        currentDragTile = tile;
//        currentDragObject = tile.gameObject;
//        isDragging = true;
        
//        // Store original position in case we need to return
//        originalPosition = tile.transform.position;
        
//        // If dragging from board, remove from board state
//        if (tile.Location == TileLocation.Board)
//        {
//            var gameManager = GameManager.Inst;
//            if (gameManager != null)
//            {
//                gameManager.RemoveTileFromBoard(tile, originalPosition);
//            }
//        }
        
//        // Immediately lift the tile to show it's being dragged
//        Vector3 liftedPos = tile.transform.position;
//        liftedPos.y = GameConstants.Grid.DRAG_HEIGHT;
//        tile.transform.position = liftedPos;
        
//        CalculateDragOffset();
//        Debug.Log($"Started dragging {tile.Letter.name} from {tile.Location}");
//    }

//    private void CalculateDragOffset()
//    {
//        // Simple approach: start with zero offset and let the tile follow the mouse directly
//        dragOffset = Vector3.zero;
//        Debug.Log("Using zero drag offset for direct mouse following");
//    }

//    private void UpdateDragPosition()
//    {
//        if (currentDragObject == null || inputManager == null) 
//        {
//            Debug.LogWarning("currentDragObject or inputManager is null!");
//            return;
//        }
        
//        Ray ray = inputManager.GetCameraRay(playerCamera);
        
//        // Get cursor position at board level for preview calculation
//        Plane boardPlane = new Plane(Vector3.up, Vector3.zero);
        
//        if (boardPlane.Raycast(ray, out float distance))
//        {
//            Vector3 cursorWorldPos = ray.GetPoint(distance);
//            Vector3 targetPos = GetPreviewPosition(cursorWorldPos);
            
//            // Smoothly lerp to the target position
//            float snapSpeed = GameConstants.Visual.SNAP_SPEED;
//            currentDragObject.transform.position = Vector3.Lerp(
//                currentDragObject.transform.position, 
//                targetPos, 
//                snapSpeed * Time.deltaTime
//            );
//        }
//    }
    
//    private Vector3 GetPreviewPosition(Vector3 cursorWorldPos)
//    {        
//        // Check if cursor is within board bounds
//        if (GameManager.Inst.board != null && GameManager.Inst.board.IsWithinBounds(cursorWorldPos))
//        {
//            // Within board bounds - snap to grid position at drag height
//            Vector3 snappedBoardPos = GameManager.Inst.board.SnapToGrid(cursorWorldPos);
//            snappedBoardPos.y = GameConstants.Grid.DRAG_HEIGHT;
//            return snappedBoardPos;
//        }
        
//        // Outside board bounds - follow cursor directly at drag height
//        return new Vector3(cursorWorldPos.x, GameConstants.Grid.DRAG_HEIGHT, cursorWorldPos.z);
//    }
    
//    private bool TryPlaceOnBoard(Vector3 cursorWorldPos)
//    {
//        if (GameManager.Inst.board == null || currentDragTile == null) return false;
        
//        Vector3 boardPosition = GameManager.Inst.board.SnapToGrid(cursorWorldPos);
        
//        if (currentDragTile.Location == TileLocation.Hand)
//        {
//            if (GameManager.Inst.board.PlaceTileDataOnly(currentDragTile.Letter, boardPosition))
//            {
//                GameManager.Inst.hand.Remove(currentDragTile.Letter);
//                GameManager.Inst.hand.Draw();
//                Destroy(currentDragTile.gameObject);
//                return true;
//            }
//        }
//        else if (currentDragTile.Location == TileLocation.Board)
//        {
//            if (GameManager.Inst.board.PlaceTileDataOnly(currentDragTile.Letter, boardPosition))
//            {
//                Debug.Log(originalPosition);
//                GameManager.Inst.board.RemoveTileAt(originalPosition);
//                Destroy(currentDragTile.gameObject);
//                return true;
//            }
//        }
        
//        return false;
//    }
    
//    private bool TryReturnToHand(Vector3 cursorWorldPos)
//    {
//        var gameManager = GameManager.Inst;
//        if (currentDragTile == null) return false;
        
//        if (currentDragTile.Location == TileLocation.Board && gameManager != null)
//        {
//            gameManager.ReturnTileToHand(currentDragTile);
//            Destroy(currentDragTile.gameObject);
//            return true;
//        }
//        else if (currentDragTile.Location == TileLocation.Hand)
//        {
//            currentDragTile.SetOriginalPosition(currentDragTile.transform.position);
//            return true;
//        }
        
//        return false;
//    }
    
//    private void ReturnToOriginal()
//    {
//        var gameManager = GameManager.Inst;
//        if (currentDragTile == null) return;
        
//        currentDragTile.transform.position = originalPosition;
        
//        if (currentDragTile.Location == TileLocation.Board && gameManager != null)
//        {
//            gameManager.RestoreBoardTileAt(currentDragTile.Letter, originalPosition);
//        }
//    }
    
//    private void ClearDragState()
//    {
//        currentDragTile = null;
//        currentDragObject = null;
//        isDragging = false;
//    }
//}