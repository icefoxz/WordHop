using System;

namespace IsAd.Scripts
{
    public interface IUnityImpressionData
    {
        event Action<IronSourceImpressionData> OnImpressionDataReady;

        event Action<IronSourceImpressionData> OnImpressionSuccess;
    }
}
