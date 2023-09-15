using AOT.BaseUis;
using AOT.Views;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_Home : UiBase
{
    private Button btn_tapToStart { get; }
    private View_menu_achievement view_menu_achievement { get; }

    public View_Home(IView v, UnityAction onTapToStart, UnityAction onAchievementAction, bool display = true) : base(v,
        display)
    {
        btn_tapToStart = v.Get<Button>("btn_tapToStart");
        btn_tapToStart.onClick.AddListener(onTapToStart);
        view_menu_achievement = new View_menu_achievement(
            v.Get<View>("view_menu_achievement"), 
            onAchievementAction);
    }

    private class View_menu_achievement : UiBase
    {
        private Button btn_click { get; }
        public View_menu_achievement(IView v,UnityAction onclickAction ,bool display = true) : base(v, display)
        {
            btn_click = v.Get<Button>("btn_click");
            btn_click.onClick.AddListener(onclickAction);
        }
    }
}