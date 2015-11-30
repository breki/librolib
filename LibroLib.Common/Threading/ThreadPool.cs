using System;
using System.Collections.Generic;
using System.Threading;
//using log4net;

namespace LibroLib.Threading
{
    public class ThreadPool : IThreadPool
    {
        public ThreadPool(ISyncObjectsFactory syncObjectsFactory)
        {
            this.syncObjectsFactory = syncObjectsFactory;
        }

        public IThread CreateThread (Action<IThread> threadAction)
        {
            ThreadWrapper thread = new ThreadWrapper(this, syncObjectsFactory, threadAction);
            
            lock (this)
                threads.Add(thread);

            return thread;
        }

        public void StopAllThreads (ThreadPoolStopSettings settings)
        {
            //log.Info("Signaling all threads to stop");

            foreach (IThread thread in threads)
                thread.SignalToStop();

            List<IThread> remainingThreads = new List<IThread>(threads);

            for (int i = 0; remainingThreads.Count > 0 && i < settings.MaxStoppingLoops; i++)
            {
                Thread.Sleep (i == 0 ? settings.InitialStoppingTimeout : settings.SubsequentStoppingTimeout);

                List<IThread> threadsToProcess = new List<IThread>(remainingThreads);
                foreach (IThread thread in threadsToProcess)
                {
                    if (thread.Join(TimeSpan.Zero))
                    {
                        //if (log.IsDebugEnabled)
                        //    log.DebugFormat("Thread {0} stopped normally", thread.ManagedThreadId);

                        remainingThreads.Remove(thread);
                        thread.Dispose();
                    }
                }
            }

            foreach (IThread thread in remainingThreads)
            {
                //log.WarnFormat("Aborting thread {0} since it failed to stop normally", thread.ManagedThreadId);
                thread.Abort();
            }

            threads.Clear();
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
                    lock (this)
                    {
                        foreach (IThread thread in threads)
                            thread.Dispose();
                    }
                }

                disposed = true;
            }
        }

        private readonly ISyncObjectsFactory syncObjectsFactory;
        private bool disposed;
        private readonly List<IThread> threads = new List<IThread>();
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}