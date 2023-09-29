using AOT.BaseUis;
using AOT.Utls;
using AOT.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_TopSection : UiBase
{
    private enum Modes
    {
        Home, Stage
    }
    private View_SubMenu view_subMenu { get; set; }
    private View_Badge view_badge { get; }
    private Text text_days { get; }
    private Transform trans_flag { get; }


    public View_TopSection(IView v, UnityAction onSettingAction, UnityAction onHomeAction) : base(v)
    {
        view_subMenu = new View_SubMenu(v.Get<View>("view_subMenu"),
            onSettingAction,
            onHomeAction);
        view_badge = new View_Badge(v.Get<View>("view_badge"));
        text_days = v.Get<Text>("text_days");
        trans_flag = v.Get<Transform>("trans_flag");

    }

    public void Init()
    {
        Game.MessagingManager.RegEvent(GameEvents.Stage_Start, _ => SetMode(Modes.Stage));
        Game.MessagingManager.RegEvent(GameEvents.Stage_Quit, _ => SetMode(Modes.Home));
        Game.MessagingManager.RegEvent(GameEvents.Stage_Level_Start, LevelStartLoadInfo);
        SetMode(Modes.Home);
    }

    private void SetMode(Modes mode)
    {
        trans_flag.gameObject.SetActive(mode == Modes.Stage);
        view_badge.Display(mode == Modes.Stage);
        view_subMenu.SetMode(mode);
    }

    private void SetDays(int days) => text_days.text = days.ToString();

    private void LevelStartLoadInfo(ObjectBag obj)
    {
        var player = Game.Model.Player;
        var job = player.Current.Job;
        SetDays(player.Days);
        SetBadge(job.Title, player.QualityLevel);
    }

    private void SetBadge(string title, int level)
    {
        view_badge.Set(title, level);
        var badgeCfg = Game.Model.Player.GetBadgeCfg();
        BadgeConfigLoader.LoadPrefab(badgeCfg, view_badge.GameObject);
    }

    private class View_SubMenu : UiBase
    {

        private Element_Sub element_sub_mission { get; }
        private Element_Sub element_sub_rank { get; }
        private Button btn_settings { get; }
        private Button btn_home { get; }

        public View_SubMenu(IView v, UnityAction onSettingAction, UnityAction onHomeAction, bool display = true) :
            base(v, display)
        {
            element_sub_mission = new Element_Sub(v.Get<View>("element_sub_mission"), () => { }, false);
            element_sub_rank = new Element_Sub(v.Get<View>("element_sub_rank"), () => { }, false);
            btn_home = v.Get<Button>("btn_home");
            btn_home.onClick.AddListener(onHomeAction);
            btn_settings = v.Get<Button>("btn_settings");
            btn_settings.onClick.AddListener(onSettingAction);
        }

        public void SetMode(Modes mode)
        {
            btn_home.gameObject.SetActive(mode == Modes.Stage);
        }

        private class Element_Sub : UiBase
        {
            private Image img_icon { get; }
            private Image img_notify { get; }
            private Button btn_click { get; }

            public Element_Sub(IView v, UnityAction onClickAction, bool display) : base(v,false)
            {
                img_icon = v.Get<Image>("img_icon");
                img_notify = v.Get<Image>("img_notify");
                btn_click = v.Get<Button>("btn_click");
                btn_click.onClick.AddListener(onClickAction);
            }

            public void SetNotify(bool display) => img_notify.gameObject.SetActive(display);
        }
    }
}