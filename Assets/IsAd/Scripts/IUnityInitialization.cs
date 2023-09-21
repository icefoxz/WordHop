using System;

namespace IsAd.Scripts
{
    public interface IUnityInitialization
    {
        event Action OnSdkInitializationCompletedEvent;
    }
}
