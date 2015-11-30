using System;

namespace LibroLib.Threading
{
    public interface IThreadPool : IDisposable
    {
        IThread CreateThread(Action<IThread> threadAction);
        void StopAllThreads(ThreadPoolStopSettings settings);
    }
}