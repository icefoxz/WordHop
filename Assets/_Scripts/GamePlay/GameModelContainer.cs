using GamePlay;
using Unity.Jobs;

public class GameModelContainer
{
    public PlayerModel Player { get; private set; }
    public WordLevelModel WordLevel { get; } = new WordLevelModel();
    public InfinityStageModel InfinityStage { get; private set; }

    public void NewInfinityStage(InfinityStageModel infinityStage)
    {
        InfinityStage = infinityStage;
        Game.MessagingManager.SendParams(GameEvents.Stage_Start);
    }

    public void InitPlayer(JobTypes jobType)
    {
        var job = Game.ConfigureSo.JobConfig.GetPlayerJob(jobType, 1, 1);
        var levelFields = Game.ConfigureSo.UpgradeConfigSo.GetLevels();
        var player = new PlayerModel(new PlayerRec(1, 0, 0, 0, job), levelFields, 6);
        Player = player;
        Game.PlayerSave.LoadHighestRecord();
    }

    public PlayerModel TryGetPlayer()
    {
        if(Player==null)InitPlayer(JobTypes.Villagers);
        return Player;
    }
}