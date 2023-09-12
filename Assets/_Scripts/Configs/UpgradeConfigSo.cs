using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeCfgSo", menuName = "配置/玩家/升等配置")]
public class UpgradeConfigSo : ScriptableObject
{
    [SerializeField]private UpgradeLevel[] 升等配置;
    private UpgradeLevel[] Levels => 升等配置;

    public IPlayerLevelField[] GetLevels() => Levels;

    [Serializable]
    private class UpgradeLevel : IPlayerLevelField
    {
        [SerializeField] private int 等级;
        [SerializeField] private int 经验值;

        public int Level => 等级;
        public int MaxExp => 经验值;
    }
}