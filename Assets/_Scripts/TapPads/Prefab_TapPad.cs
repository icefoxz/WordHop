using System;
using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Prefab_TapPad : UiBase
{
    private Button btn_outline { get; }
    private Button btn_base { get; }
    private Button btn_item { get; }
    private TMP_Text tmp_order { get; }

    private RectTransform baseRect { get; }
    private RectTransform outlineRect { get; }
    private RectTransform itemRect { get; }
    private Image outlineImage { get; }
    private Image baseImage { get; }
    private Image itemImage { get; }

    public Prefab_TapPad(IView v,
        UnityAction onBaseAction,
        UnityAction onOutlineAction,
        UnityAction itemAction,
        bool display = true)
        : base(v, display)
    {
        btn_outline = v.Get<Button>("btn_outline");
        btn_base = v.Get<Button>("btn_base");
        btn_item = v.Get<Button>("btn_item");
        tmp_order = v.Get<TMP_Text>("tmp_order");

        baseRect = btn_base.GetComponent<RectTransform>();
        outlineRect = btn_outline.GetComponent<RectTransform>();
        itemRect = btn_item.GetComponent<RectTransform>();

        outlineImage = btn_outline.GetComponent<Image>();
        baseImage = btn_base.GetComponent<Image>();
        itemImage = btn_item.GetComponent<Image>();

        btn_base.onClick.AddListener(onBaseAction);
        btn_outline.onClick.AddListener(onOutlineAction);
        btn_item.onClick.AddListener(itemAction);
    }

    // 设置文字
    public void SetText(string text,bool upperCap = true)
    {
        if(upperCap)
            text = text.ToUpper();
        tmp_order.text = text;
    }

    // 设置文字是否显示
    public void SetTextVisible(bool visible) => tmp_order.gameObject.SetActive(visible);

    // 设置外线颜色
    public void SetOutlineColor(Color color) => outlineImage.color = color;
    // 设置中心颜色
    public void SetBaseColor(Color color) => baseImage.color = color;

    // 设置按钮大小
    public void SetSize(Vector2 size, float outlineRatio = -1f)
    {
        baseRect.sizeDelta = size;
        if (outlineRatio >= 0)
            SetOutlineRatio(outlineRatio);
    }

    // 设置物件按钮大小
    public void SetItemSize(Vector2 size) => itemRect.sizeDelta = size;

    // 设置物件按钮位置
    public void SetItemPosition(Vector2 position) => itemRect.anchoredPosition = position;

    // 设置物件按钮颜色
    public void SetItemColor(Color color) => itemImage.color = color;

    // 设置物件按钮是否可见
    public void SetItemVisible(bool visible) => btn_item.gameObject.SetActive(visible);

    // 设置物件图标
    public void SetItemSprite(Sprite sprite) => itemImage.sprite = sprite;

    // 设置外线比率(实际上是内线)
    public void SetOutlineRatio(float ratio)
    {
        TapPadConfig.SetInlineRatio(RectTransform, baseRect, ratio);
    }

    public void ApplyDifficulty(float outline, float item)
    {
        SetOutlineRatio(outline);
        SetItemVisible(item > 0);
    }
}
