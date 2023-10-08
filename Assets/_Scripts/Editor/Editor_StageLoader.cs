#if UNITY_EDITOR
using System.Linq;
using AOT.Utl;
using AOT.Views;
using Sirenix.OdinInspector;
using UnityEngine;

public class Editor_StageLoader : MonoBehaviour
{
    [SerializeField] private TextAsset 按键数据;
    [SerializeField] private TextAsset 布局;
    [SerializeField] private View 预设物;
    [SerializeField] private Transform 目标父物件;
    [SerializeField] private SpriteContainerSo spriteContainer;

    [Button(ButtonSizes.Large, Name = "加载"), GUIColor(1f, 0.8f, 0)]
    private void RestoreLevel()
    {
        if (TapPadConfig.ValidateTapHops(预设物))
            Debug.Log("预设物验证通过!");
        else
            Debug.LogError("预设物验证失败");
        
        var btnCfg = Json.Deserialize<LevelConfig>(按键数据.text);
        var layoutCfg = Json.Deserialize<LayoutConfig>(布局.text);
        if (btnCfg.tapPads.Count != layoutCfg.Rects.Count)
            Debug.LogError($"按键[{btnCfg.tapPads.Count}]与布局[{layoutCfg.Rects.Count}]不匹配");
        var spMapper = spriteContainer.Data.ToDictionary(s => s.name, s => s);
        for (var i = 0; i < btnCfg.tapPads.Count; i++)
        {
            var tapHopConfig = btnCfg.tapPads[i];
            var rectConfig = layoutCfg.Rects[i];
            var correspondingView = Instantiate(预设物, 目标父物件.transform);
            if (correspondingView)
            {
                tapHopConfig.Apply(correspondingView, spMapper);
                rectConfig.Apply(correspondingView.RectTransform);
            }
            else
                Debug.LogWarning($"No matching View found for TapHopConfig with order {tapHopConfig.clickOrder}");
        }
    }

    [Button(ButtonSizes.Medium),GUIColor("RED")]private void ClearPrefabs()
    {
        foreach (Transform obj in 目标父物件)
        {
            GameObject.Destroy(obj.gameObject);
        }
        Debug.Log("布局清除!", 目标父物件);
    }
}
#endif