using AOT.Core;
using UnityEngine;

public class GamePlayController : MonoBehaviour, IController 
{
    private ConfigureSo Config => Game.ConfigureSo;
    private WordConfigSo WordConfig => Config.WordConfig;
    private LayoutConfigSo LayoutConfig => Config.LayoutConfig;
    private LevelDifficultySo LevelDifficulty => Config.LevelDifficulty;
    private TapPadDifficultySo TapPadDifficulty => Config.TapPadDifficulty;

    private DifficultyLoader ChallengeLoader { get; set; }
    //当前游戏进度
    private InfinityStageHandler StageHandler { get; set; }

    public void Start()
    {
        ChallengeLoader = new DifficultyLoader(LevelDifficulty, TapPadDifficulty);
    }

    public void StartGame()
    {
        StageHandler = new InfinityStageHandler(Game.Model.Player, 
            Game.Model.WordLevel, 
            WordConfig, 
            ChallengeLoader,
            LayoutConfig);
        StageHandler.StartGame();
        Game.MessagingManager.SendParams(GameEvents.Stage_Start);
    }

    public void StartLevel() => StageHandler.StartLevel();

    public void OnAlphabetSelected(Alphabet alphabet, bool isOutline)
    {
        var isHinted = StageHandler.IsHinted(alphabet);
        var stateNum = 5;
        if (isHinted) stateNum--;
        if (isOutline) stateNum--;
        stateNum -= alphabet.MissCount;
        var state = stateNum switch
        {
            >= 5 => Alphabet.States.Excellent,
            > 2 => Alphabet.States.Great,
            _ => Alphabet.States.Fair
        };
        alphabet.UpdateState(state);
        StageHandler.ApplyOrder(alphabet);
    }

    public void OnItemClicked(Alphabet alphabet)
    {
        Game.MessagingManager.SendParams(GameEvents.Level_Item_Clear);
    }
}