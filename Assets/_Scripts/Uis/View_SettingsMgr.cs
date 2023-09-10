using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_SettingsMgr
{
    private View_Settings View_settings { get; set; }
    public View_SettingsMgr(IView view)
    {
        View_settings = new View_Settings(view,
            onClickAction: () => { });
    }

    private class View_Settings : UiBase
    {
        private Button btn_back { get; set; }
        private Element_Audio element_audioSFX { get; set; }
        private Element_Audio element_audioBGM { get; set; }
        private Button btn_ok { get; set; }
        public View_Settings(IView v, UnityAction onClickAction) : base(v, false)
        {
            btn_back = v.Get<Button>("btn_back");
            btn_back.onClick.AddListener(() =>
            {
                Hide();
            });
            element_audioSFX = new Element_Audio(v.Get<View>("element_audioSFX"));
            element_audioBGM = new Element_Audio(v.Get<View>("element_audioBGM"));
            btn_ok = v.Get<Button>("btn_ok");
            btn_back.onClick.AddListener(() =>
            {
                onClickAction();
                Hide();
            });
        }

        private class Element_Audio : UiBase
        {
            private View_Icon iconView { get; set; }
            private TMP_Text text_title { get; set; }
            private Slider slider_volume { get; set; }
            public Element_Audio(IView v) : base(v, true)
            {
                iconView = new View_Icon(v.Get<View>("view_icon"),
                    () => { Debug.Log("Sound is on/off"); });
                text_title = v.Get<TMP_Text>("text_title");
                slider_volume = v.Get<Slider>("slider_volume");
            }

            private class View_Icon : UiBase
            {
                 private Image img_iconOn { get; set; }
                 private Image img_iconOff { get; set; }
                private Button btn_click { get; set; }
                private bool Toggle { get; set; }
                public View_Icon(IView v, UnityAction onClickToggleAction) : base(v, true)
                {
                    img_iconOn = v.Get<Image>("img_iconOn");
                    img_iconOff = v.Get<Image>("img_iconOff");
                    btn_click = v.Get<Button>("btn_click");
                    btn_click.onClick.AddListener(onClickToggleAction);
                }
                public void SetIcon(bool on)
                {
                    if (on)
                    {
                        img_iconOn.gameObject.SetActive(true);
                        img_iconOff.gameObject.SetActive(false);
                    }
                    else
                    {
                        img_iconOn.gameObject.SetActive(false);
                        img_iconOff.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
