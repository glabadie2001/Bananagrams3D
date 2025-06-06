using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Bag<T>
{
    [SerializeField]
    List<T> contents;

    public Bag()
    {
        contents = new List<T>();
    }

    public void Add(T addition)
    {
        contents.Add(addition);
    }

    public void AddMany(params T[] additions)
    {
        contents.AddRange(additions);
    }

    public T Pull()
    {
        int rand = Random.Range(0, contents.Count);
        T item = contents[rand];
        contents.RemoveAt(rand);

        return item;
    }

    public int Count => contents.Count;
}
