namespace LibroLib.Threading
{
    public interface ISyncObjectsFactory
    {
        ISignal CreateAutoResetSignal(bool initialState);
        ISignal CreateManualResetSignal(bool initialState);
        ISemaphore CreateSemaphore(int initialCount, int maximumCount);
        ISynchronizer CreateSynchronizer();
    }
}