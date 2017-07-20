using System;
using System.Threading;

namespace LibroLib.Threading
{
    public class ManualResetSignal : WindowsWaitHandleBase, ISignal
    {
        public ManualResetSignal(bool initialState)
        {
            wrappedEvent = new ManualResetEvent(initialState);
        }

        public override WaitHandle WrappedWaitHandle => wrappedEvent;

        public override bool Wait()
        {
            return wrappedEvent.WaitOne();
        }

        public override bool Wait(TimeSpan timeout)
        {
            return wrappedEvent.WaitOne(timeout);
        }

        public void Set()
        {
            wrappedEvent.Set();
        }

        public void Reset()
        {
            wrappedEvent.Reset();
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            // clean native resources         

            if (disposing)
            {
                if (wrappedEvent != null)
                {
#if NET35
                        wrappedEvent.Close ();
#else
                    wrappedEvent.Dispose ();
#endif
                    wrappedEvent = null;
                }
            }

            disposed = true;
        }

        private bool disposed;
        private ManualResetEvent wrappedEvent;
    }
}