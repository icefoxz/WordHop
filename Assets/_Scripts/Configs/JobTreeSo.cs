using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "JobTreeSo", menuName = "配置/玩家职业/职业树")]
public class JobTreeSo : ScriptableObject
{
    [SerializeField] private Sprite 职业图标;
    [SerializeField] private string 介绍;
    [SerializeField] private JobSwitchField[] 职业等级;

    public string Brief => 介绍;
    private JobSwitchField[] Jobs => 职业等级;
    public Sprite JobIcon => 职业图标;

    public JobSwitch[] GetJobSwitches(int level, int switchQuality) => Jobs
        .Where(j => j.Level == level && j.Quality <= switchQuality)
        .Select(j => j.JobSwitch).ToArray();

    [Serializable]private class JobSwitchField
    {
        public int Level;
        public int Quality;
        public JobSwitch JobSwitch;

        public static JobSwitchField Instance(JobTree a)
        {
            return new JobSwitchField
            {
                Level = a.SwitchLevel,
                Quality = a.SwitchQuality,
                JobSwitch = new JobSwitch
                {
                    Level = a.ToLevel,
                    Quality = a.ToQuality,
                    JobType = a.ToType,
                    Cost = a.Cost,
                    Message = a.Message,
                    CnMessage = a.CnMessage
                }
            };
        }
    }

    public void SetJobTree(JobTree[] jobTree)
    {
        var fields = jobTree.Select(JobSwitchField.Instance).ToArray();
        职业等级 = fields;
    }

    public class JobTree
    {
        public int FromTypeId { get; }
        public int SwitchLevel { get; }
        public int SwitchQuality { get; }
        public int ToTypeId { get; }
        public int ToLevel { get; }
        public int ToQuality { get; }
        public int Cost { get; }
        public string Message { get; }
        public string CnMessage { get; }

        public JobTypes FromType => (JobTypes)FromTypeId;
        public JobTypes ToType => (JobTypes)ToTypeId;

        public JobTree(string[] line)
        {
            FromTypeId = int.Parse(line[0]);
            SwitchLevel = int.Parse(line[1]);
            SwitchQuality = int.Parse(line[2]);
            ToTypeId = int.Parse(line[3]);
            ToLevel = int.Parse(line[4]);
            ToQuality = int.Parse(line[5]);
            Cost = int.Parse(line[6]);
            Message = line[7];
            CnMessage = line[8];
        }
    }
}

[Serializable]
public class JobSwitch
{
    public JobTypes JobType;
    public int Level;
    public int Quality = 1;
    public int Cost;
    public string Message;
    public string CnMessage;
}