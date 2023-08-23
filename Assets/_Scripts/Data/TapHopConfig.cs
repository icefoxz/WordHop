using System;
using System.Collections.Generic;
using AOT.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public record TapPadConfig
{
    public ButtonConfig tapPad;
    public ButtonConfig outline;
    public ButtonConfig trap;

    public float outlineRatio; // 外线比率
    public int clickOrder; // 点击顺序

    // 创建TapHop配置
    public static TapPadConfig Create(IView view)
    {
        var rect = view.GameObject.GetComponent<RectTransform>();
        var baseButton = view.Get<Button>("btn_base");
        var outlineButton = view.Get<Button>("btn_outline");
        var itemButton = view.Get<Button>("btn_item");

        var tapPadRect = baseButton.GetComponent<RectTransform>();

        var tapPadBtn = ButtonConfig.Create(itemButton);
        var outlineBtn = ButtonConfig.Create(outlineButton);
        var trapBtn = ButtonConfig.Create(baseButton);


        var ratio = 1 - (tapPadRect.sizeDelta.x / rect.sizeDelta.x);
        
        var config = new TapPadConfig
        {
            tapPad = tapPadBtn,
            outline = outlineBtn,
            trap = trapBtn,
            outlineRatio = ratio,
        };
        return config;
    }

    // 应用TapHop配置
    public void Apply(IView view, IReadOnlyDictionary<string, Sprite> spriteMapper)
    {
        var baseButton = view.Get<Button>("btn_base");
        var outlineButton = view.Get<Button>("btn_outline");
        var itemButton = view.Get<Button>("btn_item");
        var orderText = view.Get<TMP_Text>("tmp_order");
        tapPad.Apply(itemButton, spriteMapper);
        outline.Apply(outlineButton, spriteMapper);
        trap.Apply(baseButton, spriteMapper);
        orderText.text = clickOrder.ToString();
        //SetInlineRatio(view.RectTransform, baseRect, config.outlineRatio);
    }

    // 验证TapHop是否设置了所需的按钮等
    public static bool ValidateTapHops(View view)
    {
        var hasTapPad = view.Get<Button>("btn_base");
        var hasOutline = view.Get<Button>("btn_outline");
        var hasItem = view.Get<Button>("btn_item");
        var hasText = view.Get<TMP_Text>("tmp_order");
        // 验证每个TapHop是否都设置了所需的按钮等
        var isValid = view && 
                      hasTapPad &&
                      hasOutline &&
                      hasItem &&
                      hasText;
        return isValid;
    }

    // 设置外线的大小
    public static float GetOutlineRatio(View view)
    {
        var outlineButton = view.Get<Button>("btn_base");
        return outlineButton.GetComponent<RectTransform>().localScale.x;
    }

    // 设置内线的大小
    public static void SetInlineRatio(RectTransform rectTransform, RectTransform baseRect, float ratio)
    {
        var size = rectTransform.sizeDelta;
        var inlineSize = GetInlineSize(ratio, size);
        baseRect.sizeDelta = inlineSize;
    }

    // 获取内线的大小
    public static Vector2 GetInlineSize(float ratio, Vector2 size)
    {
        var inlineSize = new Vector2(size.x * (1 - ratio), size.y * (1 - ratio));
        return inlineSize;
    }
}

[Serializable]public record ButtonConfig
{
    public string SpriteName;
    public RectConfig RectTransform;

    // 创建按钮配置
    public static ButtonConfig Create(Button button)
    {
        return new ButtonConfig
        {
            SpriteName = button.image.sprite.name,
            RectTransform = RectConfig.Create(button.GetComponent<RectTransform>())
        };
    }

    // 应用按钮配置
    public void Apply(Button button,IReadOnlyDictionary<string,Sprite> mapper)
    {
        button.image.sprite = mapper[SpriteName];
        RectTransform.Apply(button.GetComponent<RectTransform>());
    }
}
