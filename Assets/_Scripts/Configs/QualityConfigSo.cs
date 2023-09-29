using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu (fileName = "QualityCfgSo", menuName = "配置/玩家/品质配置")]
public class QualityConfigSo: ScriptableObject
{
    [SerializeField] private QualityField 村民;
    [SerializeField] private QualityField 勇士;
    [SerializeField] private QualityField 神秘;
    [SerializeField] private QualityField 魔法师;
    [SerializeField] private QualityField 精灵;
    [SerializeField] private QualityField 死灵;
    private QualityField Warriors => 勇士;
    private QualityField Mages => 魔法师;
    private QualityField Elves => 精灵;
    private QualityField Villagers => 村民;
    private QualityField Mysterious => 神秘;
    private QualityField Necromancer => 死灵;

    public (Sprite icon, string brief, int cost, int quality)[] GetQualityOptions(JobTypes job)
    {
        return job switch
        {
            JobTypes.Villagers => Villagers.GetQualityOptions(),
            JobTypes.Warriors => Warriors.GetQualityOptions(),
            JobTypes.Mages => Mages.GetQualityOptions(),
            JobTypes.Elves => Elves.GetQualityOptions(),
            JobTypes.Necromancers => Necromancer.GetQualityOptions(),
            JobTypes.Mysterious => Mysterious.GetQualityOptions(),
            _ => throw new ArgumentOutOfRangeException(nameof(job), job, null)
        };
    }

    [Serializable] private class QualityField
    {
        [SerializeField] private Settings _high;
        [SerializeField] private Settings _mid;
        [SerializeField] private Settings _low;

        private Settings High => _high;
        private Settings Mid => _mid;
        private Settings Low => _low;

        public (Sprite icon, string brief, int cost, int quality)[] GetQualityOptions()
        {
            var array = new [] { (Mid,1), (Low,0), (High,2)};
            return array.Select((o) =>
            {
                var q = o.Item1;
                var i = o.Item2;
                return (q.Icon, q.Brief, q.Cost, i);
            }).ToArray();
        }
        [Serializable]private class Settings
        {
            [SerializeField] private Sprite _icon;
            [SerializeField] private string _brief;
            [SerializeField] private int _cost;

            public Sprite Icon => _icon;
            public string Brief => _brief;
            public int Cost => _cost;
        }
    }
}