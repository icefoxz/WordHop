using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LayoutConfigure", menuName = "配置/布局")]
public class LayoutConfigSo : ScriptableObject
{
    [SerializeField] private TextAsset[] 布局_3键;
    [SerializeField] private TextAsset[] 布局_4键;
    [SerializeField] private TextAsset[] 布局_5键;
    [SerializeField] private TextAsset[] 布局_6键;
    [SerializeField] private TextAsset[] 布局_7键;

    private TextAsset[] Tap3Assets => 布局_3键;
    private TextAsset[] Tap4Assets => 布局_4键;
    private TextAsset[] Tap5Assets => 布局_5键;
    private TextAsset[] Tap6Assets => 布局_6键;
    private TextAsset[] Tap7Assets => 布局_7键;

    public TextAsset[] GetLayouts(int keys) => keys switch
    {
        3 => Tap3Assets,
        4 => Tap4Assets,
        5 => Tap5Assets,
        6 => Tap6Assets,
        7 => Tap7Assets,
        _ => throw new ArgumentOutOfRangeException(nameof(keys), keys, null)
    };
    public TextAsset GetRandomLayout(int keys) => GetLayouts(keys).OrderByDescending(_=>UnityEngine.Random.Range(0f,1f)).First();
}