using System;
using System.Collections;
using System.Linq;
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
    public View_StageClearMgr(IView view,UnityAction onClickAction)
    {
        View_stageClear = new View_StageClear(view, onClickAction,
            onClickAdAction: () => { Debug.Log("Watch ads to get reward"); }
        );
    }

    private void Show() => View_stageClear.Show();
    private void SetExpBar(int exp, int max) => View_stageClear.SetExpBar(exp, max);
    private void SetExpValue(int exp) => View_stageClear.SetExpValue(exp);
    private void SetLevel(int level) => View_stageClear.SetLevel(level);

    /// <summary>
    /// 播放升级记录
    /// </summary>
    /// <param name="stars"></param>
    /// <param name="upgrade"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public IEnumerator PlayUpgrade(int stars,UpgradingRecord upgrade, float seconds = 1f)
    {
        var firstRec = upgrade.Levels[0];
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
            yield return View_stageClear.PlayToLevel(nextRec.Level, 1, 1, half);
            yield return new WaitForSeconds(0.5f);
            yield return View_stageClear.PlayToValue(nextRec.ToExp, nextRec.MaxExp, half)
                .WaitForCompletion();
        }

        //预设UI
        void Preset(int exp, UpgradingRecord.LevelRecord re)
        {
            SetLevel(re.Level);
            SetExpBar(re.FromExp, re.MaxExp);
            SetExpValue(exp);
        }
    }


    private class View_StageClear : UiBase
    {
        private TMP_Text text_exp { get; set; }
        private Button btn_next { get; set; }
        private Button btn_ad { get; set; }
        private Image img_star_0 { get; }
        private Image img_star_1 { get; }
        private Image img_star_2 { get; }
        private Image[] Stars { get; }
        private View_userLevel view_userLevel { get; set; }
        public View_StageClear(IView v, UnityAction onClickNextAction, UnityAction onClickAdAction) : base(v, false)
        {
            img_star_0 = v.Get<Image>("img_star_0");
            img_star_1 = v.Get<Image>("img_star_1");
            img_star_2 = v.Get<Image>("img_star_2");
            Stars = new[] { img_star_0, img_star_1, img_star_2 };
            text_exp = v.Get<TMP_Text>("text_exp");
            btn_next = v.Get<Button>("btn_next");
            btn_next.onClick.AddListener(() =>
            {
                onClickNextAction();
                Hide();
            });
            btn_ad = v.Get<Button>("btn_ad");
            btn_ad.onClick.AddListener(onClickAdAction);
            view_userLevel = new View_userLevel(v.Get<View>("view_userLevel"));
        }
        public void PlayLevelAura() => view_userLevel.PlayLevelAura();
        public void SetExpValue(int exp) => text_exp.text = exp.ToString();
        public void SetExpBar(int exp, float max)
        {
            view_userLevel.SetExp(exp, max);
        }

        public void SetLevel(int level) => view_userLevel.SetLevel(level);

        public Tween PlayToValue(int toValue, float max, float seconds) =>
            view_userLevel.PlayToValue(toValue, max, seconds);

        public Tween PlayToLevel(int level, int frameIndex, int wingsIndex, float sec) =>
            view_userLevel.PlayToLevel(level, frameIndex, wingsIndex, sec);

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


        private class View_userLevel : UiBase
        {
            private Slider slider_exp { get; set; }
            private TMP_Text tmp_exp { get; set; }
            private View_level view_level { get; set; }
            private Animation anim_levelAura { get; set; }
            public View_userLevel(IView v) : base(v, true)
            {
                slider_exp = v.Get<Slider>("slider_exp");
                tmp_exp = v.Get<TMP_Text>("tmp_exp");
                view_level = new View_level(v.Get<View>("view_level"));
                anim_levelAura = v.Get<Animation>("anim_levelAura");
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

            public void SetLevel(int level) => view_level.SetLevel(level);

            public Tween PlayToValue(int toValue, float max, float seconds)
            {
                return slider_exp.DOValue(toValue / max, seconds)
                    .OnUpdate(() => UpdateExpText((int)(slider_exp.value * max), (int)max))
                    .OnComplete(() => UpdateExpText(toValue, (int)max));
            }

            public Tween PlayToLevel(int level, int frameIndex, int wingsIndex, float sec)=>
                view_level.PlayToLevel(level, frameIndex, wingsIndex, sec);

            private void UpdateExpText(int value, int max)
            {
                tmp_exp.text = value + "/" + max; // 格式化为 "current/max"
            }

            private class View_level : UiBase
            {
                private Element_wings element_wings_0 { get; set; }
                private Element_wings element_wings_1 { get; set; }
                private Element_wings element_wings_2 { get; set; }
                private Image img_frame_0 { get; set; }
                private Image img_frame_1 { get; set; }
                private Image img_frame_2 { get; set; }
                private TMP_Text tmp_level { get; set; }

                private Image[] img_frames { get; }
                private Element_wings[] element_wings { get; }
                public View_level(IView v) : base(v, true)
                {
                    element_wings_0 = new Element_wings(v.Get<View>("element_wings_0"));
                    element_wings_1 = new Element_wings(v.Get<View>("element_wings_1"));
                    element_wings_2 = new Element_wings(v.Get<View>("element_wings_2"));
                    element_wings = new Element_wings[] { element_wings_0, element_wings_1, element_wings_2 };
                    img_frame_0 = v.Get<Image>("img_frame_0");
                    img_frame_1 = v.Get<Image>("img_frame_1");
                    img_frame_2 = v.Get<Image>("img_frame_2");
                    tmp_level = v.Get<TMP_Text>("tmp_level");
                    img_frames = new Image[] { img_frame_0, img_frame_1, img_frame_2 };
                }
                public void SetLevel(int level) => tmp_level.text = level.ToString();

                public Tween PlayToLevel(int level, int frameIndex, int wingsIndex, float sec)
                {
                    var (lastWings, lastFrame) = (element_wings.FirstOrDefault(o => o.IsActive()),
                        img_frames.FirstOrDefault(f => f.IsActive()));
                    var (wings, frame) = (element_wings[wingsIndex], img_frames[frameIndex]);
                    var half = sec / 2;
                    return DOTween.Sequence().Append(lastFrame.DOFade(0, half))
                        .Join(lastWings.PlayFadeAll(0, half))
                        .Join(tmp_level.DOFade(0, half))
                        .AppendCallback(() =>
                        {
                            foreach (var wing in element_wings) wing.Display(false);
                            wings.Display(true);
                            lastFrame.gameObject.SetActive(false);
                            lastFrame.DOFade(1, 0);
                            frame.DOFade(0, 0);
                            frame.gameObject.SetActive(true);
                            SetLevel(level);
                        })
                        .Append(frame.DOFade(1, half))
                        .Join(tmp_level.DOFade(1, half))
                        .Join(wings.PlayFadeAll(1, half));
                }

                private class Element_wings : UiBase
                {
                    private Image img_wing_1 { get; }
                    private Image img_wing_2 { get; }
                    public bool IsActive()=> img_wing_1.IsActive() || img_wing_2.IsActive();
                    public Element_wings(IView v, bool display = true) : base(v, display)
                    {
                        img_wing_1 = v.Get<Image>("img_wing_1");
                        img_wing_2 = v.Get<Image>("img_wing_2");
                    }

                    public Tween PlayFadeAll(float value, float secs)
                    {
                        return DOTween.Sequence()
                            .Append(img_wing_1.DOFade(value, secs))
                            .Join(img_wing_2.DOFade(value, secs))
                            .AppendCallback(() => img_wing_1.DOFade(1, 0));
                    }

                    public void Display(bool display)
                    {
                        img_wing_1.gameObject.SetActive(display);
                        img_wing_2.gameObject.SetActive(display);
                    }
                }
            }
        }
    }

}
