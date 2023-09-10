using System;
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
    public View_StageClearMgr(IView view,UnityAction onClickAction)
    {
        View_stageClear = new View_StageClear(view, onClickAction,
            onClickAdAction: () => { Debug.Log("Watch ads to get reward"); }
        );
    }

    private void Show() => View_stageClear.Show();
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
    public IEnumerator PlayUpgrade(string title,int stars, UpgradingRecord upgrade, UnityAction<GameObject> transformAction ,float seconds = 1f)
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

        public void SetBadge(BadgeConfiguration badgeCfg) => view_userLevel.SetBadge(badgeCfg);

        public void SetLevel(string title, int level) => view_userLevel.SetLevel(title, level);

        public Tween PlayToValue(int toValue, float max, float seconds) =>
            view_userLevel.PlayToValue(toValue, max, seconds);

        public IEnumerator PlayToLevel(string title, int level, UnityAction<GameObject> transformAction, float sec) =>
            view_userLevel.PlayToLevel(title, level, transformAction,sec);

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
            //private View_level view_level { get; set; }
            private Animation anim_levelAura { get; set; }
            private View_Badge view_badge { get; }
            public View_userLevel(IView v) : base(v, true)
            {
                slider_exp = v.Get<Slider>("slider_exp");
                tmp_exp = v.Get<TMP_Text>("tmp_exp");
                //view_level = new View_level(v.Get<View>("view_level"));
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
    }
}