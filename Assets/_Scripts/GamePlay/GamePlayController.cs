using AOT.Core;
using Sirenix.OdinInspector;
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
    private InfinityStageModel StageModel => Game.Model.InfinityStage;

    public void Start()
    {
        ChallengeLoader = new DifficultyLoader(LevelDifficulty, TapPadDifficulty);
    }

    public void StartGame()
    {
        Game.Model.NewInfinityStage(new InfinityStageModel(ChallengeLoader, LayoutConfig));
        StageModel.StartGame();
    }

    public void StartLevel() => StageModel.StartLevel();

    public void OnAlphabetSelected(Alphabet alphabet, bool isOutline)
    {
        var isHinted = StageModel.IsHinted(alphabet);
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
        StageModel.ApplyOrder(alphabet);
    }

    public void OnItemClicked()
    {
        Game.MessagingManager.SendParams(GameEvents.Level_Item_Clear);
    }

#if UNITY_EDITOR
    [Button(ButtonSizes.Medium),GUIColor("red")]private void Hack_Level_Win() => StageModel.HackWin();
#endif
    public void QuitCurrentGame()
    {
        StageModel.StopService();
        Quit();
    }

    public void Quit() => StageModel.Quit();
}