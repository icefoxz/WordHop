using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JobType", menuName = "����/���ְҵ/ְҵ")]
public class JobTypeSo : ScriptableObject
{
    [SerializeField] private JobField[] ְҵ�ȼ�;
    public JobField[] LevelSets => ְҵ�ȼ�;
    public JobTypeSo(JobField[] job)
    {
        ְҵ�ȼ� = job;
    }
}
[Serializable]
public class JobField
{
    [SerializeField] private int �ȼ�;
    [SerializeField] private string �ƺ�;
    public int Level => �ȼ�;
    public string Title => �ƺ�;
    public JobField(int level, string title)
    {
        �ȼ� = Level;
        �ƺ� = title;
    }
}
