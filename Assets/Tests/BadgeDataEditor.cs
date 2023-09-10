#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using AOT.Utl;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class BadgeDataEditor : MonoBehaviour
{
    public BadgeConfiguration badgeConfiguration;

    [Button, GUIColor("red")]
    public void SaveBadgeState(GameObject badgePrefab)
    {
        // First resolve any duplicate names
        Dictionary<string, int> nameCount = new Dictionary<string, int>();
        ResolveDuplicateNames(badgePrefab.transform, nameCount);

        var list = new List<BadgeConfiguration.GoStruct>();
        // Then save the state as before
        RecursivelySaveState(list, badgePrefab.transform);
        // 保存更改到磁盘
        var paths =AssetDatabase.GetAssetPath(badgeConfiguration).Split('.');
        var path = Path.Combine($"{paths[0]}.bytes");
        var jsonData = Json.Serialize(list);
        File.WriteAllText(path, jsonData);
        AssetDatabase.Refresh(); // 保存所有修改
    }
    private void ResolveDuplicateNames(Transform currentTransform, Dictionary<string, int> nameCount)
    {
        // Check if the name exists in the dictionary
        if (nameCount.ContainsKey(currentTransform.name))
        {
            nameCount[currentTransform.name]++;
            currentTransform.name += "~" + nameCount[currentTransform.name];
        }
        else
        {
            nameCount[currentTransform.name] = 0;
        }

        // Recursively call this function for each child
        foreach (Transform child in currentTransform)
        {
            ResolveDuplicateNames(child, nameCount);
        }
    }

    private void RecursivelySaveState(List<BadgeConfiguration.GoStruct> list, Transform currentTransform)
    {
        Debug.Log($"currentTransform.name: {currentTransform.name}");
        int count = 0;
        var newItemState = new BadgeConfiguration.GoStruct()
        {
            name = currentTransform.name,
            isVisible = currentTransform.gameObject.activeSelf
        };
        list.Add(newItemState);

        foreach (Transform child in currentTransform)
        {
            count++;
            RecursivelySaveState(list, child);
        }
        Debug.Log($"count: {count}");
    }
}
#endif