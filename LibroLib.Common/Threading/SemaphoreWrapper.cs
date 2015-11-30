using System;
using System.Threading;

namespace LibroLib.Threading
{
    public class SemaphoreWrapper : WindowsWaitHandleBase, ISemaphore
    {
        public SemaphoreWrapper(int initialCount, int maximumCount)
        {
            wrappedSemaphore = new Semaphore(initialCount, maximumCount);
        }

        public override WaitHandle WrappedWaitHandle
        {
            get { return wrappedSemaphore; }
        }

        public override bool Wait()
        {
            return wrappedSemaphore.WaitOne();
        }

        public override bool Wait(TimeSpan timeout)
        {
            return wrappedSemaphore.WaitOne(timeout);
        }

        public int Release()
        {
            return wrappedSemaphore.Release();
        }

        public int Release(int releaseCount)
        {
            return wrappedSemaphore.Release(releaseCount);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources            
                    if (wrappedSemaphore != null)
                        wrappedSemaphore.Close();
                }

                disposed = true;
            }
        }

        private bool disposed;
        private readonly Semaphore wrappedSemaphore;
    }
}