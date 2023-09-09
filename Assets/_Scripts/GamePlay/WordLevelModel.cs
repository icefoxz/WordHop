using System.Collections.Generic;
using System.Linq;
using AOT.Utls;

// 等级管理器, 主要管理文字信息
public class WordLevelModel : ModelBase
{
    private readonly List<Alphabet> _selectedList = new List<Alphabet>();
    private readonly List<Alphabet> _hints = new List<Alphabet>();
    public WordGroup WordGroup { get; private set; }
    public LayoutConfig Layout { get; set; }
    public int Seconds { get; set; }
    public WordDifficulty[] WordDifficulties { get; set; }

    public IReadOnlyList<Alphabet> SelectedAlphabets => _selectedList;
    public IReadOnlyList<Alphabet> Hints => _hints;

    public bool IsComplete => Rule?.IsComplete ?? false;
    //是否上一个字符正确
    public bool IsLastAlphabetApply { get; private set; }
    private GamePlayRule Rule { get; set; }

    public void InitLevel(WordDifficulty[] wds, WordGroup wg, int secs, LayoutConfig layout)
    {
        WordDifficulties = wds;
        WordGroup = wg;
        Seconds = secs;
        Layout = layout;
        Rule = new GamePlayRule(wg.Words);
        _selectedList.Clear();
        SendEvent(GameEvents.Level_Init);
    }

    public int GetCurrentMaxScore()
    {
        var perfectMultiplier = WordGroup.Key.Length - 1; // Perfect game, so no misses.
        return Seconds * perfectMultiplier;
    }

    public void Hints_add(Alphabet alphabet)
    {
        _selectedList.Remove(alphabet);
        SendEvent(GameEvents.Level_Hints_add);
    }

    private void SelectedAlphabet_Add(Alphabet alphabet)
    {
        _selectedList.Add(alphabet);
        SendEvent(GameEvents.Level_Alphabet_Add);
    }

    public void SelectedAlphabet_Clear()
    {
        _selectedList.Clear();
        SendEvent(GameEvents.Level_Word_Clear);
    }

    public bool CheckIfApply(Alphabet alphabet)
    {
        if (SelectedAlphabets.Any(alphabet.Equals)) return true;// 已经点击过了, 但反馈正确

        IsLastAlphabetApply = Rule.CheckIfApply(alphabet.Text);
        if (IsLastAlphabetApply) 
            SelectedAlphabet_Add(alphabet);
        else SendEvent(GameEvents.Level_Alphabet_Failed);
        return IsLastAlphabetApply;
    }

    public void Update(int secs)
    {
        var letters = WordGroup.Words[0];
        var alphabetLength = letters.Length;
        var elapsed = Seconds - secs;
        var hints =
            WordHintProvider.CalculateHints(elapsedSeconds: elapsed, totalSeconds: Seconds, totalHints: alphabetLength);

        // 使用hintIndex来决定添加哪一个提示，例如，如果hintIndex是1，则添加第二个字母的提示。
        for (var index = 0; index < hints; index++)
        {
            if (index >= letters.Length) break;
            if (index < _hints.Count) continue;
            var alphabet = new Alphabet(index, letters[index].ToString());
            _hints.Add(alphabet);
            XDebug.Log($"Add hint: {alphabet.Text}");
            SendEvent(GameEvents.Level_Hints_add, alphabet.Text);
        }

        SendEvent(GameEvents.Stage_Timer_Update, secs);
    }

    public void Reset()
    {
        Rule = null;
        WordGroup = default;
        Layout = null;
        Seconds = 0;
        _selectedList.Clear();
        _hints.Clear();
        SendEvent(GameEvents.Level_Reset);
    }
}