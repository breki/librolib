using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Threading;

namespace LibroLib.Threading
{
    public class ThreadWrapper : IThread
    {
        public ThreadWrapper(
            IThreadPool ownerThreadPool,
            ISyncObjectsFactory syncObjectsFactory,
            Action<IThread> threadAction)
        {
            this.ownerThreadPool = ownerThreadPool;
            Contract.Requires(syncObjectsFactory != null);
            Contract.Requires(threadAction != null);

#pragma warning disable CC0031 // Check for null before calling a delegate
            thread = new Thread(() => threadAction(this));
#pragma warning restore CC0031 // Check for null before calling a delegate
            thread.CurrentCulture = CultureInfo.InvariantCulture;
            thread.CurrentUICulture = CultureInfo.InvariantCulture;
            threadStopSignal = syncObjectsFactory.CreateManualResetSignal(false);
        }

        public string Name
        {
            get { return thread.Name; }
            set { thread.Name = value; }
        }

        public int ManagedThreadId
        {
            get { return thread.ManagedThreadId; }
        }

        public ISignal ThreadStopSignal
        {
            get { return threadStopSignal; }
        }

        public bool IsAlive
        {
            get { return thread.IsAlive; }
        }

        public bool IsStopping
        {
            get { return isStopping; }
        }

        public void Start()
        {
            thread.Start();
        }

        public void SignalToStop()
        {
            threadStopSignal.Set();
            isStopping = true;
        }

        public bool Join(TimeSpan timeout)
        {
            return thread.Join(timeout);
        }

        public void Abort()
        {
            thread.Abort();
        }

        public void Dispose()
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
                    if (thread != null)
                        thread.Abort();

                    if (threadStopSignal != null)
                        threadStopSignal.Dispose();
                }

                disposed = true;
            }
        }

        private bool disposed;
        private readonly IThreadPool ownerThreadPool;
        private readonly Thread thread;
        private readonly ISignal threadStopSignal;
        private bool isStopping;
    }
}