namespace LibroLib.Threading
{
    public class SyncObjectsFactory : ISyncObjectsFactory
    {
        public ISignal CreateAutoResetSignal(bool initialState)
        {
            return new AutoResetSignal (initialState);
        }

        public ISignal CreateManualResetSignal(bool initialState)
        {
            return new ManualResetSignal(initialState);
        }

        public ISemaphore CreateSemaphore(int initialCount, int maximumCount)
        {
            return new SemaphoreWrapper(initialCount, maximumCount);
        }

        public ISynchronizer CreateSynchronizer()
        {
            return new Synchronizer();
        }
    }
}