using System;

namespace LibroLib.Threading
{
    public interface ISynchronizer
    {
        void AssignWaitHandles(params IWaitHandle[] handles);
        int WaitAny();
        int WaitAny(TimeSpan timeout);
        bool WaitAll();
        bool WaitAll(TimeSpan timeout);
    }
}