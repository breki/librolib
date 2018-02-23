using System;
using System.Threading;

namespace LibroLib.Threading
{
    public abstract class WindowsWaitHandleBase : IWaitHandle
    {
        public abstract WaitHandle WrappedWaitHandle { get; }

        public abstract bool Wait();
        public abstract bool Wait(TimeSpan timeout);
        public abstract void Dispose();
    }
}