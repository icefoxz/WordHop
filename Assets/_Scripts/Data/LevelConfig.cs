using System;
using System.Collections.Generic;

[Serializable]
public record LevelConfig
{
    public int countdownTime; // 关卡的倒计时时间
    public List<TapPadConfig> tapPads = new List<TapPadConfig>(); // 关卡内所有的TapHop配置
}

[Serializable]
public record LayoutConfig
{
    public List<RectConfig> Rects = new List<RectConfig>(); // 关卡布局的TapHop配置
}