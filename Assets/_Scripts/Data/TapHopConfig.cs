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

    public static TapPadConfig Create(View view)
    {
        var rect = view.GetComponent<RectTransform>();
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

    public static void Apply(IView view, TapPadConfig config, IReadOnlyDictionary<string, Sprite> spriteMapper)
    {
        var baseButton = view.Get<Button>("btn_base");
        var outlineButton = view.Get<Button>("btn_outline");
        var itemButton = view.Get<Button>("btn_item");
        var orderText = view.Get<TMP_Text>("tmp_order");
        config.tapPad.Apply(itemButton, spriteMapper);
        config.outline.Apply(outlineButton, spriteMapper);
        config.trap.Apply(baseButton, spriteMapper);
        orderText.text = config.clickOrder.ToString();
        //SetInlineRatio(view.RectTransform, baseRect, config.outlineRatio);
    }

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

    public static float GetOutlineRatio(View view)
    {
        var outlineButton = view.Get<Button>("btn_base");
        return outlineButton.GetComponent<RectTransform>().localScale.x;
    }

    public void Apply(IView view, IReadOnlyDictionary<string,Sprite> mapper) => Apply(view, this, mapper);

    public static void SetInlineRatio(RectTransform rectTransform, RectTransform baseRect, float ratio)
    {
        var size = rectTransform.sizeDelta;
        var inlineSize = new Vector2(size.x * (1 - ratio), size.y * (1 - ratio));
        baseRect.sizeDelta = inlineSize;
    }
}

public record ButtonConfig
{
    public string SpriteName;
    public RectConfig RectTransform;

    public static ButtonConfig Create(Button button)
    {
        return new ButtonConfig
        {
            SpriteName = button.image.sprite.name,
            RectTransform = RectConfig.Create(button.GetComponent<RectTransform>())
        };
    }

    public void Apply(Button button,IReadOnlyDictionary<string,Sprite> mapper)
    {
        button.image.sprite = mapper[SpriteName];
        RectTransform.Apply(button.GetComponent<RectTransform>());
    }
}
