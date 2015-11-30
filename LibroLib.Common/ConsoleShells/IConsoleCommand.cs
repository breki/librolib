using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LibroLib.ConsoleShells
{
    [ContractClass (typeof(IConsoleCommandContract))]
    public interface IConsoleCommand
    {
        string CommandId { get; }
        object Description { get; }

        int? ParseArgs(IConsoleEnvironment consoleEnvironment, IList<string> args);
        string ConstructUsageHelpText (string indentation);
        int Execute(IConsoleEnvironment env);
    }

    [ContractClassFor (typeof(IConsoleCommand))]
    // ReSharper disable once InconsistentNaming
    internal abstract class IConsoleCommandContract : IConsoleCommand
    {
        public string CommandId
        {
            get
            {
                Contract.Ensures (Contract.Result<string>() != null);
                return default(string);
            }
        }

        public object Description
        {
            get { return default(object); }
        }

        public int? ParseArgs(IConsoleEnvironment consoleEnvironment, IList<string> args)
        {
            Contract.Requires(consoleEnvironment != null);
            Contract.Requires(args != null);
            return default(int?);
        }

        public string ConstructUsageHelpText(string indentation)
        {
            return default(string);
        }

        public int Execute(IConsoleEnvironment env)
        {
            Contract.Requires(env != null);
            return default(int);
        }
    }
}