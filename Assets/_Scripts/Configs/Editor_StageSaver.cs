#if UNITY_EDITOR
using System;
using AOT.Views;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using AOT.Utl;
using UnityEngine.Serialization;


//[CreateAssetMenu(fileName = "配置器",menuName = "关卡/配置器")]
public class Editor_StageSaver : MonoBehaviour
{
    [SerializeField]private int 关卡秒数 = 10;
    [SerializeField][FormerlySerializedAs("tapHops")]private List<View> tapPads = new List<View>();

    [Button(ButtonSizes.Large, Name = "保存固定键配置", Style = ButtonStyle.Box, Expanded = true),
     GUIColor(0.4f, 0.8f, 1)]
    private void SaveFixButtons(string 关卡名 = "level")
    {
        VerifyList();
        var levelConfig = new LevelConfig
        {
            countdownTime = 关卡秒数,
        };
        for (var index = 0; index < tapPads.Count; index++)
        {
            var tapHop = tapPads[index];
            var tapHopConfig = TapPadConfig.Create(tapHop);
            tapHopConfig.clickOrder = index + 1;
            levelConfig.tapPads.Add(tapHopConfig);
        }

        var json = Json.Serialize(levelConfig);
        System.IO.File.WriteAllText($"Assets/Configs/Levels/{关卡名}.json", json);
        AssetDatabase.Refresh();
    }

    private void VerifyList()
    {
        if (tapPads.Any(t => !TapPadConfig.ValidateTapHops(t))) 
            Debug.LogError("无法储存关卡, 预设物验证失败!");
    }

    [Button(ButtonSizes.Large, Name = "保存布局", Style = ButtonStyle.Box, Expanded = true),
     GUIColor(1f, 0.8f, 0)]
    private void SaveLayout(string 布局名 = "layout")
    {
        VerifyList();
        var layoutConfig = new LayoutConfig();
        for (var index = 0; index < tapPads.Count; index++)
        {
            var tapHop = tapPads[index].RectTransform;
            var tapHopConfig = RectConfig.Create(tapHop);
            layoutConfig.Rects.Add(tapHopConfig);
        }
        var json = Json.Serialize(layoutConfig);
        System.IO.File.WriteAllText($"Assets/Configs/Layouts/{布局名}.json", json);
    }
}
#endif