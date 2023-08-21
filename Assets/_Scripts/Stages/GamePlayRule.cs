using System.Linq;

/// <summary>
/// 文字玩法规则<br/>
/// 1. 从第一个字开始，依次输入<br/>
/// 2. 输入的字必须是正确的顺序<br/>
/// </summary>
public class GamePlayRule
{
    public string[] Words { get; }
    public int Index { get; private set; }
    public bool IsComplete { get; private set; }
    public string CompletedWord { get; private set; }
    public GamePlayRule(string[] words)
    {
        Words = words;
    }

    public bool CheckIfApply(char character)
    {
        CompletedWord += character;
        var array = Words.Where(o => o.StartsWith(CompletedWord)).ToArray();
        foreach (var word in array)
        {
            Index++;
            IsComplete = Index >= word.Length;
            return true;
        }
        Index = 0;
        IsComplete = false;
        CompletedWord = string.Empty;
        return false;
    }
}