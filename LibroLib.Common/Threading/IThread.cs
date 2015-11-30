using System;

namespace LibroLib.Threading
{
    public interface IThread : IDisposable
    {
        string Name { get; set; }
        int ManagedThreadId { get; }
        ISignal ThreadStopSignal { get; }
        bool IsStopping { get; }

        void Start ();
        void SignalToStop();
        bool Join(TimeSpan timeout);
        void Abort();
    }
}