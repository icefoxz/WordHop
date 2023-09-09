using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfigure", menuName = "配置/玩家职业")]
public class JobTreeSo : ScriptableObject
{
    [SerializeField] private string[] 职业名称;
    [SerializeField] private JobTypeSo[] 职业;
    private string[] JobName => 职业名称;
    private JobTypeSo[] JobType => 职业;
    private JobTypeSo CurrentJob { get; set; }
  
    public string GetJobType(int job)
    {
        if(job < 0 || job >= JobName.Length) return null;
        var job_name = JobName[job];
        CurrentJob = JobType[job];
        return job_name;
    }
    public string GetJobLevelTitle(int level)
    {
        var job = CurrentJob;
        if (level < 0 || level > job.LevelSets.Length - 1) return null;
        return job.LevelSets[level].Title;
    }
}
