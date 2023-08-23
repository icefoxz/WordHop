using System.Linq;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "TapPadDifficulty", menuName = "配置/难度/按键")]
public class TapPadDifficultySo : ScriptableObject
{
    [SerializeField]private TapPadDifficulty 难度配置;

    public TapPadDifficulty Difficulty => 难度配置;

    public WordDifficulty GetDifficultyValue(float difficultValue, float randomRange = 0) =>
        Difficulty.GetDifficultyValue(difficultValue, randomRange);
}

[Serializable]
public class TapPadDifficulty
{
    [SerializeField] private OutlineDifficulty[] _outlineSet;
    [SerializeField] private ItemSetting _itemSet;
    private OutlineDifficulty[] OutlineSet => _outlineSet;
    private ItemSetting ItemSet => _itemSet;

    public WordDifficulty GetDifficultyValue(float difficultValue, float randomRange = 0)
    {
        var dValue = difficultValue + Random.Range(-randomRange, randomRange);
        var outlines = OutlineSet.Where(o => o.DifficultyValue <= dValue).OrderByDescending(_ => Random.Range(0, 1f)).ToList();
        if (outlines.Count == 0) outlines.Add(OutlineSet.First());
        var outline = outlines[0];
        dValue -= outline.DifficultyValue;
        var arg = dValue <= 0
            ? new WordDifficulty(outline.Ratio, 0)
            : new WordDifficulty(outline.Ratio, ItemSet.GetValue(dValue) ? 
                (int)ItemSet.DifficultyValue : 0);
        return arg;
    }

    public float MaxDifficulty() =>
        OutlineSet.Max(o => o.DifficultyValue) + ItemSet.DifficultyValue;

    [Serializable]
    private class OutlineDifficulty
    {
        public float DifficultyValue;
        public float Ratio;
    }

    [Serializable]
    private class ItemSetting
    {
        public float Weight;
        public int Items;
        public float DifficultyValue;

        public bool GetValue(float dValue)
        {
            var random = Random.Range(0, 1f);
            if (random > Weight) return false;
            return dValue > DifficultyValue;
        }
    }
}