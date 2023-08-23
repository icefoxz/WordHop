using UnityEngine;

[CreateAssetMenu(fileName = "ConfigureSo", menuName = "配置/主配置")]
public class ConfigureSo : ScriptableObject
{
    [SerializeField] private StageConfigSo 关卡配置;
    [SerializeField] private LayoutConfigSo 布局配置;
    [SerializeField] private WordConfigSo 词语配置;
    [SerializeField] private LevelDifficultySo 关卡难度;
    [SerializeField] private TapPadDifficultySo 按键难度;
    public StageConfigSo StageConfig => 关卡配置;
    public LayoutConfigSo LayoutConfig => 布局配置;
    public WordConfigSo WordConfig => 词语配置;
    public LevelDifficultySo LevelDifficulty => 关卡难度;
    public TapPadDifficultySo TapPadDifficulty => 按键难度;
}