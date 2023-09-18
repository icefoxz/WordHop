using System;
using System.Linq;
using AOT.Utl;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "LevelDifficulty", menuName = "配置/难度/小关卡")]
public class LevelDifficultySo : ScriptableObject
{
    //[SerializeField] private WordLengthDifficulty[] 字数配置;
    [SerializeField] private TextAsset 字数配置Json;
    [SerializeField] private CountdownSecsDifficulty[] 挑战时长配置;
    private WordLengthDifficulty[] WordSet => Json.Deserialize<WordLengthDifficulty[]>(WordSetJson);
    private CountdownSecsDifficulty[] CountdownSet => 挑战时长配置;
    private string WordSetJson => 字数配置Json.text;

    public CountdownSecsDifficulty GetCountdownSecsByDifficulty(float difficulty)
    {
        return CountdownSet
               .Where(w => w.DifficultyValue <= difficulty)
               .OrderByDescending(w => w.DifficultyValue)
               .FirstOrDefault() ?? CountdownSet[0]; // 如果没有合适的配置，返回默认的配置
    }

    public int GetWordLengthByDifficulty(float difficulty)
    {
        var item = GetWordLengthDifficultySet(difficulty);
        // 使用权重选择字数
        return WeightedRandomSelection(item.WordLengths, item.Weights);
    }

    public WordLengthDifficulty GetWordLengthDifficultySet(float difficulty)
    {
        return WordSet
               .Where(w => w.DifficultyValue <= difficulty)
               .OrderByDescending(w => w.DifficultyValue)
               .FirstOrDefault() ?? WordSet[0]; // 如果没有合适的配置，返回默认的配置
    }

    public WordLengthDifficulty GetClosestDifficultyForSpecifiedWordLength(float currentDifficulty, int specifiedWordLength)
    {
        WordLengthDifficulty closestDifficulty = null;
        var minDiff = float.MaxValue;

        foreach (var item in WordSet)
        {
            if (!item.WordLengths.Contains(specifiedWordLength)) continue;
            var diff = Mathf.Abs(item.DifficultyValue - currentDifficulty);
            if (diff < minDiff)
            {
                minDiff = diff;
                closestDifficulty = item;
            }
        }
        return closestDifficulty ?? WordSet[0];
    }

    public int GetMaxWords() => WordSet.SelectMany(w => w.WordLengths).Max();

    public float GetMaxTimeDifficulty() => CountdownSet.Max(c => c.DifficultyValue);

    private int WeightedRandomSelection(int[] options, float[] weights)
    {
        float totalWeight = weights.Sum();
        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        for (int i = 0; i < options.Length; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
                return options[i];
        }

        return options[0]; // Shouldn't be reached if weights are properly normalized.
    }
}

[Serializable]
public class WordLengthDifficulty
{
    public float DifficultyValue; // 难度值
    public int[] WordLengths; // 对应的字数数组
    public float[] Weights; // 字数数组中每个字数对应的权重
}

[Serializable]
public class CountdownSecsDifficulty
{
    public float DifficultyValue; // 难度值
    public int ExtraSecs; // 对应的倒计时秒数
}