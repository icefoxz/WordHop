using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AOT.Core.Systems.Coroutines;
using AOT.Utl;
using AOT.Utls;
using GamePlay;
using UnityEngine;

//无限关卡执行器
public class InfinityStageModel : ModelBase
{
    public PlayerModel Player => Game.Model.Player;
    public WordLevelModel WordLevel => Game.Model.WordLevel;
    public WordConfigSo WordConfig => Game.ConfigureSo.WordConfig;
    public DifficultyLoader DifficultyLoader { get; }
    public LayoutConfigSo LayoutConfig { get; }
    public Dictionary<int,List<WordGroup>> Data { get; } = new Dictionary<int, List<WordGroup>>();

    private int _timer;
    private int _countdownTime;
    private CoroutineInstance _countdownCoroutine;

    public InfinityStageModel(
        DifficultyLoader difficultyLoader, 
        LayoutConfigSo layoutConfig)
    {
        DifficultyLoader = difficultyLoader;
        LayoutConfig = layoutConfig;
        ResetData();
    }

    private void ResetData()
    {
        for (var i = 3; i <= 7; i++)
        {
            var words = WordConfig.GetWords(i);
            Data.Add(i, words.ToList());
        }
    }

    public void StartGame()
    {
        Player.Reset();
        StartLevel();
    }

    public void StartLevel()
    {
        WordLevel.Reset();
        if (_countdownCoroutine) _countdownCoroutine.StopCo();
        LoadChallengeStage();
        _countdownCoroutine = Game.CoroutineService.RunCo(StartCountdown(), nameof(InfinityStageModel));
    }

    // 挑战模式, 无限关卡, 动态生成, 难度递增
    private void LoadChallengeStage()
    {
        var levelIndex = Player.StageLevelDifficultyIndex;
        var (wds, exSecs, difficulty) = DifficultyLoader.GetChallengeStageLevelConfig(levelIndex + 1);
        var wg = GetWordGroup(wds);

        var secs = exSecs + //最多5秒
                   wg.Key.Length + //最多7字
                   Game.ConfigureSo.GameRoundConfigSo.BaseSeconds;
        var layout = GetLayout(wds.Length);
        _countdownTime = secs;
        WordLevel.InitLevel(wds, wg, difficulty, secs, layout);
    }

    // 获取词语组
    private WordGroup GetWordGroup(TapDifficulty[] wds)
    {
        var wgs = Data[wds.Length].ToArray();
        if (wgs.Length == 0)
        {
            ResetData();
            wgs = Data[wds.Length].ToArray();
        }

        var wg = wgs.OrderByDescending(_ => Random.Range(0, wgs.Length)).First();
        return wg;
    }

    private LayoutConfig GetLayout(int words)
    {
        var layoutAsset = LayoutConfig.GetRandomLayout(words);
        var layoutConfig = Json.Deserialize<LayoutConfig>(layoutAsset.text);
        return layoutConfig;
    }

    IEnumerator StartCountdown()
    {
        _timer = _countdownTime;
        WordLevel.Update(_timer);
        while (_timer > 0)
        {
            yield return new WaitForSeconds(1f);
            Timer_Add(-1);
            // 更新UI显示倒计时，如果有的话
            WordLevel.Update(_timer);
            if (_timer <= 0)
                break;
        }

        Lose(); // 时间到，游戏失败
    }

    private void Timer_Add(int value)
    {
        _timer += value;
        if(_timer < 0) _timer = 0;
    }

    private void Lose()
    {
        // 游戏失败的逻辑
        StopService();
        Game.MessagingManager.SendParams(GameEvents.Stage_Level_Lose);
    }

    public void StopService()
    {
        if (_countdownCoroutine) _countdownCoroutine.StopCo(); // 停止倒计时
    }

    public void HackWin()
    {
        WordLevel.SetAllAlphabetSelected();
        Win();
    }

    private void Win()
    {
        // 游戏胜利的逻辑
        if(_countdownCoroutine) _countdownCoroutine.StopCo(); // 停止倒计时
        Player.StageLevelPass(_timer);
        XDebug.Log("You Win!");
        Game.MessagingManager.SendParams(GameEvents.Stage_Level_Win);
    }

    public void ApplyOrder(Alphabet alphabet)
    {
        if (WordLevel.CheckIfApply(alphabet))
        {
            if (!WordLevel.IsComplete) return;
            // 玩家完成了所有点击，视为过关
            Win();
            return;
        }
        alphabet.Miss();
        if (_timer > 1) Timer_Add(-1);
        // 点击的顺序不对，视为失败
        WordLevel.SelectedAlphabet_Clear();
    }

    public bool IsHinted(Alphabet alphabet) => WordLevel.Hints.Any(a => a.Equals(alphabet));

    public void Quit()
    {
        if (_countdownCoroutine) _countdownCoroutine.StopCo(); // 停止倒计时
        Game.MessagingManager.SendParams(GameEvents.Stage_Quit);
    }
}