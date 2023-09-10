using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfigure", menuName = "配置/玩家/职业树")]
public class JobTreeSo : ScriptableObject
{
    public enum JobTypes
    {
        Warriors,
        Mages,
        Elves,
        Rouges
    }

    [SerializeField]private JobTypeField[] 职业;
    private JobTypeField[] Fields => 职业;

    private JobTypeSo GetJobType(string jobName)
    {
        var job = Fields.FirstOrDefault(j => jobName.Equals(j.JobName));
        if (job != null) return job.JobSo;
        Debug.LogError($"没有找到职业{jobName}");
        return null;
    }

    public string GetJobLevelTitle(JobTypes type, int level)
    {
        return type switch
        {
            JobTypes.Warriors => GetJobLevelTitle("Warriors", level),
            JobTypes.Mages => GetJobLevelTitle("Mages", level),
            JobTypes.Elves => GetJobLevelTitle("Elves", level),
            JobTypes.Rouges => GetJobLevelTitle("Rouges", level),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public string GetJobLevelTitle(string jobName,int level)
    {
        var job = GetJobType(jobName).LevelSets.FirstOrDefault(j => j.Level == level);
        if (job != null) return job.Title;
        Debug.LogError($"没有找到职业{jobName}的{level}级称号");
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