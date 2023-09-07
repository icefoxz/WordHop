using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfigure", menuName = "����/���ְҵ")]
public class PlayerLevelConfigSo : ScriptableObject
{
    [SerializeField] private PlayerLevel[] ְҵ;
    private PlayerLevel[] Types => ְҵ;

    internal string GetClassLevelName(int level)
    {
        return Types[level].GetClassLevel(level);
    }

    [Serializable]
    private class PlayerLevel
    {
        [SerializeField] private string ְҵ��;
        [SerializeField] private LevelSet[] ְҵ�ȼ�;
        private string TypeName => ְҵ��;
        private LevelSet[] levelSets => ְҵ�ȼ�;

        internal string GetClassLevel(int level)
        {
            return levelSets[level].Title;
        }
    }

    [Serializable]
    private class LevelSet
    {
        [SerializeField] private int �ȼ�;
        [SerializeField] private string �ƺ�;
        public int Level => �ȼ�;
        public string Title => �ƺ�;

        public LevelSet(int level, string title)
        {
            �ȼ� = level;
            �ƺ� = title;
        }
    }
}
