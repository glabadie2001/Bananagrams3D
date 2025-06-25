using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRules", menuName = "Scriptable Objects/GameRules")]
public class GameRules : ScriptableObject
{
    public List<LetterData> letters;
    public List<BagLetterEntry> startingBag;
    public GameObject tilePrefab;
    public GameObject handTilePrefab;

    public IEnumerable<ValueDropdownItem<int>> GetLetterIndexOptions()
    {
        if (letters == null) yield break;

        for (int i = 0; i < letters.Count; i++)
        {
            yield return new ValueDropdownItem<int>(letters[i].name + " (" + letters[i].baseValue + ")", i);
        }
    }

    public List<LetterData> GetExpandedBagContents()
    {
        var result = new List<LetterData>();
        foreach (var entry in startingBag)
        {
            if (entry.letterIndex >= 0 && entry.letterIndex < letters.Count)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    result.Add(letters[entry.letterIndex]);
                }
            }
        }
        return result;
    }
}

[System.Serializable]
public struct LetterData
{
    public string name;
    public int baseValue;
    public Material baseMat;

    public LetterData(string _text, int _baseValue, Material _baseMat)
    {
        name = _text;
        baseValue = _baseValue;
        baseMat = _baseMat;
    }

    public float Score()
    {
        return baseValue;
    }
}

public struct WordData
{
    public LetterData[] chars;

    // TODO: This might be the slowest shit ever, and probably unnecessary. Eat the overhead of a List if it becomes an issue
    public WordData(IEnumerable<LetterData> _chars)
    {
        chars = _chars.ToArray();
    }

    public float Score()
    {
        float total = 0;
        foreach (LetterData c in chars)
        {
            total += c.Score();
        }

        return total;
    }
}

[System.Serializable]
public struct BagLetterEntry
{
    [ValueDropdown("@UnityEngine.Resources.FindObjectsOfTypeAll<GameRules>()[0].GetLetterIndexOptions()")]
    [LabelText("Letter")]
    public int letterIndex;

    [MinValue(1)]
    public int count;

    public BagLetterEntry(int _letterIndex, int _count)
    {
        letterIndex = _letterIndex;
        count = _count;
    }
}