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
            onBgmToggleAction: OnBgmToggleAction,
            onSfxToggleAction: OnSfxToggleAction,
            onBgmVolumeAction: f => AudioManager.SetBgmVolume(f),
            onSfxVolumeAction: f => AudioManager.SetSfxVolume(f));
    }

    private void OnSfxToggleAction(bool isOn)
    {
        var isMute = !isOn;
        AudioManager.SetSfxMute(isMute);
        var volume = isMute ? 0 : Pref.GetSfxVolume();
        View_settings.SetSfx(isMute, volume);
    }

    private void OnBgmToggleAction(bool isOn)
    {
        var isMute = !isOn;
        AudioManager.SetBgmMute(isMute);
        var volume = isMute ? 0 : Pref.GetBgmVolume();
        View_settings.SetBgm(isMute, volume);
    }

    private void SetCurrent()
    {
        Set(isBgmMute: !View_settings.IsBgmOn, 
            isSfxMute: !View_settings.IsSfxOn, 
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

    public void Init()
    {
        LoadPref();
    }

    private void LoadPref()
    {
        var bgmMute = Pref.GetBgmMute();
        var sfcMute = Pref.GetSfxMute();
        var bgmVolume = bgmMute ? 0 : Pref.GetBgmVolume();
        var sfxVolume = sfcMute ? 0 : Pref.GetSfxVolume();
        View_settings.Set(bgmMute, sfcMute, bgmVolume, sfxVolume);
    }

    private void HideSettings()
    {
        Game.Pause(false);
        View_settings.Hide();
    }

    public void Show()
    {
        Game.Pause(true);
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
        public bool IsBgmOn => !element_audioBGM.IsMute;
        public bool IsSfxOn => !element_audioSFX.IsMute;

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
            SetBgm(bgmMute, bgmVolume);
            SetSfx(sfxMute, sfxVolume);
        }

        public void SetSfx(bool sfxMute, float sfxVolume)
        {
            element_audioSFX.SetMute(sfxMute);
            element_audioSFX.SetVolume(sfxVolume);
        }

        public void SetBgm(bool bgmMute, float bgmVolume)
        {
            element_audioBGM.SetMute(bgmMute);
            element_audioBGM.SetVolume(bgmVolume);
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
                view_icon = new View_Icon(v.Get<View>("view_icon"), isOn =>
                {
                    onToggleAction(isOn);
                    SetMute(!isOn);
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

            public void SetMute(bool isMute)
            {
                IsMute = isMute;
                slider_volume.interactable = !IsMute;
                view_icon.SetToggle(!IsMute);
            }


            private class View_Icon : UiBase
            {
                private Image img_iconOn { get; set; }
                private Image img_iconOff { get; set; }
                private Button btn_click { get; set; }
                private bool ToggleOn { get; set; }
                private event UnityAction<bool> OnClickToggleAction;

                public View_Icon(IView v, UnityAction<bool> onClickToggleAction) : base(v, true)
                {
                    img_iconOn = v.Get<Image>("img_iconOn");
                    img_iconOff = v.Get<Image>("img_iconOff");
                    btn_click = v.Get<Button>("btn_click");
                    OnClickToggleAction = onClickToggleAction;
                    btn_click.onClick.AddListener(SwitchToggle);
                }

                private void SwitchToggle()
                {
                    SetToggle(!ToggleOn);
                    OnClickToggleAction?.Invoke(ToggleOn);
                }

                public void SetToggle(bool isOn)
                {
                    ToggleOn = isOn;
                    img_iconOn.gameObject.SetActive(ToggleOn);
                    img_iconOff.gameObject.SetActive(!ToggleOn);
                }
            }
        }
    }
}
