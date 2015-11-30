namespace LibroLib.Threading
{
    public interface ISemaphore : IWaitHandle
    {
        int Release();
        int Release(int releaseCount);
    }
}