using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "LevelDifficulty", menuName = "配置/难度/小关卡")]
public class LevelDifficultySo : ScriptableObject
{
    [SerializeField] private WordLengthDifficulty[] 字数配置;
    [SerializeField] private CountdownSecsDifficulty[] 挑战时长配置;
    private WordLengthDifficulty[] WordSet => 字数配置;
    private CountdownSecsDifficulty[] CountdownSet => 挑战时长配置;

    public CountdownSecsDifficulty GetCountdownSecsByDifficulty(float difficulty)
    {
        foreach (var item in CountdownSet
                     .Where(w => w.DifficultyValue <= difficulty)
                     .OrderByDescending(w => w.DifficultyValue))
        {
            if (item.DifficultyValue <= difficulty)
                return item;
        }
        return CountdownSet[0]; // 如果没有合适的配置，返回默认的配置
    }
    public WordLengthDifficulty GetWordLengthByDifficulty(float difficulty)
    {
        foreach (var item in WordSet
                     .Where(w => w.DifficultyValue <= difficulty)
                     .OrderByDescending(w => w.DifficultyValue))
        {
            if (item.DifficultyValue <= difficulty)
                return item;
        }
        return WordSet[0]; // 如果没有合适的配置，返回默认的配置
    }
    public WordLengthDifficulty GetRandomDifficultyByLength(int length)
    {
        foreach (var item in WordSet
                     .Where(w => w.WordLength <= length)
                     .OrderByDescending(_ => Random.Range(0, 1f)))
        {
            if (item.DifficultyValue <= length)
                return item;
        }
        return WordSet[0]; // 如果没有合适的配置，返回默认的配置
    }

    public WordLengthDifficulty GetRandomWordLengthByDifficulty(float difficulty)
    {
        foreach (var set in WordSet
                     .Where(w => w.DifficultyValue <= difficulty)
                     .OrderByDescending(_ => Random.Range(0, WordSet.Length)))
            return set;
        return WordSet[0]; // 如果没有合适的配置，返回默认的配置
    }

    public int GetMaxWords()=> WordSet.Max(w => w.WordLength);

    public float GetMaxTimeDifficulty()=> CountdownSet.Max(c => c.DifficultyValue);
}

[Serializable]
public class WordLengthDifficulty
{
    public float DifficultyValue; // 难度值
    public int WordLength; // 对应的字数
}

[Serializable]
public class CountdownSecsDifficulty
{
    public float DifficultyValue; // 难度值
    public int ExtraSecs; // 对应的倒计时秒数
}