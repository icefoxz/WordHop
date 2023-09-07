using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfigure", menuName = "配置/玩家职业")]
public class PlayerLevelConfigSo : ScriptableObject
{
    [SerializeField] private PlayerLevel[] 职业;
    private PlayerLevel[] Types => 职业;

    internal string GetClassLevelName(int level)
    {
        return Types[level].GetClassLevel(level);
    }

    [Serializable]
    private class PlayerLevel
    {
        [SerializeField] private string 职业名;
        [SerializeField] private LevelSet[] 职业等级;
        private string TypeName => 职业名;
        private LevelSet[] levelSets => 职业等级;

        internal string GetClassLevel(int level)
        {
            return levelSets[level].Title;
        }
    }

    [Serializable]
    private class LevelSet
    {
        [SerializeField] private int 等级;
        [SerializeField] private string 称号;
        public int Level => 等级;
        public string Title => 称号;

        public LevelSet(int level, string title)
        {
            等级 = level;
            称号 = title;
        }
    }
}
