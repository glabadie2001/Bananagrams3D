using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridGameZone<T> : GameZone<T>
{
    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    [SerializeField, ReadOnly, ShowInInspector]
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
                    if (item != null && !EqualityComparer<T>.Default.Equals(item, default(T)))
                        count++;
                }
            }
            return count;
        }
    }

    public override int Capacity => width * height;
    public int Width => width;
    public int Height => height;

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
            return contents[row * width + col];
        }
        set
        {
            ValidateIndices(row, col);
            contents[row * width + col] = value;
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
        if (Renderer is IGridZoneRenderer<T> gridRenderer)
        {
            // If the renderer understands grids, pass the grid data
            gridRenderer.RenderGrid(this, width, height);
        }
        else if (Renderer != null)
        {
            // Fall back to linear rendering
            Renderer.Render(this);
        }
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