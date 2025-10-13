using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public abstract class GameZone<T> : IEnumerable<T>
{
    [Header("Zone Configuration")]
    [Required]
    public Transform container;

    [SerializeField, InlineProperty]
    [ValidateInput("ValidateRenderer", "Assigned object must implement IZoneRenderer<T>")]
    protected ScriptableObject rendererAsset;

    public IZoneRenderer<T> Renderer => rendererAsset as IZoneRenderer<T>;

    public abstract int Count { get; }
    public abstract int Capacity { get; }

    public abstract void Add(T item);
    public abstract bool Remove(T item);
    public abstract void Clear();
    public abstract bool Contains(T item);
    public abstract IEnumerator<T> GetEnumerator();

    // Swap methods - support both item-based and index-based swapping
    public abstract bool Swap(T item1, T item2);
    public abstract bool SwapByIndex(int index1, int index2);

    // Helper methods for cross-zone operations
    public abstract int GetIndexOf(T item);
    public abstract T TryGetItemAt(int index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual void SetRendererAsset(ScriptableObject asset)
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

    [Button("Refresh Zone Display")]
    [ShowIf("@Renderer != null")]
    public virtual void RefreshDisplay()
    {
        Renderer?.Initialize(container);
        RenderContents();
    }

    // Let derived classes decide how to pass their data to the renderer
    protected abstract void RenderContents();

    [InfoBox("Renderer is not assigned. Zone will not display visually.", InfoMessageType.Warning)]
    [ShowIf("@Renderer == null")]
    public bool ShowRendererWarning => true;

    protected virtual bool ValidateRenderer()
    {
        if (rendererAsset == null) return true;
        return rendererAsset is IZoneRenderer<T>;
    }

    public bool IsWithinBounds(Vector3 worldPosition)
    {
        return Renderer.IsWithinBounds(worldPosition);
    }
}

public interface IZoneRenderer<T>
{
    public void Initialize(Transform container);
    public void Render(IEnumerable<T> list);
    public void OnDrawGizmos();

    bool IsWithinBounds(Vector3 worldPosition);

    public Rect GetBounds { get; }
}

public interface IGridZoneRenderer<T> : IZoneRenderer<T>
{
    void RenderGrid(IReadOnlyGrid<T> grid, int width, int height);
}