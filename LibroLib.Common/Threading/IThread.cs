using System;

namespace LibroLib.Threading
{
    public interface IThread : IDisposable
    {
        string Name { get; set; }
        int ManagedThreadId { get; }
        ISignal ThreadStopSignal { get; }
        bool IsAlive { get; }
        bool IsStopping { get; }

        void Start ();
        void SignalToStop();
        bool Join(TimeSpan timeout);
        [Obsolete("This method is obsolete, since .NET Core does not support Thread.Abort()")]
        void Abort();
    }
}