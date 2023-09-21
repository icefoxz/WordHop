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
        public int GameStageIndex { get; private set; }
        private IPlayerLevelField[] Levels { get; set; }
        public UpgradingRecord UpgradeRecord { get; private set; }
        public PlayerRec Current { get; private set; }
        public PlayerRec HighestRec { get; private set; }
        private WordLevelModel WordLevel => Game.Model.WordLevel;

        public PlayerUpgradeHandler UpgradeHandler { get; private set; }
        public int Exp => Current.Exp;
        public int Coin => Current.Coin;
        public int Stars { get; private set; }

        public int LastCoinAdd { get; private set; }

        public PlayerModel(PlayerRec current, IPlayerLevelField[] levelFields)
        {
            Current = current;
            HighestRec = current;
            Levels = levelFields;
            UpgradeHandler = new PlayerUpgradeHandler(Levels);
        }

        public void SetHighestLevel(PlayerRec rec)
        {
            if (rec != null)
                HighestRec = rec;
        }

        public int GetScore() => Exp + Levels.Where(l => l.Level < Current.Level).Sum(l => l.MaxExp);

        public int GetPlayerLevel() => Current.Level;
        public int GetMaxExp(int level) => Levels.FirstOrDefault(l => l.Level == level)?.MaxExp ?? -1;
        public int GetMaxExpOfCurrentLevel() => UpgradeHandler.CurrentLevel.MaxExp;

        private void ScoreCalculation(int secs,int maxSecs,float difficulty, int missTake)
        {
            var rewardConfig = Game.ConfigureSo.GameRoundConfigSo;
            var point = secs - missTake;
            var reward = rewardConfig.GetRewardsByQuality(secs, maxSecs, difficulty);
            var currentLevel = Upgrade(reward.Exp);
            var adRatio = reward.AdRatio;
            var isLevelUp = UpgradeRecord.Levels.Count > 1;
            Stars = rewardConfig.CalculateStars(secs, WordLevel.TotalSeconds, difficulty);
            AddCoin(reward.Coin);
            Current.AddScore(reward.Exp);
            var job = Game.ConfigureSo.JobConfig.GetPlayerJob(Current.Job.JobType, currentLevel);
            Current.UpdateJob(job);
            SendEvent(GameEvents.Stage_Point_Update, point);
            if(isLevelUp) SendEvent(GameEvents.Player_Level_Up, currentLevel);
        }

        public void AddAdCoin() => AddCoin(LastCoinAdd /2);

        /// <summary>
        /// 一般用在内部调用, 除非Hack等级
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private int Upgrade(int exp)
        {
            UpgradeRecord = UpgradeHandler.Upgrade(exp);
            var currentLevel = UpgradeHandler.CurrentLevel.Level;
            var currentExp = UpgradeHandler.Exp;
            Current.SetLevel(currentLevel, currentExp);
            return currentLevel;
        }

        public void CompareAndReplaceHighest()
        {
            if (HighestRec == null || (Current != HighestRec && Current.Score > HighestRec.Score))
            {
                HighestRec = Current;
                SendEvent(GameEvents.Stage_Highest_Rec_Update, HighestRec);
            }
        }

        public void SwitchJob(JobSwitch op)
        {
            UpgradeHandler.SetLevel(op.Level, 0);
            Current.SetLevel(op.Level, 0);
            var job = Game.ConfigureSo.JobConfig.GetPlayerJob(op.JobType, op.Level);
            Current.UpdateJob(job);
            SendEvent(GameEvents.Stage_Job_Switch);
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
            var missTake = WordLevel.GetMissTakes();
            var difficulty = WordLevel.Difficulty;
            ScoreCalculation(secs, WordLevel.TotalSeconds,difficulty, missTake);
            GameStageIndex++;
        }

        public void Reset()
        {
            GameStageIndex = 0;
            UpgradeHandler.Reset();
        }

        public (string title, Sprite sprite)? GetPlayerLevelInfo()
        {
            var playerLevel = GetPlayerLevel();
            return Game.ConfigureSo.JobConfig.GetJobInfo(Current.Job.JobType, playerLevel);
        }

        public bool IsMaxLevel() => Current.Level >= Game.ConfigureSo.UpgradeConfigSo.GetMaxLevel();

#if UNITY_EDITOR
        public void HackUpgrade(int exp) => Upgrade(exp);
#endif
    }
}