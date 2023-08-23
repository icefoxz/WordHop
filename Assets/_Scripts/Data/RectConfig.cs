using System;
using UnityEngine;

[Serializable]public record RectConfig
{
    public Vector2 Position; // 位置
    public Vector2 SizeDelta; // 大小
    public Vector2 AnchorMin; // 锚点min
    public Vector2 AnchorMax; // 锚点max 
    public Vector2 AnchoredPosition; // 锚点位置
    public bool IsItemVisible; // 是否可见

    public static RectConfig Create(RectTransform rect)
    {
        return new RectConfig
        {
            Position = rect.localPosition,
            SizeDelta = rect.sizeDelta,
            AnchorMin = rect.anchorMin,
            AnchorMax = rect.anchorMax,
            AnchoredPosition = rect.anchoredPosition,
            IsItemVisible = rect.gameObject.activeSelf
        };
    }

    public void Apply(RectTransform rect)
    {
        rect.localPosition = Position;
        rect.sizeDelta = SizeDelta;
        rect.anchorMin = AnchorMin;
        rect.anchorMax = AnchorMax;
        rect.anchoredPosition = AnchoredPosition;
        rect.gameObject.SetActive(IsItemVisible);
    }
}