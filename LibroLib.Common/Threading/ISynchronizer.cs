using System;
using System.Diagnostics.Contracts;

namespace LibroLib.Threading
{
    [ContractClass(typeof(ISynchronizerContract))]
    public interface ISynchronizer
    {
        void AssignWaitHandles(params IWaitHandle[] handles);
        int WaitAny();
        int WaitAny(TimeSpan timeout);
        bool WaitAll();
        bool WaitAll(TimeSpan timeout);
    }

    [ContractClassFor(typeof(ISynchronizer))]
    // ReSharper disable once InconsistentNaming
    internal abstract class ISynchronizerContract : ISynchronizer
    {
        void ISynchronizer.AssignWaitHandles(params IWaitHandle[] handles)
        {
            Contract.Requires(handles != null);
            Contract.Requires(Contract.ForAll(handles, x => x != null));

            throw new NotImplementedException();
        }

        int ISynchronizer.WaitAny()
        {
            throw new NotImplementedException();
        }

        int ISynchronizer.WaitAny(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        bool ISynchronizer.WaitAll()
        {
            throw new NotImplementedException();
        }

        bool ISynchronizer.WaitAll(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }
}