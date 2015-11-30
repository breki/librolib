using System;

namespace LibroLib.Misc
{
    public interface ITimeService
    {
        DateTime CurrentTime { get; }
        DateTime CurrentTimeUtc { get; }

        void Wait(TimeSpan timeSpan);
    }
}