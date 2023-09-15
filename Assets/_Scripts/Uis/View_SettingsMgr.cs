using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_SettingsMgr
{
    private View_Settings View_settings { get; set; }
    private AudioManager AudioManager => Game.AudioManager;

    public View_SettingsMgr(IView view)
    {
        View_settings = new View_Settings(view, onClickAction: () =>
            {
                HideSettings();
                SetCurrent();
            },
            onBgmToggleAction: AudioManager.SetBgmMute,
            onSfxToggleAction: AudioManager.SetSfxMute,
            onBgmVolumeAction: AudioManager.SetBgmVolume,
            onSfxVolumeAction: AudioManager.SetSfxVolume);
    }

    private void SetCurrent()
    {
        Set(isBgmMute: View_settings.IsBgmOn, 
            isSfxMute: View_settings.IsSfxOn, 
            bgmVolume: View_settings.BgmVolume, 
            sfxVolume: View_settings.SfxVolume);
    }

    public void Set(bool isBgmMute, bool isSfxMute, float bgmVolume, float sfxVolume)
    {
        View_settings.Set(isBgmMute, isSfxMute, bgmVolume, sfxVolume);
        Pref.SetBgmMute(isBgmMute);
        Pref.SetSfxMute(isSfxMute);
        Pref.SetBgmVolume(bgmVolume);
        Pref.SetSfxVolume(sfxVolume);
    }

    public void LoadPref()
    {
        var gbmMute = Pref.GetBgmMute();
        var sfcMute = Pref.GetSfxMute();
        var bgmVolume = Pref.GetBgmVolume();
        var sfxVolume = Pref.GetSfxVolume();
        View_settings.Set(gbmMute, sfcMute, bgmVolume, sfxVolume);
    }

    private void HideSettings()
    {
        Time.timeScale = 1;
        View_settings.Hide();
    }

    public void Show()
    {
        Time.timeScale = 0;
        View_settings.Show();
    }

    private class View_Settings : UiBase
    {
        private Button btn_back { get; set; }
        private Element_Audio element_audioSFX { get; set; }
        private Element_Audio element_audioBGM { get; set; }
        private Button btn_ok { get; set; }

        public float BgmVolume => element_audioBGM.Volume;
        public float SfxVolume => element_audioSFX.Volume;
        public bool IsBgmOn => element_audioBGM.IsMute;
        public bool IsSfxOn => element_audioSFX.IsMute;

        public View_Settings(IView v, UnityAction onClickAction,
            UnityAction<bool> onBgmToggleAction,
            UnityAction<bool> onSfxToggleAction,
            UnityAction<float> onBgmVolumeAction,
            UnityAction<float> onSfxVolumeAction) : base(v, false)
        {
            btn_back = v.Get<Button>("btn_back");
            btn_back.onClick.AddListener(Hide);
            element_audioSFX = new Element_Audio(v.Get<View>("element_audioSFX"), onSfxToggleAction, onSfxVolumeAction);
            element_audioBGM = new Element_Audio(v.Get<View>("element_audioBGM"), onBgmToggleAction, onBgmVolumeAction);
            btn_ok = v.Get<Button>("btn_ok");
            btn_back.onClick.AddListener(onClickAction);
            btn_ok.onClick.AddListener(onClickAction);
        }

        public void Set(bool bgmMute, bool sfxMute, float bgmVolume, float sfxVolume)
        {
            element_audioBGM.SetToggle(bgmMute);
            element_audioSFX.SetToggle(sfxMute);
            element_audioBGM.SetVolume(bgmVolume);
            element_audioSFX.SetVolume(sfxVolume);
        }

        private class Element_Audio : UiBase
        {
            private View_Icon view_icon { get; set; }
            private TMP_Text text_title { get; set; }
            private Slider slider_volume { get; set; }
            public bool IsMute { get; private set; }
            public float Volume { get; private set; }

            public Element_Audio(IView v, 
                UnityAction<bool> onToggleAction, 
                UnityAction<float> onVolumeAction) : base(v)
            {
                view_icon = new View_Icon(v.Get<View>("view_icon"), () =>
                {
                    onToggleAction(!IsMute);
                    SetToggle(!IsMute);
                });
                text_title = v.Get<TMP_Text>("text_title");
                slider_volume = v.Get<Slider>("slider_volume");
                slider_volume.onValueChanged.AddListener(volume =>
                {
                    onVolumeAction(volume);
                    SetVolume(volume);
                });
            }

            public void SetVolume(float volume)
            {
                slider_volume.value = volume;
                Volume = volume;
            }

            public void SetToggle(bool on)
            {
                view_icon.SetToggle(on);
                IsMute = on;
            }

            private class View_Icon : UiBase
            {
                private Image img_iconOn { get; set; }
                private Image img_iconOff { get; set; }
                private Button btn_click { get; set; }

                public View_Icon(IView v, UnityAction onClickToggleAction) : base(v, true)
                {
                    img_iconOn = v.Get<Image>("img_iconOn");
                    img_iconOff = v.Get<Image>("img_iconOff");
                    btn_click = v.Get<Button>("btn_click");
                    btn_click.onClick.AddListener(onClickToggleAction);
                }

                public void SetToggle(bool mute)
                {
                    img_iconOn.gameObject.SetActive(!mute);
                    img_iconOff.gameObject.SetActive(mute);
                }
            }
        }
    }
}
