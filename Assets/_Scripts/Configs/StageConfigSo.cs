using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ISmallLevel
{
    string Name { get; }
    string Data { get; }
}
[CreateAssetMenu(fileName = "StageConfigure", menuName = "关卡/大关卡")]
public class StageConfigSo : ScriptableObject
{
    [SerializeField]private StageField[] 大关卡;
    private StageField[] Stages => 大关卡;
    [SerializeField]private LayoutField[] 布局;
    private LayoutField[] Layouts => 布局;

    public string[] GetLayouts(int buttonCount) => Layouts.Where(c => c.ButtonCount == buttonCount).SelectMany(c => c.GetLayouts()).ToArray();
    public string GetRandomLayout(int buttonCount)
    {
        var array = GetLayouts(buttonCount);
        if (array.Length == 0)
            throw new NullReferenceException($"找不到[{buttonCount}]的布局!");
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public ISmallLevel GetRandomLevel(int index)
    {
        if (index < 0 || index >= Stages.Length)
            return null;
        var stage = Stages[index];
        if (stage.Data.Length == 0)
            return null;
        return new SmallLevel(stage.Name ,stage.Data[UnityEngine.Random.Range(0, stage.Data.Length)].text);
    }

    public string[] GetAllLevels(int index)
    {
        if (index < 0 || index >= Stages.Length)
            return null;
        return Stages[index].Data.Select(x => x.text).ToArray();
    }

    [Serializable] private class StageField
    {
        [SerializeField] private string 关卡名;
        [SerializeField] private TextAsset[] 小关卡;
        public string Name => 关卡名;
        public TextAsset[] Data=> 小关卡;
    }
    private record SmallLevel(string Name, string Data) : ISmallLevel
    {
        public string Name { get; } = Name;
        public string Data { get; } = Data;
    }

    [Serializable] private class LayoutField
    {
        [SerializeField] private int 按键数;
        [SerializeField] private TextAsset[] 布局;
        private TextAsset[] LayoutAssets => 布局;
        public int ButtonCount => 按键数;
        private IEnumerable<string> Layouts => LayoutAssets.Select(x => x.text);
        public IEnumerable<string> GetLayouts() => Layouts;
        public string GetLayout(int index) => LayoutAssets[index].text;
        public string GetRandomLayout() => LayoutAssets[UnityEngine.Random.Range(0, LayoutAssets.Length)].text;
    }
}