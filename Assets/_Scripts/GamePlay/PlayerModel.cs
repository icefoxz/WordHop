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
        public PlayerRec Current { get; private set; }
        public PlayerRec HighestRec { get; private set; }
        private WordLevelModel WordLevel => Game.Model.WordLevel;

        public PlayerUpgradeHandler UpgradeHandler { get; private set; }
        public int Exp => Current.Exp;
        public int Coin => Current.Coin;
        public int LastCoinAdd { get; private set; }

        public PlayerModel(PlayerRec current, IPlayerLevelField[] levelFields)
        {
            Current = current;
            Levels = levelFields;
            UpgradeHandler = new PlayerUpgradeHandler(Levels);
        }

        public void SetHighestLevel(PlayerRec current) => HighestRec = current;

        public int GetScore() => Exp + Levels.Where(l => l.Level < Current.Level).Sum(l => l.MaxExp);

        public int GetPlayerLevel() => Current.Level;
        public int GetMaxExp(int level) => Levels.FirstOrDefault(l => l.Level == level)?.MaxExp ?? -1;
        public int GetMaxExpOfCurrentLevel() => UpgradeHandler.CurrentLevel.MaxExp;

        private void Upgrade(int missTake, int secs)
        {
            var point = secs - missTake;
            var upgradeExp = Game.ConfigureSo.GameRoundConfigSo.CalculateExperience(point);
            var coin = Game.ConfigureSo.GameRoundConfigSo.CalculateCoins(point);
            UpgradeRecord = UpgradeHandler.Upgrade(upgradeExp);
            var level = UpgradeHandler.CurrentLevel.Level;
            var exp = UpgradeHandler.Exp;
            AddCoin(coin);
            Current.AddScore(point);
            Current.SetLevel(level, exp);
            var job = Game.ConfigureSo.JobTree.GetPlayerJob(Current.Job.JobType, level);
            Current.UpdateJob(job);
            SendEvent(GameEvents.Stage_Point_Update, point);
            CompareAndReplaceHighest();
        }

        private void CompareAndReplaceHighest()
        {
            if (HighestRec == null || (Current != HighestRec && Current.Score > HighestRec.Score))
            {
                HighestRec = Current;
                SendEvent(GameEvents.Stage_Highest_Level_Update, HighestRec);
            }
        }

        public void SwitchJob(JobSwitch op)
        {
            UpgradeHandler.SetLevel(op.Level, 0);
            Current.SetLevel(op.Level, 0);
            var job = Game.ConfigureSo.JobTree.GetPlayerJob(op.JobType, op.Level);
            Current.UpdateJob(job);
            CompareAndReplaceHighest();
            SendEvent(GameEvents.Stage_Job_Update);
        }
        
        public void AddCoin(int coin)
        {
            LastCoinAdd = coin;
            Current.AddCoin(coin);
            SendEvent(GameEvents.Stage_Coin_Update, coin);
        }

        //public void SetCoin(int coin)
        //{
        //    Current.SetCoin(coin);
        //    SendEvent(GameEvents.Stage_Coin_Update, coin);
        //}

        public void StageLevelPass(int secs)
        {
            var missTake = (int)WordLevel.GetMissTakeAve();
            Upgrade(missTake, secs);
            StageLevelDifficultyIndex++;
        }

        public void Reset()
        {
            StageLevelDifficultyIndex = 0;
            UpgradeHandler.Reset();
        }

        public (string title, Sprite sprite)? GetPlayerLevelInfo()
        {
            var playerLevel = GetPlayerLevel();
            return Game.ConfigureSo.JobTree.GetJobInfo(Current.Job.JobType, playerLevel);
        }
    }
}