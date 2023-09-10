using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class BadgeManager : MonoBehaviour
{
    public BadgeConfiguration badgeConfiguration;

    [Button,GUIColor("cyan")] public void ApplyBadgeState(GameObject badgePrefab)
    {
        foreach (var itemState in badgeConfiguration.badgeItems)
        {
            Transform[] allChildren = badgePrefab.GetComponentsInChildren<Transform>(true); // include inactive
            foreach (Transform child in allChildren)
            {
                if (child.name == itemState.itemName)
                {
                    child.gameObject.SetActive(itemState.isVisible);
                    break; // Assuming unique names, so break out once found
                }
            }
        }
    }

    [Button,GUIColor("red")] public void SaveBadgeState(GameObject badgePrefab)
    {
        // First resolve any duplicate names
        Dictionary<string, int> nameCount = new Dictionary<string, int>();
        ResolveDuplicateNames(badgePrefab.transform, nameCount);

        // Then save the state as before
        badgeConfiguration.badgeItems.Clear();
        RecursivelySaveState(badgePrefab.transform);
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

    private void RecursivelySaveState(Transform currentTransform)
    {
        Debug.Log($"currentTransform.name: {currentTransform.name}");
        int count = 0;
        BadgeConfiguration.BadgeItemState newItemState = new BadgeConfiguration.BadgeItemState
        {
            itemName = currentTransform.name,
            isVisible = currentTransform.gameObject.activeSelf
        };
        badgeConfiguration.badgeItems.Add(newItemState);

        foreach (Transform child in currentTransform)
        {
            count++;
            RecursivelySaveState(child);
        }
        Debug.Log($"count: {count}");
    }

}