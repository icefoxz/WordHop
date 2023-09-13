using log4net.Core;
using System.Linq;
using UnityEngine;

namespace GamePlay
{
    /// <summary>
    /// 关卡记录器, 用于记录关卡的分数, 关卡索引等
    /// </summary>
    public class PlayerModel : ModelBase
    {
        /// <summary>
        /// 用于不断攀升增加难度的数值
        /// </summary>
        public int StageLevelDifficultyIndex { get; private set; }
        private IPlayerLevelField[] Levels { get; set; }
        public UpgradingRecord UpgradeRecord { get; private set; }
        public PlayerLevel Level { get; private set; }
        public PlayerLevel HighestLevel { get; private set; }

        public PlayerUpgradeHandler UpgradeHandler { get; private set; }
        public int Exp => Level.Exp;

        public PlayerModel(PlayerLevel level, IPlayerLevelField[] levelFields)
        {
            Level = level;
            Levels = levelFields;
            UpgradeHandler = new PlayerUpgradeHandler(Levels);
        }

        public void SetHighestLevel(PlayerLevel level) => HighestLevel = level;

        public int GetScore() => Exp + Levels.Where(l => l.Level < Level.Level).Sum(l => l.MaxExp);

        public int GetPlayerLevel() => Level.Level;
        public int GetMaxExp(int level) => Levels.FirstOrDefault(l => l.Level == level)?.MaxExp ?? -1;
        public int GetMaxExpOfCurrentLevel() => UpgradeHandler.CurrentLevel.MaxExp;

        private void Upgrade(int score)
        {
            UpgradeRecord = UpgradeHandler.Upgrade(score);
            var level = UpgradeHandler.CurrentLevel.Level;
            var exp = UpgradeHandler.Exp;
            Level.AddScore(score);
            Level.AddCoin(score);
            Level.SetLevel(level, exp);
            var job = Game.ConfigureSo.JobTree.GetPlayerJob(Level.Job.JobType, level);
            Level.UpdateJob(job);
            SendEvent(GameEvents.Stage_Point_Update, score);
            CompareAndReplaceHighest();
        }

        private void CompareAndReplaceHighest()
        {
            if (HighestLevel == null || (Level != HighestLevel && Level.Score > HighestLevel.Score))
            {
                HighestLevel = Level;
                SendEvent(GameEvents.Stage_Highest_Level_Update, HighestLevel);
            }
        }

        public void SwitchJob(JobSwitch op)
        {
            UpgradeHandler.SetLevel(op.Level, 0);
            Level.SetLevel(op.Level, 0);
            var job = Game.ConfigureSo.JobTree.GetPlayerJob(op.JobType, op.Level);
            Level.UpdateJob(job);
            CompareAndReplaceHighest();
            SendEvent(GameEvents.Stage_Job_Update);
        }
        
        public void AddCoin(int coin)
        {
            Level.AddCoin(coin);
            SendEvent(GameEvents.Stage_Coin_Update, coin);
        }

        public void SetCoin(int coin)
        {
            Level.SetCoin(coin);
            SendEvent(GameEvents.Stage_Coin_Update, coin);
        }

        public void StageLevelPass(int point)
        {
            Upgrade(point);
            StageLevelDifficultyIndex++;
        }

        public void Reset()
        {
            StageLevelDifficultyIndex = 0;
            UpgradeHandler.Reset();
        }

        public (string title, Sprite sprite)? GetPlayerLevelInfo()
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
            return Game.ConfigureSo.JobTree.GetJobInfo(Level.Job.JobType, playerLevel);
        }
    }
}