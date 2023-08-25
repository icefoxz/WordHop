using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_StageClearMgr
{
    private View_StageClear View_stageClear { get; set; }
    public View_StageClearMgr(IView view)
    {
        View_stageClear = new View_StageClear(view,
            onClickNextAction: ()=> { Debug.Log("Next Stage"); },
            onClickAdAction: ()=> { Debug.Log("Watch ads to get reward"); }
        );
        Game.MessagingManager.RegEvent(GameEvents.Stage_Game_Win, bag => StageClear_Window());
    }

    private void StageClear_Window()
    {
        View_stageClear.Show();
    }

    private class View_StageClear : UiBase
    {
        private TMP_Text text_exp { get; set; }
        private Button btn_next { get; set; }
        private Button btn_ad { get; set; }
        private View_userLevel userLevelView { get; set; }
        public View_StageClear(IView v, UnityAction onClickNextAction, UnityAction onClickAdAction) : base(v, false)
        {
            text_exp = v.Get<TMP_Text>("text_exp");
            btn_next = v.Get<Button>("btn_next");
            btn_next.onClick.AddListener(onClickNextAction);
            btn_ad = v.Get<Button>("btn_ad");
            btn_ad.onClick.AddListener(onClickAdAction);
            userLevelView = new View_userLevel(v.Get<View>("view_userLevel"));
        }
        public void SetValue(int exp) => text_exp.text = exp.ToString();

        private class View_userLevel : UiBase
        {
            private Slider slider_exp { get; set; }
            private TMP_Text tmp_exp { get; set; }
            private View_level levelView { get; set; }
            public View_userLevel(IView v) : base(v, true)
            {
                slider_exp = v.Get<Slider>("slider_exp");
                tmp_exp = v.Get<TMP_Text>("tmp_exp");
                levelView = new View_level(v.Get<View>("view_level"));
            }
            public void SetExp(int exp) => tmp_exp.text = exp.ToString();
            public void SetLevel(int level) => levelView.SetLevel(level);

            private class View_level : UiBase
            {
                private GameObject obj_wings_0 { get; set; }
                private GameObject obj_wings_1 { get; set; }
                private GameObject obj_wings_2 { get; set; }
                private Image img_frame_0 { get; set; }
                private Image img_frame_1 { get; set; }
                private Image img_frame_2 { get; set; }
                private TMP_Text tmp_level { get; set; }
                public View_level(IView v) : base(v, true)
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
}
