using System;
using System.IO;
using AOT.Utl;
using UnityEngine;

[CreateAssetMenu(fileName = "Badge_", menuName = "配置/玩家职业/徽章")]
public class BadgeConfiguration : ScriptableObject
{
    public int Level;
    public TextAsset JsonFile;
    public GoStruct[] GetData() => Json.Deserialize<GoStruct[]>(JsonFile.text);

    public struct GoStruct
    {
        public string name;
        public bool isVisible;
    }
}