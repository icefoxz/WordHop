using System.Linq;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRoundConfigSo", menuName = "配置/过关奖励")]
public class GameRoundConfigSo : ScriptableObject
{
    [SerializeField] private DifficultyRewardData[] 奖励数据;
    [SerializeField] private int 基础秒数 = 15;
    [SerializeField] private float 广告倍率;

    private float AdRatio => 广告倍率;
    private DifficultyRewardData[] Rewards => 奖励数据;
    public int BaseSeconds => 基础秒数;

    public DifficultyRewardData GetRewardDataByDifficulty(float difficultyValue)
    {
        // 根据浮点值选择相应的难度配置
        // 这里假设奖励数据数组已按难度值从低到高排序
        foreach (var data in 奖励数据)
        {
            if (difficultyValue <= data.DifficultyValue)
            {
                return data;
            }
        }
        return 奖励数据.LastOrDefault();  // 如果没有匹配的，返回最高难度的数据
    }

    public int CalculateStars(int seconds, int totalSecs ,float difficultyValue)
    {
        // 获取质量索引
        var qualityIndex = GetQualityIndex(seconds, totalSecs, difficultyValue);

        // 根据质量索引返回星星数
        return qualityIndex switch
        {
            0 => 3, // 高质量
            1 => 2, // 中质量
            2 => 1, // 低质量
            _ => 1
        };
    }

    // 获取质量索引, 0: 高质量, 1: 中质量, 2: 低质量
    private int GetQualityIndex(int seconds,int totalSecs, float difficultyValue)
    {
        var data = GetRewardDataByDifficulty(difficultyValue);
        var highRate = totalSecs - data.Seconds;
        if (seconds >= highRate) return 0; // 高质量
        var midRate = totalSecs / 3;
        if (seconds >= midRate) return 1; // 中质量
        return 2; // 低质量
    }

    public RewardResult GetRewardsByQuality(int seconds, int totalSecs, float difficultyValue)
    {
        var data = GetRewardDataByDifficulty(difficultyValue);
        int qualityIndex = GetQualityIndex(seconds, totalSecs, difficultyValue);

        RewardResult result = new RewardResult();

        switch (qualityIndex)
        {
            case 0: // 高质量
                result.Exp = data.Exp.High;
                result.Coin = data.Coin.High;
                break;
            case 1: // 中质量
                result.Exp = data.Exp.Middle;
                result.Coin = data.Coin.Middle;
                break;
            case 2: // 低质量
                result.Exp = data.Exp.Low;
                result.Coin = data.Coin.Low;
                break;
            default:
                break;
        }

        result.AdRatio = AdRatio;
        return result;
    }

    [Serializable]
    public class DifficultyRewardData
    {
        public float DifficultyValue;  // 0f - 1f
        public int Seconds; // 高质量(扣除)秒数
        public Quality Exp;
        public Quality Coin;

        [Serializable]
        public class Quality
        {
            public int Low;
            public int Middle;
            public int High;
        }
    }

    public class RewardResult
    {
        public int Exp;
        public int Coin;
        public float AdRatio;
    }
}
