using System.Collections;
using AOT.BaseUis;
using AOT.Views;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class View_Badge : UiBase
{
    private TMP_Text tmp_class { get; }
    private TMP_Text tmp_level { get; }
    private CanvasGroup canvas_badge { get; }

    public View_Badge(IView v, bool display = true) : base(v, display)
    {
        tmp_class = v.Get<TMP_Text>("tmp_class");
        tmp_level = v.Get<TMP_Text>("tmp_level");
        canvas_badge = v.Get<CanvasGroup>("canvas_badge");
    }

    public void Set(string title, int level)
    {
        tmp_class.text = title;
        tmp_level.text = level.ToString();
    }

    public IEnumerator PlayFadeIn(float sec = 0.5f)
    {
        canvas_badge.alpha = 1f;
        yield return canvas_badge.DOFade(0f, sec).WaitForCompletion();
    }

    public IEnumerator PlayFadeOut(float sec = 0.5f)
    {
        canvas_badge.alpha = 0f;
        yield return canvas_badge.DOFade(1f, sec).WaitForCompletion();
    }
}