using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JobType", menuName = "配置/玩家职业/职业")]
public class JobTypeSo : ScriptableObject
{
    [SerializeField] private JobField[] 职业等级;
    public JobField[] LevelSets => 职业等级;
    public JobTypeSo(JobField[] job)
    {
        职业等级 = job;
    }
}
[Serializable]
public class JobField
{
    [SerializeField] private int 等级;
    [SerializeField] private string 称号;
    public int Level => 等级;
    public string Title => 称号;
    public JobField(int level, string title)
    {
        等级 = Level;
        称号 = title;
    }
}
