using System.Collections;
using UnityEngine;

/// <summary>
/// Unified tile component that can exist in any game location.
/// Handles visual state, hover effects, and basic tile behavior.
/// Location-specific logic is handled by managers, not the tile itself.
/// </summary>
public class Tile : MonoBehaviour, IDraggable
{
    [Header("Tile Data")]
    [SerializeField] private LetterInstance letterInstance;

    // Cached components for performance
    private Vector3 originalPosition;
    private bool isHovering = false;
    private Rigidbody rb;
    private Collider col;
    private MeshRenderer meshRenderer;

    /// <summary>
    /// The letter instance this tile represents
    /// </summary>
    public LetterInstance Letter => letterInstance;
    
    private void Awake()
    {
        // Cache components for performance
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        originalPosition = transform.position;
    }

    /// <summary>
    /// Initialize tile with letter instance
    /// Call after instantiation to set up tile state.
    /// </summary>
    public void Initialize(LetterInstance letter)
    {
        letterInstance = letter;

        // Apply visual representation
        if (meshRenderer && letter.data.baseMat != null)
            meshRenderer.material = letter.data.baseMat;
    }

    public void Move(GameZone<LetterInstance> target)
    {
        letterInstance.owner.Remove(letterInstance);
        target.Add(letterInstance);
        letterInstance.owner = target;
    }
    
    /*
    public void Swap(Tile target)
    {
        GameZone<LetterData> targetDst = target.letterData.owner;
        target.Move(this.letterData.owner);
        this.Move(targetDst);
    }*/

    public void Remove()
    {
        letterInstance.owner.Remove(letterInstance);
    }
    
    public void SaveOriginalPosition()
    {
        originalPosition = transform.position;

    }

    public void RestoreOriginalPosition()
    {
        isHovering = false;
        transform.position = originalPosition;
    }

    public IEnumerator Drag(float dragSpeed)
    {
        Debug.Log("Dragging");
        Camera cam = Camera.main;
        Board board = GameManager.Inst.board;
        GameConfig config = GameManager.Inst.configuration;
        Vector3 target;
        isHovering = true;
        
        while (isHovering)
        {
            Vector3 worldMouse = cam.ScreenToWorldPoint(InputManager.Inst.mousePos);

            if (board.IsWithinBounds(worldMouse))
            {
                target = board.SnapToGrid(worldMouse);
            }
            else
            {
                target = worldMouse;
                target.y = config.dragHeight;
            }
            
            transform.position = Vector3.Lerp(transform.position, target, dragSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}