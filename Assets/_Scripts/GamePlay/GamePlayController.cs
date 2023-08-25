using System;
using System.Collections;
using System.Linq;
using AOT.Core;
using AOT.Utl;
using UnityEngine;

public class GamePlayController : MonoBehaviour, IController 
{
    private ConfigureSo Config => Game.ConfigureSo;
    private StageModel Stage => Game.Model.Stage;
    private LevelModel Level => Game.Model.Level;
    private StageConfigSo StageConfig => Config.StageConfig;
    private WordConfigSo WordConfig => Config.WordConfig;
    private LayoutConfigSo LayoutConfig => Config.LayoutConfig;
    private LevelDifficultySo LevelDifficulty => Config.LevelDifficulty;
    private TapPadDifficultySo TapPadDifficulty => Config.TapPadDifficulty;

    private LevelLoader ChallengeLoader { get; set; }

    private int _timer;
    private int _countdownTime;

    public void Start()
    {
        ChallengeLoader = new LevelLoader(LevelDifficulty, TapPadDifficulty);
    }

    public void StartGame()
    {
        Stage.Reset();
        StartLevel();
    }

    public void StartLevel()
    {
        Level.Reset();
        StopAllCoroutines();
        //LoadNormalStageLevel();
        LoadChallengeStage();
        StartCoroutine(StartCountdown());
    }

    // 挑战模式, 无限关卡, 动态生成, 难度递增
    private void LoadChallengeStage()
    {
        var levelIndex = Stage.LevelIndex;
        var words = levelIndex > 0 && levelIndex % 10 == 0 ? 7 : 0; // 每10关，第一关为7个字母，其余为随机(0)字母
        var (wds, exSecs) = ChallengeLoader.GetChallengeStageLevelConfig(levelIndex, words);
        var wg = WordConfig.GetRandomWords(wds.Length);
        var secs = exSecs + wg.Key.Length + 10; //暂时秒数这样设定
        var layout = GetLayout(wds.Length);
        Level.InitLevel(wds, wg, secs, layout);
        _countdownTime = secs;
    }

    //void LoadNormalStageLevel()
    //{
    //    var level = GetNormalStageLevelConfig(0, Stage.LevelIndex);
    //    var layout = GetLayout(level.tapPads.Count);
    //    var wg = WordConfig.GetRandomWords(level.tapPads.Count);
    //    var list = level.tapPads.OrderBy(t => t.clickOrder).ToArray();
    //    Level.Reset();
    //    for (var index = 0; index < wg.Words.Length; index++)
    //    {
    //        var tapPadCfg = list[index];
    //        var alphabet = wg.Key[index];
    //        // 创建TapHop对象并应用配置
    //        var prefabView = UiManager.InstancePrefab(); // 从某处获取或实例化
    //        layout.Rects[index].Apply(prefabView.RectTransform);
    //        var pad = new TapPad(
    //            prefabView: prefabView,
    //            onTapAction: () => ApplyOrder(alphabet, null),
    //            onOutlineAction: Lose,
    //            onItemAction: Lose,
    //            tapPadCfg.clickOrder);
    //        tapPadCfg.Apply(prefabView, Game.ResLoader.ButtonSprites);
    //        pad.SetText(alphabet.ToString());
    //        Level.TapPads_Add(pad);
    //    }

    //    Rule = new GamePlayRule(wg.Words);
    //    _countdownTime = level.countdownTime;
    //}

    private LevelConfig GetNormalStageLevelConfig(int stage, int levelIndex)
    {
        var padAsset = StageConfig.GetAllLevels(stage)[levelIndex];
        var padConfig = Json.Deserialize<LevelConfig>(padAsset.text);
        return padConfig;
    }

    private LayoutConfig GetLayout(int words)
    {
        var layoutAsset = LayoutConfig.GetRandomLayout(words);
        var layoutConfig = Json.Deserialize<LayoutConfig>(layoutAsset.text);
        return layoutConfig;
    }

    public void OnAlphabetSelected(Alphabet alphabet, bool isOutline)
    {
        var isHinted = Level.Hints.Any(a => a.Equals(alphabet));
        var stateNum = 3;
        if (isHinted) stateNum--;
        if (isOutline) stateNum--;
        var state = stateNum switch
        {
            1 => Alphabet.States.Fair,
            2 => Alphabet.States.Great,
            3 => Alphabet.States.Excellent,
            _ => throw new ArgumentOutOfRangeException()
        };
        var a = alphabet with { State = state };
        ApplyOrder(a);
    }

    private void ApplyOrder(Alphabet alphabet)
    {
        if (Level.CheckIfApply(alphabet))
        {
            if (!Level.IsComplete) return;
            // 玩家完成了所有点击，视为过关
            Win();
            return;
        }

        // 点击的顺序不对，视为失败
        Level.SelectedAlphabet_Clear();
    }

    private void Lose()
    {
        // 游戏失败的逻辑
        StopAllCoroutines(); // 停止倒计时
        Debug.Log("Game Over!");
        Game.MessagingManager.SendParams(GameEvents.Stage_Level_Lose);
        Stage.Reset();
    }

    private void Win()
    {
        // 游戏胜利的逻辑
        StopAllCoroutines(); // 停止倒计时
        Stage.AddPoint(_timer);
        Stage.NextLevel();
        Debug.Log("You Win!");
        if (Stage.LevelIndex >= StageConfig.GetAllLevels(0).Length)
        {
            // 已经完成了所有关卡，视为通关
            Game.MessagingManager.SendParams(GameEvents.Stage_Game_Win, Stage.Point);
            Stage.Reset();
        }
        else
        {
            Game.MessagingManager.SendParams(GameEvents.Stage_Level_Win);
        }
    }

    IEnumerator StartCountdown()
    {
        _timer = _countdownTime;
        Level.Update(_timer);
        while (_timer > 0)
        {
            yield return new WaitForSeconds(1f);
            _timer -= 1;
            // 更新UI显示倒计时，如果有的话
            Level.Update(_timer);
            if (_timer <= 0)
                break;
        }

        Lose(); // 时间到，游戏失败
    }
}