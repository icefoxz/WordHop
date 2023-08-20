using AOT.Views;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AOT.Utl;
using Sirenix.OdinInspector;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [ShowInInspector] private int currentOrder => GamePlay?.Order ?? 0; // 当前要点击的区域的顺序
    [ShowInInspector] private int totalAreas { get; set; } // 总点击区域
    [ShowInInspector] private float countdownTime { get; set; } = 1; // 倒计时时间

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

    private GamePlayField GamePlay { get; set; } 
    private StageConfigSo StageConfig { get; set; }
    private WordConfigSo WordConfig { get; set; }
    private class GamePlayField
    {
        public int LevelIndex { get; private set; } = 0;
        public int Point { get; private set; } = 0;
        public int Order { get; private set; }
        private int DefaultOrder { get; }
        public float Timer;

        public GamePlayField(int startingOrder = 0)
        {
            DefaultOrder = startingOrder;
            Order = startingOrder;
        }

        public void Reset()
        {
            ResetLevel();
            Point = 0;
        }

        public void NextOrder()
        {
            Order++;
        }

        public void NextLevel()
        {
            LevelIndex++;
        }

        public void AddPoint()
        {
            Point += (int)Timer;
        }

        public void ResetLevel()
        {
            LevelIndex = 0;
            Timer = 0;
            ResetOrder();
        }

        public void ResetOrder()
        {
            Order = DefaultOrder;
        }

        public bool IsWin(int order)
        {
            return Order == order;
        }
    }

    public void Init(StageConfigSo stageConfigSo, WordConfigSo wordConfigSo)
    {
        StageConfig = stageConfigSo;
        WordConfig = wordConfigSo; //todo: 把点击顺序, 换成文字顺序
        StartWindow = new WindowButtonUi(startView, StartGame, true);
        WinWindow = new WindowButtonUi(winView, StartGame);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Win, bag => WinWindow.Show());
        LoseWindow = new WindowButtonUi(loseView, StartWindow.Show);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Lose, bag => LoseWindow.Show());
        CompleteWindow = new WindowButtonUi(completeView, StartWindow.Show);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Game_Win, bag => CompleteWindow.Set(bag.GetString(0)));
        ResetGame();
    }

    private void ResetGame()
    {
        if (GamePlay == null)
        {
            var level = GetLevelConfig(0, 0).levelCfg;
            GamePlay = new GamePlayField(level.tapPads.Min(g => g.clickOrder));
        }
        GamePlay.Reset();
    }

    private void StartGame()
    {
        var (padCfg,layoutCfg) = GetLevelConfig(0,GamePlay.LevelIndex);
        LoadLevel(padCfg, layoutCfg);
        StopAllCoroutines();
        Game.MessagingManager.SendParams(GameEvents.Stage_Timer_Update, GamePlay.Timer);
        StartCoroutine(StartCountdown());

        void LoadLevel(LevelConfig level, LayoutConfig layout)
        {
            if (TapPads.Any())
            {
                TapPads.ForEach(t => t.Destroy());
                TapPads.Clear();
            }

            var list = level.tapPads.OrderBy(t => t.clickOrder).ToArray();
            for (var index = 0; index < list.Length; index++)
            {
                var tapPadCfg = list[index];
                // 创建TapHop对象并应用配置
                var prefabView = Instantiate(view_prefab, TapPadParent); // 从某处获取或实例化
                layout.Rects[index].Apply(prefabView.RectTransform);
                var pad = new TapPad(
                    prefabView: prefabView,
                    onTapAction: () => ApplyOrder(tapPadCfg.clickOrder),
                    onOutlineAction: Lose,
                    onItemAction: Lose,
                    config: tapPadCfg);
                TapPads.Add(pad);
            }

            totalAreas = level.tapPads.Count + currentOrder;
            countdownTime = level.countdownTime;
        }
    }

    private (LevelConfig levelCfg,LayoutConfig layoutCfg) GetLevelConfig(int stage,int levelIndex)
    {
        var padJson = StageConfig.GetAllLevels(stage)[levelIndex];
        var padConfig = Json.Deserialize<LevelConfig>(padJson);
        var layoutJson = StageConfig.GetRandomLayout(padConfig.tapPads.Count);
        var layoutConfig = Json.Deserialize<LayoutConfig>(layoutJson);
        return (padConfig, layoutConfig);
    }

    public void ApplyOrder(int order)
    {
        if (GamePlay.IsWin(order))
        {
            GamePlay.NextOrder();
            if (currentOrder >= totalAreas)
            {
                // 玩家完成了所有点击，视为过关
                Win();
            }
        }
        else
        {
            // 点击的顺序不对，视为失败
            Lose();
        }
    }

    public void Lose()
    {
        // 游戏失败的逻辑
        StopAllCoroutines(); // 停止倒计时
        Debug.Log("Game Over!");
        Game.MessagingManager.SendParams(GameEvents.Stage_Level_Lose);
        ResetGame();
    }

    public void Win()
    {
        // 游戏胜利的逻辑
        StopAllCoroutines(); // 停止倒计时
        GamePlay.AddPoint();
        Debug.Log("You Win!");
        GamePlay.NextLevel();
        if (GamePlay.LevelIndex >= StageConfig.GetAllLevels(0).Length)
        {
            // 已经完成了所有关卡，视为通关
            Game.MessagingManager.SendParams(GameEvents.Stage_Game_Win, GamePlay.Point);
            ResetGame();
        }
        else
        {
            Game.MessagingManager.SendParams(GameEvents.Stage_Level_Win);
            GamePlay.ResetOrder();
        }
    }

    IEnumerator StartCountdown()
    {
        GamePlay.Timer = countdownTime;
        while (GamePlay.Timer > 0)
        {
            yield return new WaitForSeconds(1f);
            GamePlay.Timer -= 1;
            // 更新UI显示倒计时，如果有的话
            Game.MessagingManager.SendParams(GameEvents.Stage_Timer_Update, GamePlay.Timer);
            if (GamePlay.Timer <= 0)
                break;
        }
        Lose(); // 时间到，游戏失败
    }
}