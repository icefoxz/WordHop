using GamePlay;

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

    public void InitPlayer(PlayerModel player) => Player = player;
}