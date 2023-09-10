using System.Collections;
using AOT.Views;
using UnityEngine;
using Sirenix.OdinInspector;

public class Test_BadgeLoader : MonoBehaviour
{
    public BadgeConfiguration badgeConfiguration;

    [Button, GUIColor("cyan")]
    public void ApplyBadgeState(GameObject badgePrefab) =>
        BadgeConfigLoader.LoadPrefab(badgeConfiguration, badgePrefab);

    [Button]public void TestFading(View view, float sec = 1f)
    {
        if(!Application.isPlaying)
            throw new System.Exception("Only available in play mode");
        var badge = new View_Badge(view);
        var half = sec / 2f;
        StartCoroutine(PlayFade(badge, half));
    }

    private IEnumerator PlayFade(View_Badge badge, float half)
    {
        yield return badge.PlayFadeIn(half);
        yield return badge.PlayFadeOut(half);
    }
}