using System;

namespace IsAd.Scripts
{
    public interface IUnityRewardedVideoManual
    {
        event Action OnRewardedVideoAdReady;

        event Action<IronSourceError> OnRewardedVideoAdLoadFailed;

    }
}