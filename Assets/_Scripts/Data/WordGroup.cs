// group of words that have the same letters
public struct WordGroup
{
    public string Key;
    public string[] Words;

    public override string ToString() => $"{{{Key}}}[{string.Join(',', Words)}]";
}