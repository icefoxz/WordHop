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

    public JobSwitch[] GetJobSwitches(int level) => Jobs.Where(j => j.Level == level).Select(j=>j.JobSwitch).ToArray();

    [Serializable]private class JobSwitchField
    {
        public int Level;
        public JobSwitch JobSwitch;
    }
}
[Serializable]
public class JobSwitch
{
    public JobTypes JobType;
    public int Level;
    public int Cost;
    public string Message;
}