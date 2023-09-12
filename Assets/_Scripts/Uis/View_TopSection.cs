using AOT.BaseUis;
using AOT.Views;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_TopSection : UiBase
{
    private View_SubMenu view_subMenu { get; set; }

    public View_TopSection(IView v, UnityAction onSettingAction) : base(v)
    {
        view_subMenu = new View_SubMenu(v.Get<View>("view_subMenu"), onSettingAction);
    }

    private class View_SubMenu : UiBase
    {
        private Element_Sub element_sub_mission { get; }
        private Element_Sub element_sub_rank { get; }
        private Button btn_settings { get; }

        public View_SubMenu(IView v, UnityAction onSettingAction, bool display = true) : base(v, display)
        {
            element_sub_mission = new Element_Sub(v.Get<View>("element_sub_mission"), () => { },false);
            element_sub_rank = new Element_Sub(v.Get<View>("element_sub_rank"), () => { },false);
            btn_settings = v.Get<Button>("btn_settings");
            btn_settings.onClick.AddListener(onSettingAction);
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