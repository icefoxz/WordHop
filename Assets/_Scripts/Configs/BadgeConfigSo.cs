using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BadgeConfigure", menuName = "配置/玩家职业/徽章")]
public class BadgeConfiguration : ScriptableObject
{
    public List<BadgeItemState> badgeItems = new List<BadgeItemState>();

    [Serializable]
    public class BadgeItemState
    {
        public string itemName;
        public bool isVisible;
    }
}