using System.Collections;
using AOT.BaseUis;
using AOT.Views;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_StageClearMgr
{
    private View_StageClear View_stageClear { get; set; }

    public View_StageClearMgr(IView view, UnityAction onClickAction)
    {
        View_stageClear = new View_StageClear(view, onClickAction,
            onClickAdAction: () => { Debug.Log("Watch ads to get reward"); }
        );
    }

    public void Hide() => View_stageClear.Hide();
    public void Show() => View_stageClear.Show();
    public void HideOptions() => View_stageClear.HideOptions(); 
    private void SetExpBar(int exp, int max) => View_stageClear.SetExpBar(exp, max);
    private void SetExpValue(int exp) => View_stageClear.SetExpValue(exp);
    private void SetLevel(string title,int level) => View_stageClear.SetLevel(title, level);

    /// <summary>
    /// 播放升级记录
    /// </summary>
    /// <param name="stars"></param>
    /// <param name="upgrade"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public IEnumerator PlayExpGrowing(string title,int stars, UpgradingRecord upgrade, 
        UnityAction<GameObject> transformAction , float seconds = 1f)
    {
        var firstRec = upgrade.Levels[0];
        View_stageClear.ResetWindowPos();
        Show();
        View_stageClear.PlayStars(stars);
        //如果不是升级,仅增加经验的演示
        if (upgrade.Levels.Count == 1)
        {
            Preset(upgrade.UpgradeExp, firstRec);
            SetExpBar(firstRec.FromExp, firstRec.MaxExp);
            yield return View_stageClear.PlayToValue(firstRec.ToExp, firstRec.MaxExp, seconds)
                .WaitForCompletion();
            yield break;
        }

        var half = seconds / 2;
        //升级演示
        for (var i = 0; i < upgrade.Levels.Count - 1; i++) //留下最后一个level记录各别演示
        {
            var rec = upgrade.Levels[i];
            Preset(upgrade.UpgradeExp, rec);
            yield return View_stageClear.PlayToValue(rec.MaxExp, rec.MaxExp, half)
                .WaitForCompletion();
            View_stageClear.PlayLevelAura();
            var nextRec = upgrade.Levels[i + 1];
            View_stageClear.SetExpBar(0, nextRec.MaxExp);
            yield return View_stageClear.PlayToLevel(title, nextRec.Level, transformAction, half);
            yield return new WaitForSeconds(0.5f);
            yield return View_stageClear.PlayToValue(nextRec.ToExp, nextRec.MaxExp, half)
                .WaitForCompletion();
        }

        //预设UI
        void Preset(int exp, UpgradingRecord.LevelRecord re)
        {
            SetLevel(title, re.Level);
            SetExpBar(re.FromExp, re.MaxExp);
            SetExpValue(exp);
        }
    }
    public void SetBadge(BadgeConfiguration badgeCfg)=> View_stageClear.SetBadge(badgeCfg);
    public void SetCard(CardArg arg, bool resetPos) => View_stageClear.SetCard(arg, resetPos);
    public void SetCardAction(UnityAction onceAction) => View_stageClear.SetCardAction(onceAction);

    public IEnumerator ShowOptions(float localY, float seconds) => View_stageClear.ShowOptions(localY, seconds);

    public void ClearCard()
    {
        View_stageClear.SetAlpha(0);
        View_stageClear.SetCardModeActive();
    }

    public IEnumerator FadeOutCard(float seconds) => View_stageClear.FadeOutCard(seconds);
    public IEnumerator PlayWindowToY(float localY, float seconds) => View_stageClear.PlayWindowToY(localY, seconds);
    public void SetOptions((string title, string message, Sprite icon)[] args, UnityAction onContinueAction,
        UnityAction<int> onOpSelectedAction) =>
        View_stageClear.SetOptions(args, onContinueAction, onOpSelectedAction);
    public void DisplayCardSect(bool display) => View_stageClear.DisplayCardSect(display);

    private class View_StageClear : UiBase
    {
        private TMP_Text text_exp { get; set; }
        private Button btn_next { get; set; }
        private Button btn_ad { get; set; }
        private Image img_star_0 { get; }
        private Image img_star_1 { get; }
        private Image img_star_2 { get; }
        private Image[] Stars { get; }
        private View_userLevel view_userLevel { get; }
        private Transform trans_win { get; }
        private View_options view_options { get; }
        private View_cardSect view_cardSect { get; }

        private event UnityAction OptionContinueAction;
        private event UnityAction<int> OptionSelectAction;

        public View_StageClear(IView v, UnityAction onClickNextAction, 
            UnityAction onClickAdAction) : base(v, false)
        {
            trans_win = v.Get<Transform>("trans_win");
            img_star_0 = v.Get<Image>("img_star_0");
            img_star_1 = v.Get<Image>("img_star_1");
            img_star_2 = v.Get<Image>("img_star_2");
            Stars = new[] { img_star_0, img_star_1, img_star_2 };
            text_exp = v.Get<TMP_Text>("text_exp");
            btn_next = v.Get<Button>("btn_next");
            btn_next.onClick.AddListener(onClickNextAction);
            btn_ad = v.Get<Button>("btn_ad");
            btn_ad.onClick.AddListener(onClickAdAction);
            view_userLevel = new View_userLevel(v.Get<View>("view_userLevel"));
            view_options = new View_options(v.Get<View>("view_options"), () => OptionContinueAction(),
                a => OptionSelectAction(a));
            view_cardSect = new View_cardSect(v.Get<View>("view_cardSect"));
        }

        public void SetCard(CardArg arg, bool resetPos)
        {
            view_cardSect.SetCard(arg, resetPos);
            view_options.Hide();
        }

        public void SetCardAction(UnityAction onceAction) => view_cardSect.SetCardAction(onceAction);

        public void SetAlpha(float alpha) => view_cardSect.SetAlpha(alpha);
        public IEnumerator FadeOutCard(float seconds)
        {
            yield return view_cardSect.FadeOut(seconds).WaitForCompletion();
        }

        public void HideOptions() => view_options.Hide();

        public void PlayLevelAura() => view_userLevel.PlayLevelAura();
        public void SetExpValue(int exp) => text_exp.text = exp.ToString();
        public void SetExpBar(int exp, float max) => view_userLevel.SetExp(exp, max);
        public void SetBadge(BadgeConfiguration badgeCfg) => view_userLevel.SetBadge(badgeCfg);
        public void SetLevel(string title, int level) => view_userLevel.SetLevel(title, level);

        public Tween PlayToValue(int toValue, float max, float seconds) =>
            view_userLevel.PlayToValue(toValue, max, seconds);

        public IEnumerator PlayToLevel(string title, int level, UnityAction<GameObject> transformAction, float sec) =>
            view_userLevel.PlayToLevel(title, level, transformAction,sec);

        public IEnumerator ShowOptions(float cardYPos, float secs)
        {
            yield return view_cardSect.ShowOptions(cardYPos, secs).WaitForCompletion();
            view_options.Show();
        }

        public Tween PlayStars(int stars)
        {
            var t = DOTween.Sequence().Pause();
            for (var i = 0; i < Stars.Length; i++)
            {
                var star = Stars[i];
                star.gameObject.SetActive(false);
                if (i >= stars) continue;
                t.AppendCallback(()=>star.gameObject.SetActive(true))
                    .AppendInterval(0.5f);
            }
            return t.Play();
        }

        public void ResetWindowPos()
        {
            trans_win.localPosition = Vector3.zero;
            view_cardSect.HideCard();
        }
        public IEnumerator PlayWindowToY(float yPos, float secs)
        {
            yield return trans_win.DOLocalMoveY(yPos, secs).WaitForCompletion();
        }

        public void SetOptions((string title, string message, Sprite icon)[] options, 
            UnityAction continueAction, UnityAction<int> selectAction)
        {
            view_options.Set(options);
            OptionContinueAction = continueAction;
            OptionSelectAction = selectAction;
        }

        public void SetCardModeActive() => view_cardSect.SetCardModeActive();

        public void DisplayCardSect(bool display)
        {
            if (display)
                view_cardSect.Show();
            else
                view_cardSect.Hide();
        }

        private class View_userLevel : UiBase
        {
            private Slider slider_exp { get; set; }
            private TMP_Text tmp_exp { get; set; }
            private Animation anim_levelAura { get; set; }
            private View_Badge view_badge { get; }
            public View_userLevel(IView v) : base(v, true)
            {
                slider_exp = v.Get<Slider>("slider_exp");
                tmp_exp = v.Get<TMP_Text>("tmp_exp");
                anim_levelAura = v.Get<Animation>("anim_levelAura");
                view_badge = new View_Badge(v.Get<View>("view_badge"));
            }

            public void PlayLevelAura()
            {
                anim_levelAura.gameObject.SetActive(false);
                anim_levelAura.gameObject.SetActive(true);
            }

            public void SetExp(int exp,float max)
            {
                slider_exp.value = exp / max;
                UpdateExpText(exp, (int)max);
            }

            public void SetLevel(string title, int level) => view_badge.Set(title, level);

            public Tween PlayToValue(int toValue, float max, float seconds)
            {
                return slider_exp.DOValue(toValue / max, seconds)
                    .OnUpdate(() => UpdateExpText((int)(slider_exp.value * max), (int)max))
                    .OnComplete(() => UpdateExpText(toValue, (int)max));
            }

            public IEnumerator PlayToLevel(string title, int level, UnityAction<GameObject> transformAction, float sec)
            {
                var half = sec / 2;
                yield return view_badge.PlayFadeIn(half);
                transformAction(view_badge.GameObject);
                SetLevel(title, level);
                yield return view_badge.PlayFadeOut(half);
            }

            private void UpdateExpText(int value, int max)
            {
                tmp_exp.text = value + "/" + max; // 格式化为 "current/max"
            }

            public void SetBadge(BadgeConfiguration badgeCfg) =>
                BadgeConfigLoader.LoadPrefab(badgeCfg, view_badge.GameObject);
        }

        private class View_options : UiBase
        {
            private Button btn_continue { get; set; }
            private Element_option element_option_1 { get; set; }
            private Element_option element_option_2 { get; set; }
            private Element_option element_option_3 { get; set; }
            private Element_option element_option_4 { get; set; }
            private Element_option element_option_5 { get; set; }
            private Element_option element_option_6 { get; set; }
            private Element_option[] ElementOptions { get; set; }
            private Image img_optionPanel { get; set; }
            public View_options(IView v, UnityAction onClickContinueAction, UnityAction<int> onClickAction) : base(v, false)
            {
                btn_continue = v.Get<Button>("btn_continue");
                btn_continue.onClick.AddListener(() =>
                {
                    onClickContinueAction();
                    Hide();
                });
                element_option_1 = new Element_option(v.Get<View>("element_option_1"), ()=>onClickAction(0));
                element_option_2 = new Element_option(v.Get<View>("element_option_2"), ()=>onClickAction(1));
                element_option_3 = new Element_option(v.Get<View>("element_option_3"), ()=>onClickAction(2));
                element_option_4 = new Element_option(v.Get<View>("element_option_4"), ()=>onClickAction(3));
                element_option_5 = new Element_option(v.Get<View>("element_option_5"), ()=>onClickAction(4));
                element_option_6 = new Element_option(v.Get<View>("element_option_6"), ()=>onClickAction(5));
                ElementOptions = new Element_option[] { element_option_1, element_option_2, element_option_3, element_option_4, element_option_5, element_option_6 };
                img_optionPanel = v.Get<Image>("img_optionPanel");
            }

            public void Set((string title, string message, Sprite icon)[] options)
            {
                img_optionPanel.gameObject.SetActive(false);
                for (var i = 0; i < ElementOptions.Length; i++)
                {
                    var element = ElementOptions[i];
                    if (i >= options.Length)
                    {
                        element.Hide();
                        continue;
                    }
                    var (title, message, icon) = options[i];
                    element.SetIcon(icon);
                    element.Set(title, message);
                    element.Show();
                }
            }

            private class Element_option : UiBase
            {
                private Image img_icon { get; set; }
                private TMP_Text tmp_title { get; set; }
                private TMP_Text tmp_message { get; set; }
                private Button btn_click { get; set; }
                public Element_option(IView v, UnityAction onClickAction) : base(v, true)
                {
                    img_icon = v.Get<Image>("img_icon");
                    tmp_title = v.Get<TMP_Text>("tmp_title");
                    tmp_message = v.Get<TMP_Text>("tmp_message");
                    btn_click = v.Get<Button>("btn_click");
                    btn_click.onClick.AddListener(onClickAction);
                }
                public void SetIcon(Sprite icon) => img_icon.sprite = icon;
                public void Set(string title, string message)
                {
                    tmp_title.text = title;
                    tmp_message.text = message;
                }
            }
        }

        private class View_cardSect : UiBase
        {
            private Transform trans_cardBg { get; }
            private CanvasGroup canvas_card { get; }
            private Button btn_card { get; }
            private View_Card view_card { get; }
            private Image img_optionNotify { get; }

            public View_cardSect(IView v, bool display = true) : base(v, display)
            {
                trans_cardBg = v.Get<Transform>("trans_cardBg");
                canvas_card = v.Get<CanvasGroup>("canvas_card");
                btn_card = v.Get<Button>("btn_card");
                view_card = new View_Card(v.Get<View>("view_card"));
                img_optionNotify = v.Get<Image>("img_optionNotify");
            }

            public void SetCardAction(UnityAction onceAction)
            {
                btn_card.onClick.RemoveAllListeners();
                btn_card.onClick.AddListener(() =>
                {
                    onceAction();
                    btn_card.onClick.RemoveAllListeners();
                });
                btn_card.gameObject.SetActive(true);
                SetOptionNotify(true);
            }

            public Tween ShowOptions(float cardYPos, float secs)
            {
                trans_cardBg.gameObject.SetActive(true);
                SetOptionNotify(false);
                return canvas_card.transform.DOLocalMoveY(cardYPos, secs);
            }

            public void SetCardModeActive() => view_card.SetMode(View_Card.Modes.Active);
            public void SetAlpha(float alpha) => canvas_card.alpha = alpha;

            public void SetCard(CardArg cardArg, bool resetPos)
            {
                if (resetPos) canvas_card.transform.localPosition = Vector3.zero; // reset position
                view_card.Set(cardArg);
                btn_card.gameObject.SetActive(false);
                SetOptionNotify(false);
            }

            public Tween FadeOut(float seconds)
            {
                SetAlpha(0);
                return canvas_card.DOFade(1, seconds);
            }

            public void HideCard()
            {
                trans_cardBg.gameObject.SetActive(false);
                view_card.Hide();
            }

            private void SetOptionNotify(bool display) => img_optionNotify.gameObject.SetActive(display);
        }
    }
}
