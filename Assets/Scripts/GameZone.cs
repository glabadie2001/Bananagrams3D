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

    // Abstract properties that derived classes must implement
    public abstract int Count { get; }
    public abstract int Capacity { get; }

    // Core operations that derived classes must implement
    public abstract void Clear();
    public abstract bool Contains(T item);
    public abstract IEnumerator<T> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

    protected bool ValidateRenderer()
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
    void RenderGrid(GridGameZone<T> grid, int width, int height);
}