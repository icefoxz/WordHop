using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RankConfigure", menuName = "配置/玩家等级")]
public class RankConfigSo : ScriptableObject
{
    [SerializeField] private string[] Title;

     public PlayerRank GetNextRank(PlayerRank rank)
    {
        return rank;
    }
}