using System.Diagnostics.Contracts;
using System.IO;

namespace LibroLib.ConsoleShells
{
    [ContractClass(typeof(IConsoleEnvironmentContract))]
    public interface IConsoleEnvironment
    {
        TextWriter OutWriter { get; }
        TextWriter ErrWriter { get; }
    }

    [ContractClassFor(typeof(IConsoleEnvironment))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IConsoleEnvironmentContract : IConsoleEnvironment
    {
        public TextWriter OutWriter
        {
            get
            {
                Contract.Ensures(Contract.Result<System.IO.TextWriter>() != null);
                return default(TextWriter);
            }
        }

        public TextWriter ErrWriter
        {
            get
            {
                Contract.Ensures(Contract.Result<System.IO.TextWriter>() != null);
                return default(TextWriter);
            }
        }
    }
}