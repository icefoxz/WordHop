using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "JobTreeSo", menuName = "配置/玩家/职业树")]
public class JobConfigSo : ScriptableObject
{
    [SerializeField]private JobTypeField[] 职业;
    [SerializeField]private JobTreeSo 村民;
    [SerializeField]private JobTreeSo 勇士;
    [SerializeField]private JobTreeSo 神秘;
    [SerializeField]private JobTreeSo 魔法师;
    [SerializeField]private JobTreeSo 精灵;
    [SerializeField] private JobTreeSo 死灵;
    private JobTypeField[] Fields => 职业;
    private JobTreeSo Warriors => 勇士;
    private JobTreeSo Mages => 魔法师;
    private JobTreeSo Elves => 精灵;
    private JobTreeSo Villagers => 村民;
    private JobTreeSo Mysterious => 神秘;
    private JobTreeSo Necromancer => 死灵;

    private JobTypeSo GetJobType(string jobName)
    {
        var job = Fields.FirstOrDefault(j => jobName.Equals(j.JobName));
        if (job != null) return job.JobSo;
        Debug.LogError($"没有找到职业{jobName}");
        return null;
    }

    public float GetCoinRatio(JobTypes jobType) => GetField(jobType).JobSo.CoinRewardRatio;
    public float GetExpRatio(JobTypes jobType) => GetField(jobType).JobSo.ExpRewardRatio;

    public PlayerJob GetPlayerJob(JobTypes type, int level,int quality)
    {
        var job = GetField(type);
        var levelSet = job.JobSo.LevelSets
            .Where(j => j.Level <= level && j.MinQuality <= quality)
            .OrderByDescending(j => j.Level)
            .ThenByDescending(j => j.MinQuality)
            .FirstOrDefault();
        if(levelSet == null)
            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        return new PlayerJob(levelSet.Id, levelSet.Title, level, type, quality);
    }

    public Dictionary<JobTypes, CardArg[]> Data()=>
        new Dictionary<JobTypes, CardArg[]>
        {
            {JobTypes.Warriors, GetCardArgs(JobTypes.Warriors)},
            {JobTypes.Mages, GetCardArgs(JobTypes.Mages)},
            {JobTypes.Elves, GetCardArgs(JobTypes.Elves)},
            {JobTypes.Villagers, GetCardArgs(JobTypes.Villagers)},
            {JobTypes.Mysterious, GetCardArgs(JobTypes.Mysterious)},
            {JobTypes.Necromancers, GetCardArgs(JobTypes.Necromancers)},
        };

    private CardArg[] GetCardArgs(JobTypes type)
    {
        var field = GetField(type);
        return field.JobSo.LevelSets
            .Select(s => new CardArg(s.Id,s.Title, s.Level, GetStars(s.Level), s.Icon, GetJobSwitches(type,s.Level)))
            .ToArray();
    }

    private JobSwitch[] GetJobSwitches(JobTypes type, int level) =>
        GetJobTree(type).GetJobSwitches(level).ToArray();

    private JobTreeSo GetJobTree(JobTypes type)
    {
        return type switch
        {
            JobTypes.Villagers => Villagers,
            JobTypes.Warriors => Warriors,
            JobTypes.Mysterious => Mysterious,
            JobTypes.Mages => Mages,
            JobTypes.Elves => Elves,
            JobTypes.Necromancers => Necromancer,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public CardArg GetCardArg(PlayerJob job)=> GetCardArg(job.JobType, job.Level, job.Quality);

    public CardArg GetCardArg(JobTypes type, int level, int quality)
    {
        var job = GetField(type);
        var levelSet = job.JobSo.LevelSets.Where(j => j.Level <= level && j.MinQuality <= quality)
            .OrderByDescending(j => j.Level)
            .ThenByDescending(j => j.MinQuality)
            .FirstOrDefault();
        if (levelSet == null)
            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        return new CardArg(levelSet.Id, levelSet.Title, level, GetStars(level), levelSet.Icon,
            GetJobSwitches(type, levelSet.Level));
    }

    private int GetStars(int level)
    {
        return 1 + level / 3;
    }

    private JobTypeField GetField(JobTypes jobTypes)
    {
        var jobName = GetJobName(jobTypes);
        var job = Fields.FirstOrDefault(j => jobName.Equals(j.JobName));
        if (job != null) return job;
        Debug.LogError($"没有找到职业{jobName}");
        return null;
    }

    private static string GetJobName(JobTypes type)
    {
        return type switch
        {
            JobTypes.Warriors => "Warriors",
            JobTypes.Mages => "Mages",
            JobTypes.Elves => "Elves",
            JobTypes.Villagers => "Villagers",
            JobTypes.Mysterious => "Mysterious",
            JobTypes.Necromancers => "Necromancer",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public Sprite GetJobIcon(JobTypes type) => GetJobTree(type).JobIcon;

    [Serializable]private class JobTypeField
    {
        [SerializeField] private string 职业名称;
        [SerializeField] private JobTypeSo 职业;
        public string JobName => 职业名称;
        public JobTypeSo JobSo => 职业;
    }

    public string GetJobBrief(JobTypes jobType) => GetJobTree(jobType).Brief;
}
