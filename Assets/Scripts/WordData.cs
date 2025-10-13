using System.Collections.Generic;
using System.Linq;

public struct WordData
{
    public LetterInstance[] chars;

    // TODO: This might be the slowest shit ever, and probably unnecessary. Eat the overhead of a List if it becomes an issue
    public WordData(IEnumerable<LetterInstance> _chars)
    {
        chars = _chars.ToArray();
    }

    public float Score()
    {
        float total = 0;
        foreach (LetterInstance c in chars)
            total += c.Score();

        return total;
    }

    public string Text()
    {
        string txt = "";
        foreach (LetterInstance c in chars)
            txt += c.data.name;

        return txt;
    }
}