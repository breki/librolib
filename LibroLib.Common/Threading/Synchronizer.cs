using System;
using System.Threading;

namespace LibroLib.Threading
{
    public class Synchronizer : ISynchronizer
    {
        public void AssignWaitHandles (params IWaitHandle[] handles)
        {
            rawHandles = new WaitHandle[handles.Length];
            for (int i = 0; i < handles.Length; i++)
            {
                IWaitHandle handle = handles[i];
                WindowsWaitHandleBase windowsWaitHandleBase = (WindowsWaitHandleBase)handle;
                rawHandles[i] = windowsWaitHandleBase.WrappedWaitHandle;
            }
        }

        public int WaitAny()
        {
            return WaitHandle.WaitAny(rawHandles);
        }

        public int WaitAny(TimeSpan timeout)
        {
            return WaitHandle.WaitAny(rawHandles, timeout);
        }

        public bool WaitAll()
        {
            return WaitHandle.WaitAll(rawHandles);
        }

        public bool WaitAll(TimeSpan timeout)
        {
            return WaitHandle.WaitAll (rawHandles, timeout);
        }

        private WaitHandle[] rawHandles;
    }
}