using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "JobTreeSo", menuName = "配置/玩家职业/职业树Csv读取器")]
public class JobTreeCompiler : ScriptableObject
{
    [SerializeField] private TextAsset _jobTreeCsv;
    [SerializeField] private JobTreeSo _villagers;
    [SerializeField] private JobTreeSo _warriors;
    [SerializeField] private JobTreeSo _mysterious;
    [SerializeField] private JobTreeSo _mages;
    [SerializeField] private JobTreeSo _elves;
    [SerializeField] private JobTreeSo _necromancers;

    private JobTreeSo Villagers => _villagers;
    private JobTreeSo Warriors => _warriors;
    private JobTreeSo Mysterious => _mysterious;
    private JobTreeSo Mages => _mages;
    private JobTreeSo Elves => _elves;
    private JobTreeSo Necromancers => _necromancers;
    private TextAsset JobTreeCsv => _jobTreeCsv;

    [Button(ButtonSizes.Large),GUIColor("Cyan")]public void CompileToSo()
    {
        var lines = JobTreeCsv.text.Split('\n').Where(s=>!string.IsNullOrWhiteSpace(s)).ToArray();
        var jobGroup = lines.Select(l => new JobTree(l.Split(','))).GroupBy(j=>j.FromType);
        var jobs = 0;
        var records = 0;
        foreach (var group in jobGroup)
        {
            var job = group.Key;
            var jobTree = group.ToArray();
            SetJobTree(job, jobTree);
            jobs++;
            records += jobTree.Length;
        }
        Debug.Log($"编译完成，共编译了{jobs}个职业，{records}条记录");
    }

    private void SetJobTree(JobTypes job, JobTree[] jobTree)
    {
        JobTreeSo so = job switch
        {
            JobTypes.Villagers => Villagers,
            JobTypes.Warriors => Warriors,
            JobTypes.Mysterious => Mysterious,
            JobTypes.Mages => Mages,
            JobTypes.Elves => Elves,
            JobTypes.Necromancers => Necromancers,
            _ => throw new ArgumentOutOfRangeException(nameof(job), job, null)
        };
        so.SetJobTree(jobTree);
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