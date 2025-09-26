using System.Collections.Generic;
using System.Linq;

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
            total += c.Score();

        return total;
    }

    public string Text()
    {
        string txt = "";
        foreach (LetterData c in chars)
            txt += c.name;
        
        return txt;
    }
}