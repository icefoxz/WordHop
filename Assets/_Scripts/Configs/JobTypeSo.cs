using System;
using UnityEngine;

[CreateAssetMenu(fileName = "JobType", menuName = "配置/玩家职业/职业")]
public class JobTypeSo : ScriptableObject
{
    [SerializeField] private JobField[] 职业等级;
    public JobField[] LevelSets => 职业等级;
}
[Serializable]
public class JobField
{
    [SerializeField] private int 等级;
    [SerializeField] private string 称号;
    [SerializeField] private Sprite 图标;
    public int Level => 等级;
    public string Title => 称号;
    public Sprite Icon => 图标;
}
