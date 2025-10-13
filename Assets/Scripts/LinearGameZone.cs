using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class LinearGameZone<T> : GameZone<T>
{
    [SerializeField]
    private int capacity;

    [SerializeField, ReadOnly, ShowInInspector]
    private List<T> contents = new List<T>();

    public override int Count => contents.Count;
    public override int Capacity => capacity;

    public IReadOnlyList<T> Contents => contents;

    public T this[int index]
    {
        get => contents[index];
        set => contents[index] = value;
    }

    public void Draw()
    {
        Renderer.Render(this);
    }

    public override void Add(T item)
    {
        if (Count >= Capacity)
        {
            Debug.LogError($"Cannot add {typeof(T).Name}, LinearGameZone is full.");
            return;
        }
        contents.Add(item);
    }

    public override bool Remove(T item) => contents.Remove(item);

    public void RemoveAt(int index) => contents.RemoveAt(index);

    public override void Clear() => contents.Clear();

    public override bool Contains(T item) => contents.Contains(item);

    public override IEnumerator<T> GetEnumerator() => contents.GetEnumerator();

    public override bool Swap(T item1, T item2)
    {
        int index1 = contents.IndexOf(item1);
        int index2 = contents.IndexOf(item2);

        if (index1 < 0 || index2 < 0)
        {
            Debug.LogWarning($"Cannot swap: One or both items not found in LinearGameZone");
            return false;
        }

        return SwapByIndex(index1, index2);
    }

    public override bool SwapByIndex(int index1, int index2)
    {
        if (index1 < 0 || index1 >= contents.Count)
        {
            Debug.LogWarning($"Cannot swap: index1 ({index1}) out of range [0, {contents.Count})");
            return false;
        }

        if (index2 < 0 || index2 >= contents.Count)
        {
            Debug.LogWarning($"Cannot swap: index2 ({index2}) out of range [0, {contents.Count})");
            return false;
        }

        (contents[index1], contents[index2]) = (contents[index2], contents[index1]);
        return true;
    }

    public override int GetIndexOf(T item)
    {
        return contents.IndexOf(item);
    }

    public override T TryGetItemAt(int index)
    {
        if (index < 0 || index >= contents.Count)
            return default(T);
        return contents[index];
    }

    protected override void RenderContents()
    {
        Renderer?.Render(contents);
    }
}