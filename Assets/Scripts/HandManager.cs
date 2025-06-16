using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HandManager : MonoBehaviour
{
    public static HandManager inst;
    
    [Header("Hand Display Settings")]
    [SerializeField] private Transform handContainer;
    [SerializeField] private GameObject handTilePrefab;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float tileSpacing = 1.2f;
    [SerializeField] private Vector3 handStartPosition = new Vector3(-10f, 1f, -8f);
    [SerializeField] private int tilesPerRow = 7;
    
    [Header("Hand Bounds")]
    [SerializeField] private Rect handBounds = new Rect(-10f, -8f, 8.4f, 4.8f);
    [SerializeField] private bool showBoundsGizmo = true;
    
    public List<Tile> handTiles = new List<Tile>();
    public List<LetterData> handLetters = new List<LetterData>();
    
    public int TileCount => handTiles.Count;
    public bool IsHandFull => handTiles.Count >= GameManager.inst?.handSize;
    

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else if (inst != this)
            Destroy(this);
            
        if (handContainer == null)
            handContainer = transform;
            
        if (playerCamera == null)
            playerCamera = Camera.main;
    }


    public void Initialize(GameObject tilePrefab)
    {
        if (tilePrefab != null)
            handTilePrefab = tilePrefab;
    }

    [Button("Refresh Hand Display")]
    public void RefreshHandDisplay(List<LetterData> letters)
    {
        ClearHand();
        handLetters = new List<LetterData>(letters);
        
        for (int i = 0; i < letters.Count; i++)
        {
            CreateHandTile(letters[i], i);
        }
    }

    public void AddLetterToHand(LetterData letter)
    {
        if (IsHandFull)
        {
            Debug.LogWarning("Hand is full, cannot add more letters.");
            return;
        }
        
        handLetters.Add(letter);
        CreateHandTile(letter, handLetters.Count - 1);
    }

    public bool RemoveLetterFromHand(LetterData letter)
    {
        int index = handLetters.FindIndex(l => l.name == letter.name && l.baseValue == letter.baseValue);
        if (index >= 0)
        {
            handLetters.RemoveAt(index);
            
            if (index < handTiles.Count)
            {
                DestroyHandTile(handTiles[index]);
                handTiles.RemoveAt(index);
                RepositionHandTiles();
            }
            return true;
        }
        return false;
    }

    public bool RemoveHandTile(Tile tile)
    {
        int index = handTiles.IndexOf(tile);
        if (index >= 0)
        {
            handLetters.RemoveAt(index);
            handTiles.RemoveAt(index);
            DestroyHandTile(tile);
            RepositionHandTiles();
            return true;
        }
        return false;
    }

    private void CreateHandTile(LetterData letter, int index)
    {
        Vector3 position = CalculateHandPosition(index);
        GameObject tileObj = TileFactory.CreateHandTile(letter, handTilePrefab, handContainer, position, playerCamera);
        
        if (tileObj != null)
        {
            Tile tile = tileObj.GetComponent<Tile>();
            handTiles.Add(tile);
        }
    }

    private void DestroyHandTile(Tile tile)
    {
        TileFactory.DestroyTile(tile);
    }

    public Vector3 CalculateHandPosition(int index)
    {
        return GridSystem.CalculateHandPosition(index, handBounds, tileSpacing, handStartPosition.y, tilesPerRow);
    }

    public void RepositionHandTiles()
    {
        for (int i = 0; i < handTiles.Count; i++)
        {
            Vector3 newPosition = CalculateHandPosition(i);
            handTiles[i].SetOriginalPosition(newPosition);
        }
    }


    public void ClearHand()
    {
        foreach (var tile in handTiles)
        {
            DestroyHandTile(tile);
        }
        handTiles.Clear();
        handLetters.Clear();
    }

    public List<LetterData> GetHandLetters()
    {
        return new List<LetterData>(handLetters);
    }

    [Button("Test Add Random Letter")]
    private void TestAddRandomLetter()
    {
        if (GameManager.inst?.rules?.letters != null && GameManager.inst.rules.letters.Count > 0)
        {
            var randomLetter = GameManager.inst.rules.letters[Random.Range(0, GameManager.inst.rules.letters.Count)];
            AddLetterToHand(randomLetter);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showBoundsGizmo) return;

        Gizmos.color = Color.cyan;
        
        Vector3 boundsCenter = new Vector3(
            handBounds.x + handBounds.width * 0.5f,
            handStartPosition.y,
            handBounds.y + handBounds.height * 0.5f
        );
        
        Vector3 boundsSize = new Vector3(handBounds.width, 0.1f, handBounds.height);
        Gizmos.DrawWireCube(boundsCenter, boundsSize);
        
        Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
        Gizmos.DrawCube(boundsCenter, boundsSize);
    }

    public Rect GetHandBounds()
    {
        return handBounds;
    }

    public void SetHandBounds(Rect newBounds)
    {
        handBounds = newBounds;
        RepositionHandTiles();
    }

    public bool IsWithinHandBounds(Vector3 worldPosition)
    {
        Vector2 worldPos2D = new Vector2(worldPosition.x, worldPosition.z);
        Vector2 boundsMin = new Vector2(handBounds.x, handBounds.y);
        Vector2 boundsMax = new Vector2(handBounds.x + handBounds.width, handBounds.y + handBounds.height);
        
        return worldPos2D.x >= boundsMin.x && worldPos2D.x <= boundsMax.x &&
               worldPos2D.y >= boundsMin.y && worldPos2D.y <= boundsMax.y;
    }
}