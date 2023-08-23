using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_LevelDifficultive : MonoBehaviour
{
    public LevelDifficultySo LevelDifficultySo;
    public TapPadDifficultySo TapPadDifficultySo;

    [Button]
    public void TestSetting(float difficultyValue)
    {
        var w = LevelLoader.GetDifficultyWeight(LevelDifficultySo, TapPadDifficultySo);

        var totalD = w.Total + difficultyValue;
        var timeD = w.GetTimeDifficulty(totalD);
        var wordD = w.GetWordDifficulty(totalD);
        var time = LevelDifficultySo.GetCountdownSecsByDifficulty(timeD);
        var word = LevelDifficultySo.GetRandomWordLengthByDifficulty(wordD);

        Debug.Log($"总权重：{totalD}，文字：{w.WordWeight}" + $"加时：{w.TimeWeight}]");
        
        Debug.Log(
            $"难度值：{totalD}，难度({wordD})-字数：{word.WordLength}难度[{word.DifficultyValue}]，" +
            $"难度({timeD})-加时：{time.ExtraSecs}难度[{time.DifficultyValue}]");
    }

    // 基于DifficultyWeight的难度权重生成字数和按键难度
    [Button]
    public void TestSettingByDifficultyWeight(float difficulty,int words)
    {
        var sets = LevelLoader.GetTapPadSettings(LevelDifficultySo, TapPadDifficultySo, difficulty, words);
        //打印设定
        print(string.Join(',',GetStrings(sets)));
    }

    private IEnumerable<string> GetStrings(WordDifficulty[] sets) => sets.Select((wd, i) => $"第{i}个字：{wd.Outline} - {wd.Item}");
}

public readonly struct WordDifficulty
{
    public float Outline { get; }
    public int Item { get; }

    public WordDifficulty(float outline, int item)
    {
        Outline = outline;
        Item = item;
    }
}

public readonly struct LevelDifficultyWeight
{
    public float MaxWord { get; }
    public float MaxTime { get; }
    public float Total => MaxWord + MaxTime;
    public float WordWeight => MaxWord / Total;
    public float TimeWeight => MaxTime / Total;

    public LevelDifficultyWeight(float maxWord, float time)
    {
        MaxWord = maxWord;
        MaxTime = time;
    }

    public float GetWordDifficulty(float difficultyValue) => WordWeight * difficultyValue;
    public float GetTimeDifficulty(float difficultyValue) => TimeWeight * difficultyValue;

    public override string ToString() => $"{{ MaxWord = {MaxWord}, MaxTime = {MaxTime} }}";
}