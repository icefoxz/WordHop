using System;

namespace IsAd.Scripts
{
    public interface IUnityLevelPlayRewardedVideoManual
    {
        event Action<IronSourceAdInfo> OnAdReady;

        event Action<IronSourceError> OnAdLoadFailed;
    }
}
