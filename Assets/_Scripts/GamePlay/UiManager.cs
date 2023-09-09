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
    [SerializeField] private View loseView;
    [SerializeField] private View startView;
    [SerializeField] private View wordSlotView;
    [SerializeField] private View underAttackView;
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

    //private WindowButtonUi WinWindow { get; set; }
    //private WindowButtonUi LoseWindow { get; set; }
    //private WindowButtonUi CompleteWindow { get; set; }
    private WindowButtonUi StartWindow { get; set; }

    private View_WordSlotMgr WordSlotMgr { get; set; }
    private View_StageClearMgr StageClearMgr { get; set; }
    private View_GameOverMgr GameOverMgr { get; set; }

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
        var wg = Game.Model.WordLevel.WordGroup;
        var wds = Game.Model.WordLevel.WordDifficulties;
        var layout = Game.Model.WordLevel.Layout;
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
    }

    private void WindowsInit()
    {
        //WinWindow = new WindowButtonUi(winView, () => { GamePlayController.StartLevel(); });
        //Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Win, bag => WinWindow.Show());
        //LoseWindow = new WindowButtonUi(loseView, StartWindow.Show);
        //Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Lose, bag => LoseWindow.Show());
        //CompleteWindow = new WindowButtonUi(completeView, StartWindow.Show);
        //Game.MessagingManager.RegEvent(GameEvents.Stage_Game_Win, bag => CompleteWindow.Set(bag.GetString(0)));
        StartWindow = new WindowButtonUi(startView, () => GamePlayController.StartGame(), true);
        GameOverMgr = new View_GameOverMgr(loseView, StartWindow.Show, () => XDebug.LogWarning("暂时不支持复活功能!"));
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Lose, bag => SetGameOver());
        StageClearMgr = new View_StageClearMgr(winView,()=> GamePlayController.StartLevel());
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
        var score = Game.Model.Stage.GetScore();
        var title = Game.Model.Stage.GetPlayerTitle();
        
        GameOverMgr.Set(title, score);
        GameOverMgr.Show(displayRevive: false);
    }

    private void OnLevelClear()
    {
        StartCoroutine(Call(PlaySlotAnim));
        IEnumerator PlaySlotAnim()
        {
            yield return WordSlotMgr.LightUpAll();
            yield return new WaitForSeconds(1f);
            var stage = Game.Model.Stage;
            var upgradeRec = stage.UpgradeRecord;
            var wordLevel = Game.Model.WordLevel;
            var max = wordLevel.GetCurrentMaxScore();
            var current = stage.UpgradeRecord.UpgradeExp;
            yield return StageClearMgr.PlayUpgrade(CalculateStar(current, max), upgradeRec);
        }
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


