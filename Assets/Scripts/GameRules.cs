using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRules", menuName = "Scriptable Objects/GameRules")]
public class GameRules : ScriptableObject
{
    public List<LetterData> letters;
    public List<BagLetterEntry> startingBag;
    public GameObject tilePrefab;

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