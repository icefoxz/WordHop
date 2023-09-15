using GamePlay;

public class GameModelContainer
{
    public PlayerModel Player { get; private set; }
    public WordLevelModel WordLevel { get; } = new WordLevelModel();

    public void Init()
    {
        var job = Game.ConfigureSo.JobTree.GetPlayerJob(JobTypes.Villagers, 1);
        var levelFields = Game.ConfigureSo.UpgradeConfigSo.GetLevels();
        Player = new PlayerModel(new PlayerRec(1, 0, 0, 0, job), levelFields);
        var highestLevel = Pref.GetHighestLevel();
        if (highestLevel != null) Player.SetHighestLevel(highestLevel);
    }
}