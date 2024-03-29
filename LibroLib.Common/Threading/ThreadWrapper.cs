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
            get => thread.Name;
            set => thread.Name = value;
        }

        public int ManagedThreadId => thread.ManagedThreadId;

        public ISignal ThreadStopSignal => threadStopSignal;

        public bool IsAlive => thread.IsAlive;

        public bool IsStopping => isStopping;

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

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

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

        private bool disposed;
        private readonly IThreadPool ownerThreadPool;
        private readonly Thread thread;
        private readonly ISignal threadStopSignal;
        private bool isStopping;
    }
}