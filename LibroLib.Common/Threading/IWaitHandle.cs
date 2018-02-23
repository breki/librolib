using System;

namespace LibroLib.Threading
{
    public interface IWaitHandle : IDisposable
    {
        bool Wait();
        bool Wait(TimeSpan timeout);
    }
}