using System;
using System.Collections;
using System.Linq;
using AOT.Views;
using AOT.BaseUis;
using AOT.Utls;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UiManager : MonoBehaviour
{
    [SerializeField] private View view_prefab;
    [SerializeField] private View winView;
    [SerializeField] private View gameOverView;
    [SerializeField] private View homeView;
    [SerializeField] private View wordSlotView;
    [SerializeField] private View topSectionView;
    [SerializeField] private View settingsView;
    [SerializeField] private View underAttackView;
    [SerializeField] private View achievementView;

    [SerializeField] private Transform _tapPadParent;
    [SerializeField] private Image _blockingPanel;
    private bool _isBusy;//用于Call这个内部方法, 为了避免有多次阻塞的操作
    // Call方法用于阻塞用户操作，直到action执行完毕, 期间用户无法操作并且有_blockingPanel会挡住玩家的Ui操作
    private IEnumerator Call(Func<IEnumerator> action)
    {
        if (_isBusy) throw new Exception("UiManager is busy");
        _isBusy = true;
        _blockingPanel.gameObject.SetActive(true);
        yield return action();
        yield return new WaitForSeconds(0.5f);
        _blockingPanel.gameObject.SetActive(false);
        _isBusy = false;
    }

    private View_WordSlotMgr WordSlotMgr { get; set; }
    private View_StageClearMgr StageClearMgr { get; set; }
    private View_GameOverMgr GameOverMgr { get; set; }
    private View_SettingsMgr SettingsMgr { get; set; }
    private View_TopSection TopSection { get; set; }
    private View_AchievementMgr AchievementMgr { get; set; }
    private View_Home view_home { get; set; }

    private Transform TapPadParent => _tapPadParent;
    private GamePlayController GamePlayController => Game.Controller.Get<GamePlayController>();

    private PrefabsViewUi<TapPad> TapPadList { get; set; }

    public void Init()
    {
        _blockingPanel.gameObject.SetActive(false);
        WindowsInit();
        RegGamePlayEvent();
        TapPadList = new PrefabsViewUi<TapPad>(view_prefab, TapPadParent);
        WordSlotMgr = new View_WordSlotMgr(wordSlotView);
    }

    private void RegGamePlayEvent()
    {
        Game.MessagingManager.RegEvent(GameEvents.Level_Init, bag => LoadLevel());
        Game.MessagingManager.RegEvent(GameEvents.Level_Word_Clear, _ => ResetTapPads());
    }

    private void LoadLevel()
    {
        var wordLevel = Game.Model.WordLevel;
        var stage = Game.Model.Player;
        var wg = wordLevel.WordGroup;
        var wds = wordLevel.WordDifficulties;
        var layout = wordLevel.Layout;
        var letters = wg.Key.OrderBy(_ => Random.Range(0, 1f)).ToArray(); // 随机排序
        TapPadList.ClearList(p => p.Destroy());
        WordSlotMgr.SetDisplay(letters.Length);
        for (var i = 0; i < letters.Length; i++)
        {
            var alphabet = letters[i];
            var wordDifficulty = wds[i];
            var id = i;
            var pad = TapPadList.Instance(view =>
            {
                var p = new TapPad(
                    prefabView: view,
                    onTapAction: pad => GamePlayController.OnAlphabetSelected(pad.Alphabet, false),
                    onOutlineAction: pad => GamePlayController.OnAlphabetSelected(pad.Alphabet, true),
                    onItemAction: pad => GamePlayController.OnItemClicked(pad.Alphabet),
                    id, alphabet);
                p.Apply(wordDifficulty);
                return p;
            });
            layout.Rects[i].Apply(pad.RectTransform);
        }

        var badgeCfg = GetBadgeCfgForCurrentLevel();
        StageClearMgr.SetBadge(badgeCfg);
    }

    private static BadgeConfiguration GetBadgeCfgForCurrentLevel()
    {
        var playerLevel = Game.Model.Player.GetPlayerLevel();
        return Game.ConfigureSo.BadgeLevelSo.GetBadgeConfig(playerLevel);
    }

    private void WindowsInit()
    {
        AchievementMgr = new View_AchievementMgr(achievementView);
        view_home = new View_Home(homeView, () =>
        {
            GamePlayController.StartGame();
            view_home.Hide();
        }, AchievementMgr.Show);
        SettingsMgr = new View_SettingsMgr(settingsView);
        SettingsMgr.LoadPref();
        TopSection = new View_TopSection(topSectionView, SettingsMgr.Show);
        //StartWindow = new WindowButtonUi(startView, () => GamePlayController.StartGame(), true);
        GameOverMgr = new View_GameOverMgr(gameOverView, view_home.Show, () => XDebug.LogWarning("暂时不支持复活功能!"));
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Lose, bag => SetGameOver());
        StageClearMgr = new View_StageClearMgr(winView, StartLevel);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Win, b => OnLevelClear());
        underAttackView.GameObject.SetActive(false);
        Game.MessagingManager.RegEvent(GameEvents.Level_Alphabet_Failed, b => PlayUnderAttack());
    }

    private void PlayUnderAttack()
    {
        StartCoroutine(OnUnderAttack());

        IEnumerator OnUnderAttack()
        {
            underAttackView.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            underAttackView.gameObject.SetActive(false);
        }
    }

    private void SetGameOver()
    {
        var player = Game.Model.Player;
        var playerLevel = player.Current;
        var job = playerLevel.Job;
        var badgeCfg = GetBadgeCfgForCurrentLevel();
        var cardArg = GetCardArg();
        BadgeConfigLoader.LoadPrefab(badgeCfg, GameOverMgr.Badge);
        GameOverMgr.Set(job.Title, job.Level, playerLevel.Score);
        GameOverMgr.SetCard(cardArg);
        GameOverMgr.Show(displayRevive: false);
    }

    private void OnLevelClear()
    {
        StartCoroutine(Call(PlaySlotAnim));
        IEnumerator PlaySlotAnim()
        {
            StageClearMgr.DisplayCardSect(false);
            yield return WordSlotMgr.LightUpAll();
            yield return new WaitForSeconds(1f);
            var player = Game.Model.Player;
            var upgradeRec = player.UpgradeRecord;
            var wordLevel = Game.Model.WordLevel;
            var max = wordLevel.GetCurrentMaxScore();
            var current = player.UpgradeRecord.UpgradeExp;
            var levelInfo = player.GetPlayerLevelInfo();
            var badgeCfg = GetBadgeCfgForCurrentLevel();
            var isUpgrade = upgradeRec.Levels.Count > 1;
            StageClearMgr.SetCoin(player.LastCoinAdd, player.Current.Coin);
            yield return StageClearMgr.PlayExpGrowing(levelInfo?.title, CalculateStar(current, max), upgradeRec,
                prefab => BadgeConfigLoader.LoadPrefab(badgeCfg, prefab));
            if (isUpgrade)
            {
                StageClearMgr.DisplayCardSect(true);
                var arg = GetCardArg();
                StageClearMgr.ClearCard();
                StageClearMgr.SetCard(arg, true);
                var options = arg.options.Where(o => player.Current.Coin >= o.Cost)
                            .Select(ConvertJobArg).ToArray();
                if (options.Length > 0)
                {
                    StageClearMgr.SetOptions(options,
                        StartLevel, index => SwitchJob(arg.options[index]));
                    StageClearMgr.SetCardAction(() => StartCoroutine(StageClearMgr.ShowOptions(1000, 1)));
                }
                yield return StageClearMgr.PlayWindowToY(300, 0.5f);
                yield return StageClearMgr.FadeOutCard(1.5f);
            }
        }
    }

    private void SwitchJob(JobSwitch job)
    {
        var player = Game.Model.Player;
        player.AddCoin(-job.Cost);
        player.SwitchJob(job);
        StageClearMgr.ClearCard();
        var arg = GetCardArg();
        StageClearMgr.SetCard(arg, false);
        StageClearMgr.HideOptions();
        StartCoroutine(PlayCardFadeout());

        IEnumerator PlayCardFadeout()
        {
            yield return StageClearMgr.FadeOutCard(1);
            yield return new WaitForSeconds(1f);
            StartLevel();
        }
    }

    private static CardArg GetCardArg()
    {
        var player = Game.Model.Player;
        return Game.ConfigureSo.JobTree.GetCardArg(player.Current.Job);
    }

    private static (string title, string Message, Sprite Icon) ConvertJobArg(JobSwitch o)
    {
        var jobInfo = Game.ConfigureSo.JobTree.GetJobInfo(o.JobType, o.Level).Value;
        return (jobInfo.title, o.Message, o.Icon);
    }

    private void StartLevel()
    {
        GamePlayController.StartLevel();
        StageClearMgr.Hide();
    }

    public int CalculateStar(float currentScore, float maxScore)
    {
        var percentageScore = currentScore / maxScore * 100;
        return percentageScore switch
        {
            >= 80 => 3,
            >= 50 => 2,
            _ => 1
        };
    }

    private void ResetTapPads()
    {
        foreach (var pad in TapPadList.List) pad.ResetColor();
    }
}


