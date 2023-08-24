using System.Collections.Generic;
using System.Linq;

// 等级管理器, 主要管理点击块
public class LevelModel : ModelBase
{
    private readonly List<Alphabet> _selectedList = new List<Alphabet>();
    private readonly List<Alphabet> _hints = new List<Alphabet>();
    public WordGroup WordGroup { get; private set; }
    public LayoutConfig Layout { get; set; }
    public int Seconds { get; set; }
    public WordDifficulty[] WordDifficulties { get; set; }

    public IReadOnlyList<Alphabet> SelectedAlphabets => _selectedList;
    public IReadOnlyList<Alphabet> Hints => _hints;

    public void InitLevel(WordDifficulty[] wds, WordGroup wg, int secs, LayoutConfig layout)
    {
        WordDifficulties = wds;
        WordGroup = wg;
        Seconds = secs;
        Layout = layout;
        _selectedList.Clear();
        SendEvent(GameEvents.Level_Init);
    }

    public void Hints_add(Alphabet alphabet)
    {
        _selectedList.Remove(alphabet);
        SendEvent(GameEvents.Level_Hints_add);
    }

    public void SelectedAlphabet_Add(Alphabet alphabet)
    {
        _selectedList.Add(alphabet);
        SendEvent(GameEvents.Level_Alphabet_Add);
    }

    public void SelectedAlphabet_Clear()
    {
        _selectedList.Clear();
        SendEvent(GameEvents.Level_Word_Clear);
    }

}