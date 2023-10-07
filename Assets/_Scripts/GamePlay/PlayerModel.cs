using System;
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
        public int Days => GameStageIndex + 1;
        private IPlayerLevelField[] Levels { get; set; }
        public UpgradingRecord UpgradeRecord { get; private set; }
        public PlayerRec Current { get; private set; }
        public PlayerRec HighestRec { get; private set; }
        private WordLevelModel WordLevel => Game.Model.WordLevel;

        public PlayerUpgradeHandler UpgradeHandler { get; private set; }
        public int Exp => Current.Exp;
        public int Coin => Current.Coin;
        public int Stars { get; private set; }
        public int QualityLevel { get; private set; } = 1;

        public int AdCoin { get; private set; }
        public int LastAddedCoin { get; private set; }

        public PlayerModel(PlayerRec current, IPlayerLevelField[] levelFields, int qualityLevel)
        {
            Current = current;
            HighestRec = current;
            Levels = levelFields;
            UpgradeHandler = new PlayerUpgradeHandler(Levels);
            QualityLevel = qualityLevel;
        }

        public void SetHighestLevel(PlayerRec rec)
        {
            if (rec != null)
                HighestRec = rec;
        }

        private void ScoreCalculation(int secs, int maxSecs, float difficulty, int missTake)
        {
            var lastJob = GetPlayerCurrentJob();
            var rewardConfig = Game.ConfigureSo.GameRoundConfigSo;
            var point = secs - missTake;
            var reward = rewardConfig.GetRewardsByQuality(secs, maxSecs, difficulty);
            var currentLevel = Upgrade(reward.Exp);
            var isLevelUp = UpgradeRecord.Levels.Count > 1;
            var job = GetPlayerCurrentJob();
            Stars = rewardConfig.CalculateStars(secs, WordLevel.TotalSeconds, difficulty);
            var (exp, coin) = ResolveReward(reward, job.JobType);
            AdCoin = CountRewardAdCoin(reward, coin);
            AddCoin(coin);
            Current.AddScore(exp);
            Current.UpdateJob(job);
            SendEvent(GameEvents.Stage_Point_Update, point);
            if (isLevelUp) SendEvent(GameEvents.Player_Level_Up, currentLevel);
            if (job != lastJob) SendEvent(GameEvents.Player_Job_Switch, job);
        }

        private PlayerJob GetPlayerCurrentJob() =>
            Game.ConfigureSo.JobConfig.GetPlayerJob(Current.Job.JobType, Current.Level, QualityLevel);

        private static int CountRewardAdCoin(GameRoundConfigSo.RewardResult reward, int coin)
        {
            var result = (int)((reward.AdRatio - 1) * coin);
            return Math.Max(1, result);//最少加1金币
        }

        private (int exp, int coin) ResolveReward(GameRoundConfigSo.RewardResult reward, JobTypes jobType)
        {
            var config = Game.ConfigureSo.JobConfig;
            var expRatio = config.GetExpRatio(jobType);
            var coinRatio = config.GetCoinRatio(jobType);
            var exp = Count(expRatio, reward.Exp);
            var coin = Count(coinRatio, reward.Coin);
            return (exp, coin);

            int Count(float ratio, int value) => (int)Math.Max(0, value * ratio);
        }

        public void AddAdCoin() => AddCoin(AdCoin);

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

        public void SwitchJob(JobSwitch op, int exp = 0)
        {
            UpgradeHandler.SetLevel(op.Level, exp);
            Current.SetLevel(op.Level, exp);
            var newJob = Game.ConfigureSo.JobConfig.GetPlayerJob(op.JobType, op.Level, op.Quality);
            Current.UpdateJob(newJob);
            SendEvent(GameEvents.Player_Job_Switch);
        }
        
        public void AddCoin(int coin)
        {
            LastAddedCoin = coin;
            Current.AddCoin(coin);
            SendEvent(GameEvents.Stage_Coin_Update, coin);
        }

        public void StageLevelPass(int secs)
        {
            var missTake = WordLevel.GetMissTakes();
            var difficulty = WordLevel.Difficulty;
            ScoreCalculation(secs, WordLevel.TotalSeconds, difficulty, missTake);
            GameStageIndex++;
        }

        public void Reset()
        {
            GameStageIndex = 0;
            UpgradeHandler.Reset();
        }

        public bool IsMaxLevel() => Current.Level >= Game.ConfigureSo.UpgradeConfigSo.GetMaxLevel();

#if UNITY_EDITOR
        public void HackUpgrade(int exp) => Upgrade(exp);
#endif
        public BadgeConfiguration GetBadgeCfg() =>
            Game.ConfigureSo.BadgeLevelSo.GetBadgeConfig(QualityLevel);

        public void AddQuality(int quality)
        {
            var lastJob = GetPlayerCurrentJob();
            QualityLevel = Math.Clamp(QualityLevel + quality, 1, 20);
            var job = GetPlayerCurrentJob();
            if (job != lastJob)
            {
                Current.UpdateJob(job);
                SendEvent(GameEvents.Player_Job_Switch, job);
            }
            SendEvent(GameEvents.Stage_Quality_Update, QualityLevel);
        }
    }
}