using UnityEngine;

/// <summary>
/// Immutable data representing a letter type in the game.
/// This is the template/definition, not a specific instance.
/// </summary>
[System.Serializable]
public struct LetterData
{
    public string name;
    public int baseValue;
    public Material baseMat;

    public LetterData(string name, int baseValue, Material baseMat)
    {
        this.name = name;
        this.baseValue = baseValue;
        this.baseMat = baseMat;
    }

    public float Score()
    {
        return baseValue;
    }

    public override string ToString()
    {
        return $"{name} ({Score()})";
    }
}

/// <summary>
/// Mutable instance of a letter in the game world.
/// Contains instance-specific state like ownership and locked status.
/// </summary>
[System.Serializable]
public class LetterInstance : ISwappable<LetterInstance>
{
    public LetterData data;
    public bool locked;
    public GameZone<LetterInstance> owner;

    public LetterInstance(LetterData data, GameZone<LetterInstance> owner = null)
    {
        this.data = data;
        this.owner = owner;
        this.locked = false;
    }

    public float Score()
    {
        return data.Score();
    }

    public void Lock()
    {
        locked = true;
        Debug.Log($"{data.name} locked");
    }

    public void SetOwner(GameZone<LetterInstance> newOwner)
    {
        owner = newOwner;
    }

    public void OnSwappedTo(GameZone<LetterInstance> newZone)
    {
        SetOwner(newZone);
    }

    public override string ToString()
    {
        return data.ToString();
    }
}