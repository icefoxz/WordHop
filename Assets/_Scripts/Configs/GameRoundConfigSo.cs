using UnityEngine;

[CreateAssetMenu(fileName = "GameRoundConfigSo", menuName = "配置/过关奖励")]
public class GameRoundConfigSo : ScriptableObject
{
    [SerializeField] private int 基础秒数 = 15;
    [SerializeField] private int 基础经验值 = 15;
    [SerializeField] private int 每提前一秒额外经验 = 4;
    [SerializeField] private float 二星百分比 = 50;
    [SerializeField] private float 三星百分比 = 80;

    public int BaseSeconds => 基础秒数; // 基础完成时间
    public int BaseExperience => 基础经验值; // 基础经验值
    public int ExtraExperiencePerSecond => 每提前一秒额外经验; // 每提前1秒获得的额外经验值

    private float Star_2Percentage => 二星百分比;
    private float Star_3Percentage => 三星百分比;


    // 计算总经验
    public int CalculateExperience(int secondsLeft, int wordLength)
    {
        var timeSaved = secondsLeft + WordLengthBonus(wordLength) - NonRewardSecs();
        return BaseExperience + Mathf.Clamp(timeSaved, 0, BaseSeconds) * ExtraExperiencePerSecond;
    }

    // 字母长度奖励
    private int WordLengthBonus(int wordLength)
    {
        return (wordLength - 3) * 2;//字数3奖励0秒, 4奖励2秒, 5奖励4秒, 6奖励6秒
    }

    // 不给予奖励的秒数
    private int NonRewardSecs() => BaseSeconds / 2;

    // 计算总金币
    public int CalculateStar(float currentScore, float maxScore)
    {
        var percentageScore = currentScore / maxScore * 100;
        if (percentageScore >= Star_3Percentage)
            return 3;
        if (percentageScore >= Star_2Percentage)
            return 2;
        return 1;
    }
}