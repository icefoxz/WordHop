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

    public PlayerJob GetPlayerJob(JobTypes type, int level)
    {
        var job = GetField(type);
        var levelSet = job.JobSo.LevelSets.FirstOrDefault(j => j.Level == level);
        if(levelSet == null)
            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        return new PlayerJob(levelSet.Title, level, type);
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
            .Select(s => new CardArg(s.Title, s.Level, GetStars(s.Level), s.Icon, GetJobSwitches(type,s.Level)))
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

    public CardArg GetCardArg(PlayerJob job)=> GetCardArg(job.JobType, job.Level);

    public CardArg GetCardArg(JobTypes type, int level)
    {
        var job = GetField(type);
        var levelSet = job.JobSo.LevelSets.FirstOrDefault(j => j.Level == level);
        if (levelSet == null)
            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        return new CardArg(levelSet.Title, level, GetStars(level), levelSet.Icon, GetJobSwitches(type, levelSet.Level));
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

    public (string title, Sprite sprite)? GetJobInfo(JobTypes type, int level) => GetJobInfo(GetJobName(type), level);

    public Sprite GetJobIcon(JobTypes type) => GetJobTree(type).JobIcon;

    public (string title, Sprite sprite)? GetJobInfo(string jobName,int level)
    {
        var job = GetJobType(jobName).LevelSets.FirstOrDefault(j => j.Level == level);
        if (job != null) return (job.Title, job.Icon);
        Debug.LogError($"没有找到职业{jobName}的{level}级设定");
        return null;
    }

    [Serializable]private class JobTypeField
    {
        [SerializeField] private string 职业名称;
        [SerializeField] private JobTypeSo 职业;
        public string JobName => 职业名称;
        public JobTypeSo JobSo => 职业;
    }

    public string GetJobBrief(JobTypes jobType) => GetJobTree(jobType).Brief;
}
