using UnityEngine;

[System.Serializable]
public class LetterData
{
    public string name;
    public int baseValue;
    public Material baseMat;
    public bool locked;

    public LetterData(string name, int baseValue, Material baseMat, bool locked)
    {
        this.name = name;
        this.baseValue = baseValue;
        this.baseMat = baseMat;
        this.locked = locked;
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
}