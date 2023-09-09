using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfigure", menuName = "����/���ְҵ")]
public class JobTreeSo : ScriptableObject
{
    [SerializeField] private string[] ְҵ����;
    [SerializeField] private JobTypeSo[] ְҵ;
    private string[] JobName => ְҵ����;
    private JobTypeSo[] JobType => ְҵ;
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
