using System;

namespace LibroLib.Threading
{
    public class ThreadPoolStopSettings
    {
        public int MaxStoppingLoops
        {
            get { return maxStoppingLoops; }
            set { maxStoppingLoops = value; }
        }

        public TimeSpan InitialStoppingTimeout
        {
            get { return initialStoppingTimeout; }
            set { initialStoppingTimeout = value; }
        }

        public TimeSpan SubsequentStoppingTimeout
        {
            get { return subsequentStoppingTimeout; }
            set { subsequentStoppingTimeout = value; }
        }

        private int maxStoppingLoops = 5;
        private TimeSpan initialStoppingTimeout = TimeSpan.FromMilliseconds(1000);
        private TimeSpan subsequentStoppingTimeout = TimeSpan.FromMilliseconds(5000);
    }
}