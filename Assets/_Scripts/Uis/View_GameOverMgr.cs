using System;
using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_GameOverMgr
{
    private View_GameOver View_gameOver { get; set; }

    public View_GameOverMgr(IView view, UnityAction onOkAction, UnityAction onReviveAction)
    {
        View_gameOver = new View_GameOver(view, onOkAction, onReviveAction);
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Lose, bag => View_gameOver.Show());
    }
    private class View_GameOver : UiBase
    {
        private TMP_Text tmp_score { get; set; }
        private TMP_Text tmp_title { get; set; }
        private Button btn_ok { get; set; }
        private Button btn_revive { get; set; }
        public View_GameOver(IView v, UnityAction onOkAction,UnityAction onReviveAction) : base(v, false)
        {
            tmp_score = v.Get<TMP_Text>("tmp_score");
            tmp_title = v.Get<TMP_Text>("tmp_title");
            btn_ok = v.Get<Button>("btn_ok");
            btn_revive = v.Get<Button>("btn_revive");
            btn_ok.onClick.AddListener(() =>
            {
                onOkAction();
                Hide();
            });
            btn_revive.onClick.AddListener(onReviveAction);
        }
    }
}
