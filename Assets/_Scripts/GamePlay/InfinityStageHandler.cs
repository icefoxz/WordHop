using System.Collections;
using System.Linq;
using AOT.Core.Systems.Coroutines;
using AOT.Utl;
using AOT.Utls;
using GamePlay;
using UnityEngine;

//无限关卡执行器
public class InfinityStageHandler 
{
    public PlayerModel Player { get; } 
    public WordLevelModel WordLevel { get; } 
    public WordConfigSo WordConfig { get; }
    public DifficultyLoader DifficultyLoader { get; }
    public LayoutConfigSo LayoutConfig { get; }

    private int _timer;
    private int _countdownTime;
    private CoroutineInstance _countdownCoroutine;

    public InfinityStageHandler(PlayerModel player,
        WordLevelModel wordLevel,
        WordConfigSo wordConfig,
        DifficultyLoader difficultyLoader, 
        LayoutConfigSo layoutConfig)
    {
        Player = player;
        WordLevel = wordLevel;
        WordConfig = wordConfig;
        DifficultyLoader = difficultyLoader;
        LayoutConfig = layoutConfig;
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
        _countdownCoroutine = Game.CoroutineService.RunCo(StartCountdown(), nameof(InfinityStageHandler));
    }

    // 挑战模式, 无限关卡, 动态生成, 难度递增
    private void LoadChallengeStage()
    {
        var levelIndex = Player.StageLevelDifficultyIndex;
        var words = levelIndex > 0 && levelIndex % 10 == 0 ? 7 : 0; // 每10关，第一关为7个字母，其余为随机(0)字母
        var (wds, exSecs) = DifficultyLoader.GetChallengeStageLevelConfig(levelIndex, words);
        var wg = WordConfig.GetRandomWords(wds.Length);
        var secs = exSecs + wg.Key.Length + 10; //暂时秒数这样设定
        var layout = GetLayout(wds.Length);
        _countdownTime = secs;
        WordLevel.InitLevel(wds, wg, secs, layout);
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
            _timer -= 1;
            // 更新UI显示倒计时，如果有的话
            WordLevel.Update(_timer);
            if (_timer <= 0)
                break;
        }

        Lose(); // 时间到，游戏失败
    }
    private void Lose()
    {
        // 游戏失败的逻辑
        if (_countdownCoroutine) _countdownCoroutine.StopCo(); // 停止倒计时
        Debug.Log("Game Over!");
        Game.MessagingManager.SendParams(GameEvents.Stage_Level_Lose);
    }

    public void HackWin()
    {
        WordLevel.SetAllAlphabetSelected();
        Win();
    }

    private void Win()//public for hack
    {
        // 游戏胜利的逻辑
        if(_countdownCoroutine) _countdownCoroutine.StopCo(); // 停止倒计时
        var missAve = WordLevel.SelectedAlphabets.Average(a => a.MissCount);
        var multiplier = WordLevel.SelectedAlphabets.Count - missAve - 1;//最后-1是因为最小字数是3, 而3个字母最多2倍分数
        var score = (int)(_timer * multiplier);
        Player.StageLevelPass(score);
        Pref.SetHighestLevel(Player.HighestLevel);
        Pref.SetPlayerLevel(Player.Level);
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
        // 点击的顺序不对，视为失败
        WordLevel.SelectedAlphabet_Clear();
    }

    public bool IsHinted(Alphabet alphabet) => WordLevel.Hints.Any(a => a.Equals(alphabet));
}