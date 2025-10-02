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

    protected override void RenderContents()
    {
        Renderer?.Render(contents);
    }
}