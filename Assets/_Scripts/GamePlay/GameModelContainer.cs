using GamePlay;

public class GameModelContainer
{
    public PlayerModel Player { get; private set; }
    public WordLevelModel WordLevel { get; } = new WordLevelModel();
    public InfinityStageModel InfinityStage { get; private set; }

    public void Init()
    {
        var job = Game.ConfigureSo.JobConfig.GetPlayerJob(JobTypes.Villagers, 1);
        var levelFields = Game.ConfigureSo.UpgradeConfigSo.GetLevels();
        Player = new PlayerModel(new PlayerRec(1, 0, 0, 0, job), levelFields);
        var highestLevel = Pref.GetHighestLevel();
        Player.SetHighestLevel(highestLevel);
    }

    public void NewInfinityStage(InfinityStageModel infinityStage)
    {
        InfinityStage = infinityStage;
        Game.MessagingManager.SendParams(GameEvents.Stage_Start);
    }
}