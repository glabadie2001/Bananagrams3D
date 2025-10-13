using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

/// <summary>
/// Genericized game zone for holding a 2D array of items.
/// </summary>
/// <typeparam name="T">Type held in zone.</typeparam>
[System.Serializable]
public class GridGameZone<T> : GameZone<T>, IReadOnlyGrid<T>
{
    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    [NonSerialized, OdinSerialize, ShowInInspector]
    protected T[] contents; // Flattened array for serialization

    public override int Count
    {
        get
        {
            int count = 0;
            if (contents != null)
            {
                foreach (var item in contents)
                {
                    Debug.Log(item);
                    count++;
                    //if (item != null && !EqualityComparer<T>.Default.Equals(item, default(T)))
                    //    count++;
                }
            }
            return count;
        }
    }

    public override int Capacity => width * height;
    public override void Add(T item)
    {
        throw new System.NotImplementedException();
    }

    public override bool Remove(T item)
    {
        if (contents == null) return false;

        for (int i = 0; i < contents.Length; i++)
        {
            if (contents[i] == null || !contents[i].Equals(item)) continue;
            contents[i] = default(T);
            return true;
        }

        return false;
    }

    public Vector3 SnapToGrid(Vector3 worldPosition)
    {
        return GridSystem.SnapToGrid(worldPosition, Renderer.GetBounds, Width, Height);
    }
    
    public int Width => width;
    public int Height => height;

    // Override to enforce IGridZoneRenderer<T> requirement
    public new IGridZoneRenderer<T> Renderer => rendererAsset as IGridZoneRenderer<T>;

    // Override validation to require IGridZoneRenderer<T>
    protected override bool ValidateRenderer()
    {
        if (rendererAsset == null) return true;
        return rendererAsset is IGridZoneRenderer<T>;
    }

    // Override to enforce IGridZoneRenderer<T> requirement
    public override void SetRendererAsset(ScriptableObject asset)
    {
        if (asset == null || asset is IGridZoneRenderer<T>)
        {
            rendererAsset = asset;
        }
        else
        {
            Debug.LogError($"Asset {asset.name} does not implement IGridZoneRenderer<{typeof(T).Name}>");
        }
    }

    public GridGameZone() { }

    public GridGameZone(int width, int height)
    {
        Initialize(width, height);
    }

    public void Initialize(int width, int height)
    {
        this.width = width;
        this.height = height;
        contents = new T[width * height];
    }

    // 2D indexer
    public T this[int row, int col]
    {
        get
        {
            ValidateIndices(row, col);
            return this[row * width + col];
        }
        set
        {
            ValidateIndices(row, col);
            this[row * width + col] = value;
        }
    }

    // Linear indexer for compatibility
    public T this[int index]
    {
        get => contents[index];
        set => contents[index] = value;
    }

    public (int row, int col) GetCoordinates(int index)
    {
        return (index / width, index % width);
    }

    public int GetIndex(int row, int col)
    {
        ValidateIndices(row, col);
        return row * width + col;
    }

    private void ValidateIndices(int row, int col)
    {
        if (row < 0 || row >= height || col < 0 || col >= width)
        {
            throw new System.IndexOutOfRangeException($"Invalid grid position: ({row}, {col})");
        }
    }

    public override void Clear()
    {
        if (contents != null)
        {
            System.Array.Clear(contents, 0, contents.Length);
        }
    }

    public override bool Contains(T item)
    {
        if (contents == null) return false;
        return System.Array.IndexOf(contents, item) >= 0;
    }

    public override IEnumerator<T> GetEnumerator()
    {
        if (contents != null)
        {
            foreach (var item in contents)
            {
                yield return item;
            }
        }
    }

    public override bool Swap(T item1, T item2)
    {
        if (contents == null)
        {
            Debug.LogWarning("Cannot swap: GridGameZone contents are null");
            return false;
        }

        int index1 = System.Array.IndexOf(contents, item1);
        int index2 = System.Array.IndexOf(contents, item2);

        if (index1 < 0 || index2 < 0)
        {
            Debug.LogWarning($"Cannot swap: One or both items not found in GridGameZone");
            return false;
        }

        return SwapByIndex(index1, index2);
    }

    public override bool SwapByIndex(int index1, int index2)
    {
        if (contents == null)
        {
            Debug.LogWarning("Cannot swap: GridGameZone contents are null");
            return false;
        }

        if (index1 < 0 || index1 >= contents.Length)
        {
            Debug.LogWarning($"Cannot swap: index1 ({index1}) out of range [0, {contents.Length})");
            return false;
        }

        if (index2 < 0 || index2 >= contents.Length)
        {
            Debug.LogWarning($"Cannot swap: index2 ({index2}) out of range [0, {contents.Length})");
            return false;
        }

        (contents[index1], contents[index2]) = (contents[index2], contents[index1]);
        return true;
    }

    public override int GetIndexOf(T item)
    {
        if (contents == null)
        {
            Debug.Log("No contents");
            return -1;
        }
        Debug.Log("Real check!");
        return System.Array.IndexOf(contents, item);
    }

    public override T TryGetItemAt(int index)
    {
        if (contents == null || index < 0 || index >= contents.Length)
            return default(T);
        return contents[index];
    }

    /// <summary>
    /// Swap two grid positions using row/column coordinates.
    /// </summary>
    public bool Swap(int row1, int col1, int row2, int col2)
    {
        try
        {
            int index1 = GetIndex(row1, col1);
            int index2 = GetIndex(row2, col2);
            return SwapByIndex(index1, index2);
        }
        catch (System.IndexOutOfRangeException e)
        {
            Debug.LogWarning($"Cannot swap: {e.Message}");
            return false;
        }
    }

    // Provide row-by-row enumeration for grid-aware operations
    public IEnumerable<IEnumerable<T>> GetRows()
    {
        for (int row = 0; row < height; row++)
        {
            yield return GetRow(row);
        }
    }

    private IEnumerable<T> GetRow(int row)
    {
        for (int col = 0; col < width; col++)
        {
            yield return this[row, col];
        }
    }

    protected override void RenderContents()
    {
        // Renderer is guaranteed to be IGridZoneRenderer<T> or null
        Renderer?.RenderGrid(this, width, height);
    }

    [Button("Resize Grid")]
    public void Resize(int newWidth, int newHeight)
    {
        var newContents = new T[newWidth * newHeight];

        // Copy existing data that fits
        int minWidth = Mathf.Min(width, newWidth);
        int minHeight = Mathf.Min(height, newHeight);

        for (int row = 0; row < minHeight; row++)
        {
            for (int col = 0; col < minWidth; col++)
            {
                newContents[row * newWidth + col] = this[row, col];
            }
        }

        width = newWidth;
        height = newHeight;
        contents = newContents;
    }
}