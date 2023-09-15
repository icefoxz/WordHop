using UnityEngine;

[CreateAssetMenu(fileName = "ConfigureSo", menuName = "配置/主配置")]
public class ConfigureSo : ScriptableObject
{
    [SerializeField] private StageConfigSo 关卡配置;
    [SerializeField] private LayoutConfigSo 布局配置;
    [SerializeField] private WordConfigSo 词语配置;
    [SerializeField] private LevelDifficultySo 关卡难度;
    [SerializeField] private TapPadDifficultySo 按键难度;
    [SerializeField] private JobTreeSo 职业树配置;
    [SerializeField] private BadgeLevelSo 徽章配置;
    [SerializeField] private UpgradeConfigSo 升等配置;
    [SerializeField] private GameRoundConfigSo 游戏奖励配置;

    public StageConfigSo StageConfig => 关卡配置;
    public LayoutConfigSo LayoutConfig => 布局配置;
    public WordConfigSo WordConfig => 词语配置;
    public LevelDifficultySo LevelDifficulty => 关卡难度;
    public TapPadDifficultySo TapPadDifficulty => 按键难度;
    public JobTreeSo JobTree => 职业树配置;
    public BadgeLevelSo BadgeLevelSo => 徽章配置;
    public UpgradeConfigSo UpgradeConfigSo => 升等配置;
    public GameRoundConfigSo GameRoundConfigSo => 游戏奖励配置;
}