using System.Linq;

/// <summary>
/// 玩家等级类, 主要提供等级提升的方法, 基于<see cref="IPlayerLevelField"/>和<see cref="UpgradingRecord"/>的数据结构
/// </summary>
public class PlayerUpgradeHandler
{
    public int Exp { get; protected set; }
    public IPlayerLevelField CurrentLevel { get; protected set; }
    public IPlayerLevelField[] Levels { get; }

    public PlayerUpgradeHandler(IPlayerLevelField[] levels)
    {
        if(levels == null || levels.Length == 0)
            throw new System.ArgumentException("levels can not be null or empty");
        Levels = levels;
    }

    public UpgradingRecord Upgrade(int exp)
    {
        var r = new UpgradingRecord(exp);
        int remainingExp = Exp + exp;  // 合并原有经验和新增经验
        IPlayerLevelField[] levels = GetNextLevels();
        for (int i = 0; i < levels.Length; i++)
        {
            var level = levels[i];
            CurrentLevel = level;
            if (i == levels.Length - 1) // 如果已经是最大等级，直接添加经验然后退出循环
            {
                Exp += remainingExp;
                r.AddRecord(level.MaxExp, level.MaxExp, CurrentLevel.Level, CurrentLevel.MaxExp);
                break;
            }
            if (remainingExp < level.MaxExp)// 如果剩余经验不足以升级到下一级
            {
                r.AddRecord(Exp, remainingExp, level.Level, level.MaxExp);
                Exp = remainingExp;
                break;
            }
            // 升级到下一级
            r.AddRecord(Exp, level.MaxExp, level.Level, level.MaxExp);
            Exp = 0;
            remainingExp -= level.MaxExp;
        }
        return r;
    }

    private IPlayerLevelField[] GetNextLevels() => Levels.Where(l => l.Level >= CurrentLevel.Level).OrderBy(l => l.Level).ToArray();

    public void Reset()
    {
        Exp = 0;
        CurrentLevel = Levels[0];
    }

    public void SetLevel(int level,int exp)
    {
        Exp = exp;
        CurrentLevel = Levels.First(l => l.Level == level);
    }
}
/// <summary>
/// 玩家等级的基本记录
/// </summary>
public interface IPlayerLevelField
{
    int Level { get; }
    int MaxExp { get; }
}