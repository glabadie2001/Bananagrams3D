using UnityEngine;

[System.Serializable]
public class LetterData
{
    public string name;
    public int baseValue;
    public Material baseMat;
    public bool locked;
    public GameZone<LetterData> owner;

    public LetterData(string name, int baseValue, Material baseMat, bool locked)
    {
        this.name = name;
        this.baseValue = baseValue;
        this.baseMat = baseMat;
        this.locked = locked;
    }

    public LetterData(LetterData template, GameZone<LetterData> owner)
    {
        this.name = template.name;
        this.baseValue = template.baseValue;
        this.baseMat = template.baseMat;
        this.locked = template.locked;
        this.owner = owner;
    }
    
    

    public float Score()
    {
        return baseValue;
    }

    public void Lock()
    {
        locked = true;
        Debug.Log($"{name} locked");
    }

    public void SetOwner(GameZone<LetterData> newOwner)
    {
        owner = newOwner;
    }
}