using System;
using System.Linq;
using AOT.Views;
using UnityEngine;

[CreateAssetMenu(fileName = "LayoutConfigure", menuName = "配置/布局")]
public class LayoutConfigSo : ScriptableObject
{
    [SerializeField] private View villagerPad;
    [SerializeField] private View warriorPad;
    [SerializeField] private View magePad;
    [SerializeField] private View elfPad;
    [SerializeField] private View necromancerPad;
    [SerializeField] private View mysteriousPad;

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

    public View GetPad(JobTypes job) => job switch
    {
        JobTypes.Villagers => villagerPad,
        JobTypes.Warriors => warriorPad,
        JobTypes.Mages => magePad,
        JobTypes.Elves => elfPad,
        JobTypes.Necromancers => necromancerPad,
        JobTypes.Mysterious => mysteriousPad,
        _ => throw new ArgumentOutOfRangeException(nameof(job), job, null)
    };

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