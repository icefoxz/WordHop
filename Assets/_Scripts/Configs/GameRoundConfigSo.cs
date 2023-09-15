using UnityEngine;

[CreateAssetMenu(fileName = "GameRoundConfigSo", menuName = "配置/过关奖励")]
public class GameRoundConfigSo : ScriptableObject
{
    [SerializeField] private int 基础秒数 = 15;
    [SerializeField] private int 基础经验值 = 15;
    [SerializeField] private int 每提前一秒额外经验 = 4;
    [SerializeField] private int 基础金币 = 0;
    [SerializeField] private float 提前每秒金币倍数 = 6f;

    public int BaseSeconds => 基础秒数; // 基础完成时间
    public int BaseExperience => 基础经验值; // 基础经验值
    public int ExtraExperiencePerSecond => 每提前一秒额外经验; // 每提前1秒获得的额外经验值
    public int BaseCoins => 基础金币; // 基础金币
    public float ExtraCoinsMultiplier => 提前每秒金币倍数; // 提前完成每秒金币的倍数增长

    // 计算总经验
    public int CalculateExperience(int secondsLeft)
    {
        var timeSaved = secondsLeft - NonRewardSecs();
        return BaseExperience + Mathf.Clamp(timeSaved, 0, BaseSeconds) * ExtraExperiencePerSecond;
    }

    private int NonRewardSecs() => BaseSeconds / 3;

    // 计算总金币
    public int CalculateCoins(int secondsLeft)
    {
        var timeSaved = secondsLeft - NonRewardSecs();
        if (timeSaved <= 0) return BaseCoins;

        return BaseCoins + timeSaved * Mathf.RoundToInt(ExtraCoinsMultiplier);
    }
}