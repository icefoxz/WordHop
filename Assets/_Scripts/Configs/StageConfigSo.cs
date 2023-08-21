using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISmallLevel
{
    string Name { get; }
    string Data { get; }
}

[CreateAssetMenu(fileName = "StageConfigure", menuName = "配置/关卡")]
public class StageConfigSo : ScriptableObject
{
    [SerializeField]private StageField[] 大关卡;
    private StageField[] Stages => 大关卡;

    public ISmallLevel GetRandomLevel(int index)
    {
        if (index < 0 || index >= Stages.Length)
            return null;
        var stage = Stages[index];
        if (stage.Data.Length == 0)
            return null;
        return new SmallLevel(stage.Name ,stage.Data[UnityEngine.Random.Range(0, stage.Data.Length)].text);
    }

    public TextAsset[] GetAllLevels(int index)
    {
        if (index < 0 || index >= Stages.Length)
            return null;
        return Stages[index].Data;
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

}
