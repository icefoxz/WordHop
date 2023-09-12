using GamePlay;
using Unity.Jobs.LowLevel.Unsafe;

public class GameModelContainer
{
    public PlayerModel Player { get; private set; }
    public WordLevelModel WordLevel { get; } = new WordLevelModel();

    public void Init()
    {
        var job = Game.ConfigureSo.JobTree.GetPlayerJob(JobTypes.Warriors, 1);
        var upgradeFields = Game.ConfigureSo.UpgradeConfigSo.GetLevels();
        Player = new PlayerModel(new PlayerLevel(1, 0, 0, 0, job), upgradeFields);
        var highestLevel = Pref.GetHighestLevel();
        if (highestLevel != null) Player.SetHighestLevel(highestLevel);
    }
}