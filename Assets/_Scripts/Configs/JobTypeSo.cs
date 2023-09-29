using System;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "JobType", menuName = "配置/玩家职业/职业")]
public class JobTypeSo : ScriptableObject
{
    [SerializeField] private JobField[] 职业等级;
    [SerializeField] private float 金币奖励倍率 = 1f;
    [SerializeField] private float 经验奖励倍率 = 1f;
    [SerializeField] private TextAsset csvFile;
    public JobField[] LevelSets => 职业等级;
#if UNITY_EDITOR
    [Button(ButtonSizes.Medium),GUIColor("Yellow")] private void LoadCsvToFields()
    {
        var lines = csvFile.text.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        职业等级 = new JobField[lines.Length];
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var fields = line.Split(',');
            职业等级[i] = JobField.InstanceWithoutSprite(
                int.Parse(fields[0]),
                int.Parse(fields[1]),
                int.Parse(fields[2]),
                fields[3]
            );
        }
    }

    [Button(ButtonSizes.Medium), GUIColor("red")]
    private void SaveFieldsToCsv()
    {
        var lines = new string[职业等级.Length + 1];
        lines[0] = "id,等级,品质,称号";
        for (var i = 0; i < 职业等级.Length; i++)
        {
            var f = 职业等级[i];
            lines[i + 1] = $"{f.Id},{f.Level},{f.MinQuality},{f.Title}";
        }
        var csvAssetPath = AssetDatabase.GetAssetPath(csvFile);
        System.IO.File.WriteAllLines(csvAssetPath, lines);
    }
#endif

    public float CoinRewardRatio => 金币奖励倍率;
    public float ExpRewardRatio => 经验奖励倍率;
}
[Serializable]
public class JobField
{
#if UNITY_EDITOR
    public static JobField InstanceWithoutSprite(int id, int 等级, int 品质, string 称号)
    {
        var f = new JobField
        {
            _id = id,
            等级 = 等级,
            品质 = 品质,
            称号 = 称号
        };
        return f;
    }
#endif
    [Min(1)][SerializeField] private int _id;
    [Min(1)][SerializeField] private int 等级;
    [Min(1)][SerializeField] private int 品质 = 1;
    [SerializeField] private string 称号;
    [SerializeField] private Sprite 图标;
    public int Id => _id;
    public int Level => 等级;
    public int MinQuality => 品质;
    public string Title => 称号;
    public Sprite Icon => 图标;

}