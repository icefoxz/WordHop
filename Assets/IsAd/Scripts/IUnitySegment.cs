using System;

namespace IsAd.Scripts
{
    public interface IUnitySegment
    {
        event Action<String> OnSegmentRecieved;
    }
}
