namespace LibroLib.Misc
{
    public interface IExecutionContext
    {
        bool ShouldAbort { get; }
    }
}