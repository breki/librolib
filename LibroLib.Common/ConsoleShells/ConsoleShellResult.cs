using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LibroLib.ConsoleShells
{
    public class ConsoleShellResult
    {
        public ConsoleShellResult()
        {
        }

        public ConsoleShellResult(int exitCode)
        {
            this.exitCode = exitCode;
        }

        public ConsoleShellResult(IConsoleCommand command)
        {
            Contract.Requires(command != null);
            commandsToExecute.Add(command);
        }

        public int? ExitCode
        {
            get { return exitCode; }
            set { exitCode = value; }
        }

        public IList<IConsoleCommand> CommandsToExecute
        {
            get
            {
                Contract.Ensures(Contract.Result<IList<IConsoleCommand>>() != null);
                return commandsToExecute;
            }
        }

        private int? exitCode;
        private readonly IList<IConsoleCommand> commandsToExecute = new List<IConsoleCommand>();
    }
}