using System;
using System.Threading;

namespace LibroLib.Misc
{
    public class RealTimeService : ITimeService
    {
        public DateTime CurrentTime
        {
            get { return DateTime.Now; }
        }

        public DateTime CurrentTimeUtc
        {
            get { return DateTime.UtcNow; }
        }

        public void Wait(TimeSpan timeSpan)
        {
            Thread.Sleep(timeSpan);
        }
    }
}