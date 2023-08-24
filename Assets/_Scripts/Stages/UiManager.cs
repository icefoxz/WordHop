using AOT.Views;
using AOT.BaseUis;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private View view_prefab;
    [SerializeField] private View winView;
    [SerializeField] private View loseView;
    [SerializeField] private View completeView;
    [SerializeField] private View startView;
    [SerializeField] private View wordSlotView;
    [SerializeField] private Transform _tapPadParent;
    
    private WindowButtonUi WinWindow { get; set; }
    private WindowButtonUi LoseWindow { get; set; }
    private WindowButtonUi CompleteWindow { get; set; }
    private WindowButtonUi StartWindow { get; set; }

    private View_WordSlotMgr WordSlotMgr { get; set; }

    private Transform TapPadParent => _tapPadParent;
    private GamePlayController GamePlayController => Game.Controller.Get<GamePlayController>();

    private PrefabsViewUi<TapPad> TapPadList { get; set; }

    public void Init()
    {
        WindowsInit();
        RegGamePlayEvent();
        TapPadList = new PrefabsViewUi<TapPad>(view_prefab, TapPadParent);
        WordSlotMgr = new View_WordSlotMgr(wordSlotView);
    }

    private void RegGamePlayEvent()
    {
        Game.MessagingManager.RegEvent(GameEvents.Level_Init, bag => InstanceUis());
    }

    private void InstanceUis()
    {
        var wg = Game.Model.Level.WordGroup;
        var wds = Game.Model.Level.WordDifficulties;
        var layout = Game.Model.Level.Layout;
        TapPadList.ClearList(p => p.Destroy());
        WordSlotMgr.SetDisplay(wg.Key.Length);
        for (var i = 0; i < wg.Key.Length; i++)
        {
            var alphabet = wg.Key[i];
            var wordDifficulty = wds[i];
            var index = i;
            var pad = TapPadList.Instance(view =>
            {
                var p = new TapPad(
                    prefabView: view,
                    onTapAction: pad => GamePlayController.ApplyOrder(pad.Alphabet),
                    onOutlineAction: _ => GamePlayController.Lose(),
                    onItemAction: _ => GamePlayController.Lose(),
                    index, alphabet);
                p.Apply(wordDifficulty);
                return p;
            });
            layout.Rects[i].Apply(pad.RectTransform);
        }
    }

    private void WindowsInit()
    {
        StartWindow = new WindowButtonUi(startView, () => { GamePlayController.StartGame(); }, true);
        WinWindow = new WindowButtonUi(winView, () => { GamePlayController.StartLevel(); });
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Win, bag => WinWindow.Show());
        LoseWindow = new WindowButtonUi(loseView, StartWindow.Show);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Lose, bag => LoseWindow.Show());
        CompleteWindow = new WindowButtonUi(completeView, StartWindow.Show);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Game_Win, bag => CompleteWindow.Set(bag.GetString(0)));
    }
}


