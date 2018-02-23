using System;
using System.Threading;
using LibroLib.Threading;
using NUnit.Framework;
using ThreadPool = LibroLib.Threading.ThreadPool;

namespace LibroLib.Tests.CommonTests.ThreadingTests
{
    public class ThreadPoolTests
    {
        [Test]
        public void CreateAndDestroyEmptyPool()
        {
            using (var pool = new ThreadPool(syncObjectsFactory))
            {
                Assert.IsNotNull(pool);
                pool.StopAllThreads(new ThreadPoolStopSettings());
            }
        }

        [Test]
        public void StopPoolWithThreadThatHasNotYetStarted()
        {
            using (var pool = new ThreadPool (syncObjectsFactory))
            {
                pool.CreateThread(t => { });
                pool.StopAllThreads (new ThreadPoolStopSettings ());
            }
        }

        [Test]
        public void StopPoolWithThreadThatHasStarted()
        {
            using (var pool = new ThreadPool (syncObjectsFactory))
            {
                IThread thread = pool.CreateThread(t => { });
                thread.Start();
                pool.StopAllThreads (new ThreadPoolStopSettings ());
            }
        }

        [Test]
        public void StopPoolWithThreadThatDoesNotWantToStop()
        {
            using (var pool = new ThreadPool (syncObjectsFactory))
            {
                IThread thread = pool.CreateThread(
                    t => Thread.Sleep(TimeSpan.FromMinutes(1)));
                thread.Start();
                ThreadPoolStopSettings threadPoolStopSettings =
                    new ThreadPoolStopSettings();
                threadPoolStopSettings.InitialStoppingTimeout = TimeSpan.Zero;
                threadPoolStopSettings.MaxStoppingLoops = 1;
                threadPoolStopSettings.SubsequentStoppingTimeout = TimeSpan.Zero;
                pool.StopAllThreads (threadPoolStopSettings);
            }
        }

        [SetUp]
        public void Setup()
        {
            syncObjectsFactory = new SyncObjectsFactory();
        }

        private ISyncObjectsFactory syncObjectsFactory;
    }
}