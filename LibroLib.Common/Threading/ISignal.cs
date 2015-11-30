namespace LibroLib.Threading
{
    public interface ISignal : IWaitHandle
    {
        void Set();
        void Reset();
    }
}