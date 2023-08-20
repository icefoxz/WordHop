using UnityEngine;

public record RectConfig
{
    public Vector2 Position;
    public Vector2 SizeDelta;
    public Vector2 AnchorMin;
    public Vector2 AnchorMax;
    public Vector2 AnchoredPosition;
    public bool IsItemVisible;

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