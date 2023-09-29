using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfigure", menuName = "配置/玩家/徽章等级")]
public class BadgeLevelSo : ScriptableObject
{
    [SerializeField] private BadgeConfiguration[] 徽章等级;
    private BadgeConfiguration[] Fields => 徽章等级;

    public BadgeConfiguration GetBadgeConfig(int level)
    {
        var badge = Fields.FirstOrDefault(b => b.Level == level);
        if (badge != null) return badge;
        Debug.LogError($"没有找到徽章等级{level}");
        return null;
    }
}