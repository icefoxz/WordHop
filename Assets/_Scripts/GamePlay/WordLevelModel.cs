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
    public int TotalSeconds { get; set; }
    public TapDifficulty[] WordDifficulties { get; set; }

    public IReadOnlyList<Alphabet> SelectedAlphabets => _selectedList;
    public IReadOnlyList<Alphabet> Hints => _hints;

    public bool IsComplete => Rule?.IsComplete ?? false;
    //是否上一个字符正确
    public bool IsLastAlphabetApply { get; private set; }
    private GamePlayRule Rule { get; set; }
    public float Difficulty { get; private set; }

    public void InitLevel(TapDifficulty[] wds, WordGroup wg, float difficulty ,int secs, LayoutConfig layout)
    {
        Difficulty = difficulty;
        WordDifficulties = wds;
        WordGroup = wg;
        TotalSeconds = secs;
        Layout = layout;
        Rule = new GamePlayRule(wg.Words);
        _selectedList.Clear();
        SendEvent(GameEvents.Level_Init);
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
        var word = GetCurrentWord();
        var alphabetLength = word.Length;
        var elapsed = TotalSeconds - secs;
        var hints =
            WordHintProvider.CalculateHints(elapsedSeconds: elapsed, totalSeconds: TotalSeconds,
                totalHints: alphabetLength);

        AddHintsOnWord(hints, word);

        SendEvent(GameEvents.Stage_Timer_Update, secs);

        void AddHintsOnWord(int hLetters, string word)
        {
            // 使用hintIndex来决定添加哪一个提示，例如，如果hintIndex是1，则添加第二个字母的提示。
            for (var index = 0; index < hLetters; index++)
            {
                if (index >= word.Length) break;
                if (index < _hints.Count) continue;
                AddHint(index, word);
            }
        }
    }

    //为当前的字增加一个提示
    private void AddHint(int index, string word)
    {
        var alphabet = new Alphabet(index, word[index].ToString());
        _hints.Add(alphabet);
        XDebug.Log($"Add hint: {alphabet.Text}");
        SendEvent(GameEvents.Level_Hints_add, alphabet.Text);
    }

    /// <summary>
    /// 为当前的字增加一个提示
    /// </summary>
    public bool TryAddHint()
    {
        var word = GetCurrentWord();
        var nextHintCount = _hints.Count + 1;
        if (nextHintCount > word.Length - 1) return false;//最后一个字不消费提示
        // 消费提示必须确保填在空位上, 有可能提示字母只有2个,但玩家已经填了3个字母, 所以接下来的提示必须是第4个字母
        var wordDiff = _selectedList.Count - _hints.Count;

        for (var i = 0; i < wordDiff; i++)
        {
            if (TryAddHintsOnWord(nextHintCount, word))
                nextHintCount++;
            else return false;
        }

        return TryAddHintsOnWord(nextHintCount, word);

        bool TryAddHintsOnWord(int hLetters, string word)
        {
            // 使用hintIndex来决定添加哪一个提示，例如，如果hintIndex是1，则添加第二个字母的提示。
            for (var index = 0; index < hLetters; index++)
            {
                if (index >= word.Length - 1) return false;//不提示最后一个字
                if (index < _hints.Count) continue;
                AddHint(index, word);
            }
            return true;
        }
    }

    //获取当前有效的单词(已填入或是第一选择的词)
    private string GetCurrentWord()
    {
        var selectedWord = string.Join(string.Empty, SelectedAlphabets.Select(a => a.Text));
        var word = string.IsNullOrWhiteSpace(selectedWord)
            ? WordGroup.Words[0]
            : WordGroup.Words.First(w => w.StartsWith(selectedWord));
        return word;
    }

    public void Reset()
    {
        Rule = null;
        WordGroup = default;
        Layout = null;
        TotalSeconds = 0;
        _selectedList.Clear();
        _hints.Clear();
        SendEvent(GameEvents.Level_Reset);
    }

    //public int GetScore(int sec)
    //{
    //    var missAve = GetMissTakeAve();
    //    var multiplier = SelectedAlphabets.Count - missAve - 1;//最后-1是因为最小字数是3, 而3个字母最多2倍分数
    //    var score = (int)(sec * multiplier);
    //    return score;
    //}

    public double GetMissTakeAve() => SelectedAlphabets.Average(a => a.MissCount);

    public int GetMissTakes() => SelectedAlphabets.Sum(a => a.MissCount);

    //hack
    public void SetAllAlphabetSelected()
    {
        _selectedList.Clear();
        foreach (var letter in WordGroup.Words[0])
        {
            var alphabet = new Alphabet(_selectedList.Count, letter.ToString());
            _selectedList.Add(alphabet);
        }
    }
}