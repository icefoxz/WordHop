using UnityEngine;

/// <summary>
/// 徽章读取器
/// </summary>
public class BadgeConfigLoader
{
    public static void LoadPrefab(BadgeConfiguration badgeConfiguration, GameObject badgePrefab)
    {
        foreach (var itemState in badgeConfiguration.GetData())
        {
            Transform[] allChildren = badgePrefab.GetComponentsInChildren<Transform>(true); // include inactive
            foreach (Transform child in allChildren)
            {
                if (child.name == itemState.name)
                {
                    child.gameObject.SetActive(itemState.isVisible);
                    break; // Assuming unique names, so break out once found
                }
            }
        }
    }
}