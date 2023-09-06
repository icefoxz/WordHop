using System;
using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;
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
        private View_Level levelView { get; set; }
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
            levelView = new View_Level(v.Get<View>("view_level"));
        }

        private class View_Level : UiBase
        {
            private GameObject obj_wings_0 { get; set; }
            private GameObject obj_wings_1 { get; set; }
            private GameObject obj_wings_2 { get; set; }
            private Image img_frame_0 { get; set; }
            private Image img_frame_1 { get; set; }
            private Image img_frame_2 { get; set; }
            private TMP_Text tmp_level { get; set; }
            public View_Level(IView v) : base(v, true)
            {
                obj_wings_0 = v.Get<GameObject>("obj_wings_0");
                obj_wings_1 = v.Get<GameObject>("obj_wings_1");
                obj_wings_2 = v.Get<GameObject>("obj_wings_2");
                img_frame_0 = v.Get<Image>("img_frame_0");
                img_frame_1 = v.Get<Image>("img_frame_1");
                img_frame_2 = v.Get<Image>("img_frame_2");
                tmp_level = v.Get<TMP_Text>("tmp_level");
            }
            public void SetLevel(int level) => tmp_level.text = level.ToString();
        }
    }
}
