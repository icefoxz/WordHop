using AOT.Views;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AOT.Utl;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]private View view_prefab;
    [SerializeField]private View winView;
    [SerializeField]private View loseView;
    [SerializeField]private View completeView;
    [SerializeField]private View startView;
    [SerializeField]private Transform _tapPadParent;
    private WindowButtonUi WinWindow { get; set; }
    private WindowButtonUi LoseWindow { get; set; }
    private WindowButtonUi CompleteWindow { get; set; }
    private WindowButtonUi StartWindow { get; set; }

    private Transform TapPadParent => _tapPadParent;
    private List<TapPad> TapPads { get; } = new List<TapPad>();

    private GamePlayRule GamePlay { get; set; } 
    private StageConfigSo StageConfig { get; set; }
    private WordConfigSo WordConfig { get; set; }
    private LayoutConfigSo LayoutConfig { get; set; }
    private LevelDifficultySo LevelDifficulty { get; set; }
    private TapPadDifficultySo TapPadDifficulty { get; set; }

    public LevelLoader ChallengeLoader { get; set; }
    private StageRecorder Recorder { get; set; }
    private List<TapPad> SelectedPads { get; } = new List<TapPad>();


    public void Init(ConfigureSo config)
    {
        StageConfig = config.StageConfig;
        WordConfig = config.WordConfig;
        LayoutConfig = config.LayoutConfig;
        LevelDifficulty = config.LevelDifficulty;
        TapPadDifficulty = config.TapPadDifficulty;
        ChallengeLoader = new LevelLoader(LevelDifficulty, TapPadDifficulty);
        Recorder = new StageRecorder();
        StartWindow = new WindowButtonUi(startView, StartGame, true);
        WinWindow = new WindowButtonUi(winView, StartLevel);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Win, bag => WinWindow.Show());
        LoseWindow = new WindowButtonUi(loseView, StartWindow.Show);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Lose, bag => LoseWindow.Show());
        CompleteWindow = new WindowButtonUi(completeView, StartWindow.Show);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Game_Win, bag => CompleteWindow.Set(bag.GetString(0)));
    }

    private void StartGame()
    {
        Recorder.Reset();
        StartLevel();
    }

    private void StartLevel()
    {
        StopAllCoroutines();
        //LoadNormalStageLevel();
        LoadChallengeStage();
        Game.MessagingManager.SendParams(GameEvents.Stage_Timer_Update, _timer);
        StartCoroutine(StartCountdown());
    }

    private void LoadChallengeStage()
    {
        var levelIndex = Recorder.LevelIndex;
        var words = levelIndex > 0 && levelIndex % 10 == 0 ? 7 : 0; // 每10关，第一关为7个字母，其余为随机(0)字母
        var (wds, exSecs) = ChallengeLoader.GetChallengeStageLevelConfig(levelIndex, words);
        var wg = WordConfig.GetRandomWords(wds.Length);
        var secs = exSecs + wg.Key.Length + 10;//暂时秒数这样设定
        var layout = GetLayout(wds.Length);
        ClearTapPads();
        for (int index = 0; index < wg.Key.Length; index++)
        {
            var alphabet = wg.Key[index];
            var prefabView = Instantiate(view_prefab, TapPadParent);
            var wordDifficulty = wds[index];
            var pad = new TapPad(
                prefabView: prefabView,
                onTapAction: pad => ApplyOrder(alphabet,pad),
                onOutlineAction: _=>Lose(),
                onItemAction: _=>Lose(),
                0);
            pad.Apply(wordDifficulty);
            pad.SetText(alphabet.ToString());
            layout.Rects[index].Apply(prefabView.RectTransform);
            TapPads.Add(pad);
        }
        GamePlay = new GamePlayRule(wg.Words);
        _countdownTime = secs;
    }

    void LoadNormalStageLevel()
    {
        var level = GetNormalStageLevelConfig(0, Recorder.LevelIndex);
        var layout = GetLayout(level.tapPads.Count);
        var wg = WordConfig.GetRandomWords(level.tapPads.Count);
        var list = level.tapPads.OrderBy(t => t.clickOrder).ToArray();
        ClearTapPads();
        for (var index = 0; index < wg.Words.Length; index++)
        {
            var tapPadCfg = list[index];
            var alphabet = wg.Key[index];
            // 创建TapHop对象并应用配置
            var prefabView = Instantiate(view_prefab, TapPadParent); // 从某处获取或实例化
            layout.Rects[index].Apply(prefabView.RectTransform);
            var pad = new TapPad(
                prefabView: prefabView,
                onTapAction: () => ApplyOrder(alphabet,null),
                onOutlineAction: Lose,
                onItemAction: Lose,
                tapPadCfg.clickOrder);
            tapPadCfg.Apply(prefabView, Game.ResLoader.ButtonSprites);
            pad.SetText(alphabet.ToString());
            TapPads.Add(pad);
        }
        GamePlay = new GamePlayRule(wg.Words);
        _countdownTime = level.countdownTime;
    }

    private void ClearTapPads()
    {
        if (TapPads.Any())
        {
            TapPads.ForEach(t => t.Destroy());
            TapPads.Clear();
        }
    }

    private LevelConfig GetNormalStageLevelConfig(int stage,int levelIndex)
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

    public void ApplyOrder(char alphabet, TapPad tapPad = null)
    {
        if (SelectedPads.Contains(tapPad)) return;
        if(GamePlay.CheckIfApply(alphabet))
        {
            if (tapPad != null) 
                SelectedPads.Add(tapPad);
            if (!GamePlay.IsComplete) return;
            // 玩家完成了所有点击，视为过关
            SelectedPads.Clear();
            Win();
            return;
        }
        // 点击的顺序不对，视为失败
        SelectedPads.Clear();
        Lose();
    }

    public void Lose()
    {
        // 游戏失败的逻辑
        StopAllCoroutines(); // 停止倒计时
        Debug.Log("Game Over!");
        Game.MessagingManager.SendParams(GameEvents.Stage_Level_Lose);
        ResetGame();
    }

    private void ResetGame() => Recorder.Reset();

    public void Win()
    {
        // 游戏胜利的逻辑
        StopAllCoroutines(); // 停止倒计时
        Recorder.AddPoint(_timer);
        Recorder.NextLevel();
        Debug.Log("You Win!");
        if (Recorder.LevelIndex >= StageConfig.GetAllLevels(0).Length)
        {
            // 已经完成了所有关卡，视为通关
            Game.MessagingManager.SendParams(GameEvents.Stage_Game_Win, Recorder.Point);
            ResetGame();
        }
        else
        {
            Game.MessagingManager.SendParams(GameEvents.Stage_Level_Win);
        }
    }

    IEnumerator StartCountdown()
    {
        _timer = _countdownTime;
        while (_timer > 0)
        {
            yield return new WaitForSeconds(1f);
            _timer -= 1;
            // 更新UI显示倒计时，如果有的话
            Game.MessagingManager.SendParams(GameEvents.Stage_Timer_Update, _timer);
            if (_timer <= 0)
                break;
        }
        Lose(); // 时间到，游戏失败
    }

    private record StageRecorder
    {
        public int LevelIndex { get; private set; }
        public int Point { get; private set; }

        public void AddPoint(int point)
        {
            Point += point;
        }

        public void NextLevel()
        {
            LevelIndex++;
        }

        public void Reset()
        {
            LevelIndex = 0;
            Point = 0;
        }
    }
    private int _timer;
    private int _countdownTime;
}