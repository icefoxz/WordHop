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
    }

    public void Show(bool displayRevive)
    {
        View_gameOver.DisplayRevive(displayRevive);
        View_gameOver.Show();
    }
    public void Set(string title, int score)
    {
        View_gameOver.Set(title, score);
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

        public void DisplayRevive(bool display) => btn_revive.gameObject.SetActive(display);

        public void Set(string title, int score)
        {
            tmp_title.text = title;
            tmp_score.text = score.ToString();
        }
    }
}
