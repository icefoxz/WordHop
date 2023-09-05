using System;
using System.Collections.Generic;
using System.Linq;
using AOT.Utls;
using UnityEngine;

// 等级管理器, 主要管理点击块
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

        var isApply = Rule.CheckIfApply(alphabet.Text);
        if (isApply)
        {
            SelectedAlphabet_Add(alphabet);
            if (Rule.IsComplete)
            {
                SelectedAlphabet_Clear();
            }
        }
        return isApply;
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

public static class WordHintProvider
{
    public static int CalculateHints(int elapsedSeconds,int totalSeconds,int totalHints)
    {
        var lastHintedIndex = -1; // -1表示没有提示
        int startSecondForHints; // 从第几秒开始提示
        int hintInterval; // 每隔多少秒提示一次
        
        if (totalSeconds > totalHints) // 如果总时间大于提示的次数
        {
            startSecondForHints = totalSeconds / 2;// 从一半开始提示
            hintInterval = (totalSeconds - startSecondForHints) / totalHints;// 每隔多少秒提示一次
        }
        else
        {
            startSecondForHints = 2; // 从第二秒开始提示
            hintInterval = Math.Max(1, (totalSeconds - 1) / totalHints); // 每隔多少秒提示一次
        }

        if (elapsedSeconds < startSecondForHints) // 如果还没到提示的时间
        {
            return lastHintedIndex; // 返回-1
        }

        var currentIndex = (elapsedSeconds - startSecondForHints) / hintInterval; // 当前提示的索引

        if (currentIndex > lastHintedIndex) // 如果当前提示的索引大于上次提示的索引
        {
            lastHintedIndex = currentIndex; // 更新上次提示的索引
        }

        return lastHintedIndex; // 返回当前提示的索引
    }
}
