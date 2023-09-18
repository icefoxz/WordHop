using AOT.BaseUis;
using AOT.Views;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_TopSection : UiBase
{
    private View_SubMenu view_subMenu { get; set; }

    public View_TopSection(IView v, UnityAction onSettingAction, UnityAction onHomeAction) : base(v)
    {
        view_subMenu = new View_SubMenu(v.Get<View>("view_subMenu"),
            onSettingAction,
            onHomeAction);
    }

    public void Init()
    {
        Game.MessagingManager.RegEvent(GameEvents.Stage_Start, _ => view_subMenu.DisplayHomeButton(true));
        Game.MessagingManager.RegEvent(GameEvents.Stage_Quit, _ => view_subMenu.DisplayHomeButton(false));
        view_subMenu.DisplayHomeButton(false);
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

        public void DisplayHomeButton(bool display) => btn_home.gameObject.SetActive(display);

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