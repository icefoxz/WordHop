using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "JobTreeSo", menuName = "配置/玩家/职业树")]
public class JobTreeSo : ScriptableObject
{
    [SerializeField]private JobTypeField[] 职业;
    private JobTypeField[] Fields => 职业;

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

    public CardArg GetCardArg(PlayerJob job)=> GetCardArg(job.JobType, job.Level);

    public CardArg GetCardArg(JobTypes type, int level)
    {
        var job = GetField(type);
        var levelSet = job.JobSo.LevelSets.FirstOrDefault(j => j.Level == level);
        if(levelSet == null)
            throw new ArgumentOutOfRangeException(nameof(level), level, null);
        return new CardArg(levelSet.Title, level, GetStars(level), levelSet.Icon, levelSet.JobSwitches);
    }

    private int GetStars(int level) => level / 2;

    private JobTypeField GetField(JobTypes jobTypes)
    {
        var jobName = GetJobName(jobTypes);
        var job = Fields.FirstOrDefault(j => jobName.Equals(j.JobName));
        if (job != null) return job;
        Debug.LogError($"没有找到职业{jobName}");
        return null;
    }

    private string GetJobName(JobTypes type)
    {
        return type switch
        {
            JobTypes.Warriors => "Warriors",
            JobTypes.Mages => "Mages",
            JobTypes.Elves => "Elves",
            JobTypes.Rouges => "Rouges",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public (string title, Sprite sprite)? GetJobInfo(JobTypes type, int level) => GetJobInfo(GetJobName(type), level);

    public Sprite GetJobIcon(JobTypes type, int level)
    {
        var job = GetJobType(GetJobName(type)).LevelSets.FirstOrDefault(j => j.Level == level);
        if (job != null) return job.Icon;
        Debug.LogError($"没有找到职业{type}的{level}级设定");
        return null;
    }

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
}