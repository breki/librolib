using System;
using System.Diagnostics.Contracts;

namespace LibroLib.Threading
{
    [ContractClass(typeof(IThreadPoolContract))]
    public interface IThreadPool : IDisposable
    {
        IThread CreateThread(Action<IThread> threadAction);
        void StopAllThreads(ThreadPoolStopSettings settings);
    }

    [ContractClassFor(typeof(IThreadPool))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IThreadPoolContract : IThreadPool
    {
        IThread IThreadPool.CreateThread(Action<IThread> threadAction)
        {
            Contract.Requires(threadAction != null);
            Contract.Ensures(Contract.Result<IThread>() != null);

            throw new NotImplementedException();
        }

        void IThreadPool.StopAllThreads(ThreadPoolStopSettings settings)
        {
            Contract.Requires (settings != null);

            throw new NotImplementedException ();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}