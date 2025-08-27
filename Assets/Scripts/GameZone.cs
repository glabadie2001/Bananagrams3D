using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class GameZone<T> : IEnumerable<T>
{
    [Header("Zone Configuration")]
    [SerializeField, Required] 
    public Transform container;
    
    [SerializeField, InlineProperty]
    [ValidateInput("ValidateRenderer", "Assigned object must implement IZoneRenderer<T>")]
    private ScriptableObject rendererAsset;
    
    public IZoneRenderer<T> Renderer => rendererAsset as IZoneRenderer<T>;
    
    [SerializeField, ReadOnly, ShowInInspector]
    private List<T> contents = new List<T>();
    
    public IEnumerable<T> Contents => contents;
    public int Count => contents.Count;
    
    public T this[int index]
    {
        get => contents[index];
        set => contents[index] = value;
    }
    
    public void Add(T item)
    {
        contents.Add(item);
    }
    
    public bool Remove(T item)
    {
        return contents.Remove(item);
    }
    
    public void RemoveAt(int index)
    {
        contents.RemoveAt(index);
    }
    
    public void Clear()
    {
        contents.Clear();
    }
    
    public bool Contains(T item)
    {
        return contents.Contains(item);
    }
    
    public void InitializeRenderer(IZoneRenderer<T> renderer)
    {
        if (renderer is ScriptableObject so)
        {
            rendererAsset = so;
        }
        else
        {
            // For non-ScriptableObject renderers, we can't assign directly
            // This would require a different architecture for non-SO renderers
            Debug.LogWarning("Cannot assign non-ScriptableObject renderer to GameZone. Use SetRendererAsset() instead.");
        }
    }
    
    public void SetRendererAsset(ScriptableObject asset)
    {
        if (asset == null || asset is IZoneRenderer<T>)
        {
            rendererAsset = asset;
        }
        else
        {
            Debug.LogError($"Asset {asset.name} does not implement IZoneRenderer<{typeof(T).Name}>");
        }
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        return contents.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    [Button("Refresh Zone Display")]
    [ShowIf("@Renderer != null")]
    public void RefreshDisplay()
    {
        Renderer?.Render(contents);
    }
    
    [InfoBox("Renderer is not assigned. Zone will not display visually.", InfoMessageType.Warning)]
    [ShowIf("@Renderer == null")]
    public bool ShowRendererWarning => true;
    
    private bool ValidateRenderer()
    {
        if (rendererAsset == null) return true; // Allow null
        return rendererAsset is IZoneRenderer<T>;
    }
    
    public bool IsWithinBounds(Vector3 worldPosition)
    {
        if (Renderer is IZoneBounds boundsChecker)
        {
            return boundsChecker.IsWithinBounds(worldPosition);
        }
        return false;
    }
}

public interface IZoneRenderer<T>
{
    public void Initialize(Transform container);
    public void Render(IEnumerable<T> list);
    public void OnDrawGizmos();
}

public interface IZoneBounds
{
    bool IsWithinBounds(Vector3 worldPosition);
}