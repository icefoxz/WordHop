using System.Linq;
using UnityEngine;

/// <summary>
/// 关卡记录器, 用于记录关卡的分数, 关卡索引等
/// </summary>
public class StageModel : ModelBase
{
    public int StageLevelIndex { get; private set; }
    private PlayerLevel[] Levels { get; set; }
    public UpgradingRecord UpgradeRecord { get; private set; }

    public PlayerUpgradeHandler UpgradeHandler { get; private set; }
    private IPlayerLevelField CurrentLevel => UpgradeHandler.CurrentLevel;
    public int Exp => UpgradeHandler.Exp;

    public StageModel()
    {
        Levels = new PlayerLevel[]
        {
            new PlayerLevel(){Level = 1, MaxExp = 100},
            new PlayerLevel(){Level = 2, MaxExp = 200},
            new PlayerLevel(){Level = 3, MaxExp = 300},
            new PlayerLevel(){Level = 4, MaxExp = 400},
            new PlayerLevel(){Level = 5, MaxExp = 500},
            new PlayerLevel(){Level = 6, MaxExp = 600},
            new PlayerLevel(){Level = 7, MaxExp = 700},
            new PlayerLevel(){Level = 8, MaxExp = 800},
            new PlayerLevel(){Level = 9, MaxExp = 900},
            new PlayerLevel(){Level = 10, MaxExp = 1000},
            new PlayerLevel(){Level = 11, MaxExp = 1200},
            new PlayerLevel(){Level = 12, MaxExp = 1300},
            new PlayerLevel(){Level = 13, MaxExp = 1700},
            new PlayerLevel(){Level = 14, MaxExp = 2200},
            new PlayerLevel(){Level = 15, MaxExp = 2800},
            new PlayerLevel(){Level = 16, MaxExp = 3400},
            new PlayerLevel(){Level = 17, MaxExp = 4000},
            new PlayerLevel(){Level = 18, MaxExp = 5000},
            new PlayerLevel(){Level = 19, MaxExp = 6000},
            new PlayerLevel(){Level = 20, MaxExp = 7000},
        };
        UpgradeHandler = new PlayerUpgradeHandler(Levels);
    }

    public int GetScore() => Exp + Levels.Where(l => l.Level < CurrentLevel.Level).Sum(l => l.MaxExp);

    public int GetPlayerLevel() => CurrentLevel.Level;
    public int GetMaxExp(int level) => Levels.FirstOrDefault(l => l.Level == level)?.MaxExp ?? -1;
    public int GetMaxExpOfCurrentLevel() => CurrentLevel.MaxExp;

    private void Upgrade(int exp)
    {
        UpgradeRecord = UpgradeHandler.Upgrade(exp);
        SendEvent(GameEvents.Stage_Point_Update, exp);
    }

    public void StageLevelPass(int point)
    {
        Upgrade(point);
        StageLevelIndex++;
    }

    public void Reset()
    {
        StageLevelIndex = 0;
        UpgradeHandler.Reset();
    }

    private class PlayerLevel : IPlayerLevelField
    {
        public int Level { get; set; }
        public int MaxExp { get; set; }
    }

    public string GetPlayerTitle()
    {
        //var titles = new string[]
        //{
        //    "Novice Warrior", //初级战士 
        //    "Brawler", //徒手格斗者 
        //    "Ironclad Fighter", //铁甲战士 
        //    "Sword Dancer", //剑舞者 
        //    "Advanced Warrior", //高级战士 
        //    "Gladiator", //斗士 
        //    "Knight", //骑士 
        //    "Swordmaster", //剑圣 
        //    "Combat Mentor", //战斗导师 
        //    "Battlefield Commander", //战场统帅 
        //    "Elite Knight", //精英骑士 
        //    "Steel Warrior", //钢铁战士 
        //    "Guardian of Honor", //荣誉守护者 
        //    "War Hero", //战争英雄 
        //    "Master Warrior", //勇士大师 
        //    "Chosen of the War God", //战神选民 
        //    "Divine Swordsman", //神剑使者 
        //    "Great Warlord", //大战领袖 
        //    "Legendary Warrior", //传奇勇士 
        //    "Undefeated War King", //不败战王 
        //};
        var playerLevel = GetPlayerLevel();

        var title = Game.ConfigureSo.JobTree.GetJobLevelTitle(JobTreeSo.JobTypes.Warriors, playerLevel);//titles[playerLevel - 1];
        return title;
    }
}