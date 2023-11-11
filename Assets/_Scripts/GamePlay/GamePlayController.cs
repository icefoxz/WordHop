using System;
using System.Linq;
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
    private InfinityStageModel StageModel => Game.Model.InfinityStage;

    public void Start()
    {
        ChallengeLoader = new DifficultyLoader(LevelDifficulty, TapPadDifficulty);
    }

    public void StartGame(JobTypes type)
    {
        Game.PlayerSave.StartGame(type);
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

    public void QuitCurrentGame()
    {
        StageModel.StopService();
        QuitToHome();
    }

    public void QuitToHome()
    {
        StageModel.Quit();
        Game.MessagingManager.SendParams(GameEvents.Game_Home);
    }


    public void QualityChange(int quality)
    {
        var player = Game.Model.Player;
        var arg = Config.QualityConfigSo
            .GetQualityOptions(player.Current.Job.JobType)
            .First(q => q.quality == quality);
        player.AddCoin(-arg.cost);
        player.AddQuality(ConvertQuality(quality));

        int ConvertQuality(int q) => q switch
        {
            2 => 1,
            0 => -1,
            _ => 0
        };
    }

}