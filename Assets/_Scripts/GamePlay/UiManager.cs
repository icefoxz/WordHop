using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AOT.Views;
using AOT.BaseUis;
using AOT.Utls;
using DG.Tweening;
using GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UiManager : MonoBehaviour
{
    //[SerializeField] private View view_prefab;
    [SerializeField] private View winView;
    [SerializeField] private View gameOverView;
    [SerializeField] private View homeView;
    [SerializeField] private View wordSlotView;
    [SerializeField] private View topSectionView;
    [SerializeField] private View settingsView;
    [SerializeField] private View underAttackView;
    [SerializeField] private View achievementView;
    [SerializeField] private View windowConfirmView;
    [SerializeField] private View selectJobView;
    [SerializeField] private View popMessageView;

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
    public View_AchievementMgr AchievementMgr { get; set; }
    private View_Home view_home { get; set; }
    private View_windowConfirm view_windowConfirm { get; set; }
    private View_SelectJobMgr view_selectJob { get; set; }
    private View_popMessage view_popMessage { get; set; }

    private Transform TapPadParent => _tapPadParent;
    private GamePlayController GamePlayController => Game.Controller.Get<GamePlayController>();

    private PrefabsViewUi<TapPad> TapPadList { get; set; }

    //标记玩家是否切换了职业, 每局会重置
    private bool _jobSwitchFlag;

    public void Init()
    {
        _blockingPanel.gameObject.SetActive(false);
        WindowsInit();
        RegGamePlayEvent();
        WordSlotMgr = new View_WordSlotMgr(wordSlotView);
    }

    private void ResetTapPad()
    {
        var job = Game.Model.Player.Current.Job.JobType;
        TapPadList?.ClearList(t => t.Destroy());
        var view = Game.ConfigureSo.LayoutConfig.GetPad(job);
        TapPadList = new PrefabsViewUi<TapPad>(view, TapPadParent);
    }

    private void RegGamePlayEvent()
    {
        Game.MessagingManager.RegEvent(GameEvents.Level_Init, bag => LoadLevel());
        Game.MessagingManager.RegEvent(GameEvents.Level_Word_Clear, _ => ResetTapPads());
        Game.MessagingManager.RegEvent(GameEvents.Level_Hints_add, TryAddObstacle);
        Game.MessagingManager.RegEvent(GameEvents.Player_Job_Switch, _ => _jobSwitchFlag = true);
    }

    private void TryAddObstacle(ObjectBag b)
    {
        var ran = Random.Range(0, 1f);
        var word = Game.Model.WordLevel;
        if (ran > word.Difficulty) return;
        var letter = b.Get<string>(0);
        var pad = TapPadList.List
            .Where(t => !t.HasItem)
            .OrderBy(_ => Random.Range(0, 1f))
            .FirstOrDefault(p => p.Alphabet.Text.Equals(letter));
        pad?.ShowItem();
    }

    private void LoadLevel()
    {
        var wordLevel = Game.Model.WordLevel;
        ResetTapPad();
        var wg = wordLevel.WordGroup;
        var wds = wordLevel.WordDifficulties;
        var layout = wordLevel.Layout;
        var letters = wg.Key.OrderBy(_ => Random.Range(0, 1f)).ToArray(); // 随机排序
        TapPadList.ClearList(p => p.Destroy());
        WordSlotMgr.SetDisplay(letters.Length);
        var layoutPrefabs = new List<RectTransform>();
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
                    onItemAction: pad => GamePlayController.OnItemClicked(),
                    id, alphabet);
                p.Apply(wordDifficulty);
                return p;
            });
            layout.Rects[i].Apply(pad.RectTransform);
            layoutPrefabs.Add(pad.RectTransform);
        }
        AdjustToCenter((RectTransform)TapPadParent, layoutPrefabs);

        //LoadBadge(StageClearMgr.GetLevelBarBadge());
    }

    private void AdjustToCenter(RectTransform layoutParent, List<RectTransform> prefabs)
    {
        if (layoutParent == null || prefabs.Count == 0)
            return;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        // 计算所有prefabs的最大边界
        foreach (var prefab in prefabs)
        {
            var left = prefab.anchoredPosition.x - prefab.rect.width / 2;
            var right = prefab.anchoredPosition.x + prefab.rect.width / 2;
            var top = prefab.anchoredPosition.y + prefab.rect.height / 2;
            var bottom = prefab.anchoredPosition.y - prefab.rect.height / 2;

            minX = Mathf.Min(minX, left);
            maxX = Mathf.Max(maxX, right);
            minY = Mathf.Min(minY, bottom);
            maxY = Mathf.Max(maxY, top);
        }

        // 计算“布局”矩形的中心
        var layoutCenterX = (minX + maxX) / 2;
        var layoutCenterY = (minY + maxY) / 2;

        // 计算移动到`TapPadParent`中心需要的偏移量
        var parentCenterX = layoutParent.rect.width / 2f; // 锚点为0.5
        var parentCenterY = layoutParent.rect.height / 2f; // 锚点为0.5
        var shiftAmountX = parentCenterX - layoutCenterX;
        var shiftAmountY = parentCenterY - layoutCenterY;

        // 将每个prefab移到正确的位置
        foreach (var prefab in prefabs)
        {
            var newPosition = prefab.anchoredPosition;
            newPosition.x += shiftAmountX;
            //newPosition.y += shiftAmountY;
            prefab.anchoredPosition = newPosition;
        }
    }

    private void WindowsInit()
    {
        AchievementMgr = new View_AchievementMgr(achievementView);
        view_windowConfirm = new View_windowConfirm(windowConfirmView);
        view_home = new View_Home(homeView, OnTapToStart, AchievementMgr.Show);
        view_popMessage = new View_popMessage(popMessageView);
        SettingsMgr = new View_SettingsMgr(settingsView);
        SettingsMgr.Init();
        TopSection = new View_TopSection(topSectionView, SettingsMgr.Show, OnHomeAction);
        TopSection.Init();
        view_selectJob = new View_SelectJobMgr(selectJobView, OnRoleSelected);
        //StartWindow = new WindowButtonUi(startView, () => GamePlayController.StartGame(), true);
        GameOverMgr = new View_GameOverMgr(gameOverView, GamePlayController.QuitToHome, () => XDebug.LogWarning("暂时不支持复活功能!"));
        Game.MessagingManager.RegEvent(GameEvents.Game_Home, _ => view_home.Show());
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Lose, bag => SetGameOver());
        StageClearMgr = new View_StageClearMgr(winView, ShowOption);
        StageClearMgr.SetCardAction(ShowOption, () => PlayDisplayOption(false));
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Win, b => OnStageClear());
        underAttackView.GameObject.SetActive(false);
        Game.MessagingManager.RegEvent(GameEvents.Level_Alphabet_Failed, b => PlayUnderAttack());
    }


    private void AdAction()
    {
#if UNITY_EDITOR
        AddCoin();
#else
        Game.AdAgent.ShowRewardedVideo((success, message) =>
        {
            if (success)
            {
                AddCoin();
                return;
            }
            PopMessage($"Ad error: {message}");
        }, null);
#endif
        void AddCoin()
        {
            var player = Game.Model.Player;
            StageClearMgr.ActiveAdButton(false);
            var lastAdded = player.LastAddedCoin;
            Game.Model.Player.AddAdCoin();
            RefreshOptions();
            StageClearMgr.SetCoin(player.LastAddedCoin + lastAdded, player.Coin);
            var fromCoin = player.Coin - player.LastAddedCoin;
            StageClearMgr.PlayToCoin(fromCoin, player.Coin);
        }
    }

    public void PopMessage(string message)
    {
        StartCoroutine(view_popMessage.SetMessage(message));
    }

    private void OnRoleSelected(JobTypes job)
    {
        var jobConfig = Game.ConfigureSo.JobConfig;
        var brief = jobConfig.GetJobBrief(job);
        
        view_windowConfirm.Set(job.ToText(), brief, StartGame, pauseGame: false);

        void StartGame()
        {
            view_selectJob.Hide();
            GamePlayController.StartGame(job);
            view_home.Hide();
        }
    }

    private void OnTapToStart() => view_selectJob.Show();

    private void OnHomeAction()
    {
        view_windowConfirm.Set("Exit to start menu", "Current progress will not be saved.", ExitToMenu,
            pauseGame: true);
    }

    private void ExitToMenu()
    {
        GamePlayController.QuitCurrentGame();
        view_home.Show();
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
        var cardArg = GetCardArg();
        LoadBadge(GameOverMgr.Badge);
        GameOverMgr.Set(job.Title, player.QualityLevel, playerLevel.Score);
        GameOverMgr.SetCard(cardArg);
        GameOverMgr.Show(displayRevive: false);
    }

    private void OnStageClear()
    {
        StartCoroutine(Call(PlaySlotAnim));

        IEnumerator PlaySlotAnim()
        {
            StageClearMgr.ResetUi();
            yield return WordSlotMgr.LightUpAll();
            yield return new WaitForSeconds(1f);
            var player = Game.Model.Player;
            var upgradeRec = player.UpgradeRecord;
            var lastLevel = upgradeRec.Levels[0];
            var arg = GetCardArg();
            StageClearMgr.ClearCard();
            StageClearMgr.SetCard(arg, true);
            StageClearMgr.DisplayCardSect(true);
            StageClearMgr.SetLevel(lastLevel.Level);
            LoadBadge(StageClearMgr.GetCardBadge());//preset
            //LoadBadge(StageClearMgr.GetLevelBarBadge());//preset
            var isAdAvailable = Game.AdAgent.IsRewardedVideoAvailable();
#if UNITY_EDITOR
            isAdAvailable = true;
#endif
            StageClearMgr.SetAdButton(isAdAvailable, AdAction);
            if (player.IsMaxLevel())//如果最大等级
            {
                StageClearMgr.SetComplete(OnGameEnd);
            }
            if (!_jobSwitchFlag)
                yield return StageClearMgr.FadeOutCard(0);
            var lastAdded = player.AdCoin;
            StageClearMgr.SetCoin(lastAdded, player.Coin);
            var fromCoin = player.Coin - lastAdded;
            StageClearMgr.PlayToCoin(fromCoin, player.Coin);
            yield return StageClearMgr.PlayExpGrowing(player.Stars, upgradeRec);
            if (_jobSwitchFlag)
            {
                //yield return StageClearMgr.PlayWindowToY(300, 0.5f);
                yield return StageClearMgr.FadeOutCard(1.5f);
            }

            yield return new WaitForSeconds(0.5f);
            RefreshOptions();
            
            _jobSwitchFlag = false;
        }
    }

    private void RefreshOptions()
    {
        var player = Game.Model.Player;
        var job = player.Current.Job;
        var qualityCfg = Game.ConfigureSo.QualityConfigSo;
        var options = Game.ConfigureSo.JobConfig.GetJobSwitches(job.JobType, player.Current.Level, player.QualityLevel)
            .ToArray();
        var qualityOps = qualityCfg.GetQualityOptions(player.Current.Job.JobType);
        StageClearMgr.SetOptions(
            qualityOps.Select(o => (o.icon, o.brief, o.cost, o.quality, player.Coin >= o.cost)).ToArray(),
            options.Select(ConvertJobArg).ToArray(),
            SelectQualityStartLevel, index => SwitchJob(options[index]));
    }

    private void ShowOption() => PlayDisplayOption(true);
    private void PlayDisplayOption(bool display)
    {
        var yPos = display ? 900 : 0;
        StartCoroutine(StageClearMgr.PlayOptions(yPos, 1, display));
    }

    private void SelectQualityStartLevel(int quality)
    {
        var player = Game.Model.Player;
        var prevQualityLevel = player.QualityLevel;
        GamePlayController.QualityChange(quality);
        var nextQualityLevel = player.QualityLevel;
        StageClearMgr.FadeQualityOptions(0, 1);
        StartCoroutine(PlayBadge());

        IEnumerator PlayBadge()
        {
            var secs = 2f;
            var fromCoin = player.Coin - player.LastAddedCoin;
            StageClearMgr.PlayToCoin(fromCoin, player.Coin);
            if (prevQualityLevel != nextQualityLevel)
            {
                secs -= 0.5f;
                var badge = StageClearMgr.GetCardBadge();
                yield return new WaitForSeconds(0.5f);
                yield return badge.PlayFadeIn();
                LoadBadge(badge);
                yield return badge.PlayFadeOut();
            }

            if (_jobSwitchFlag)
            {
                secs -= 1.5f;
                var arg = GetCardArg();
                StageClearMgr.SetCard(arg, false);
                yield return StageClearMgr.FadeOutCard(1.5f);
            }

            yield return new WaitForSeconds(secs);
            StartLevel();
        }
    }
    private static void LoadBadge(View_Badge badge)
    {
        var player = Game.Model.Player;
        var cfg = player.GetBadgeCfg();
        badge.Set(player.Current.Job.Title, player.QualityLevel);
        BadgeConfigLoader.LoadPrefab(cfg, badge.GameObject);
    }

    private void OnGameEnd()
    {
        view_windowConfirm.Set("Exit to start menu", "Save current record and exit to start menu.", EndGameToMenu,
            pauseGame: true);

        void EndGameToMenu()
        {
            StageClearMgr.Hide();
            Game.PlayerSave.SaveAll();
            ExitToMenu();
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
        return Game.ConfigureSo.JobConfig.GetCardArg(player.Current.Job);
    }

    private static (string title, string Message, Sprite Icon, int cost, bool enable) ConvertJobArg(JobSwitch o)
    {
        var player = Game.Model.Player;
        var jobInfo = Game.ConfigureSo.JobConfig.GetCardArg(o.JobType, o.Level, o.Quality);
        var jobIcon = Game.ConfigureSo.JobConfig.GetJobIcon(o.JobType);

        return (jobInfo.title, o.Message, jobIcon, o.Cost, player.Coin >= o.Cost);
    }

    private void StartLevel()
    {
        _jobSwitchFlag = false;
        GamePlayController.StartLevel();
        StageClearMgr.Hide();
    }

    private void ResetTapPads()
    {
        foreach (var pad in TapPadList.List) pad.ResetColor();
    }
}

public class View_popMessage: UiBase
{
    private TMP_Text tmp_message { get; }
    public View_popMessage(IView v) : base(v, false)
    {
        tmp_message = v.Get<TMP_Text>("tmp_message");
    }

    public IEnumerator SetMessage(string message)
    {
        tmp_message.text = message;
        tmp_message.DOFade(1, 0);
        Show();
        yield return tmp_message.DOFade(0, 1).WaitForCompletion();
        Hide();
    }
}