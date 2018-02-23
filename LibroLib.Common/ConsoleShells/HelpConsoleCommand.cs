using System.Collections.Generic;
using System.Linq;

namespace LibroLib.ConsoleShells
{
    public class HelpConsoleCommand : IConsoleCommand
    {
        public HelpConsoleCommand(ConsoleShell consoleShell)
        {
            this.consoleShell = consoleShell;
        }

        public string CommandId
        {
            get { return "help"; }
        }

        public object Description
        {
            get { return "displays the command line help"; }
        }

        public int? ParseArgs(IConsoleEnvironment consoleEnvironment, IList<string> args)
        {
            return null;
        }

        public string ConstructUsageHelpText(string indentation)
        {
            return null;
        }

        public int Execute(IConsoleEnvironment env)
        {
            env.OutWriter.WriteLine();
            env.OutWriter.WriteLine("USAGE: {0} <command> {{<parameters>}}", consoleShell.CommandExeName);
            env.OutWriter.WriteLine();

            env.OutWriter.WriteLine("COMMANDS:");

            string indentation = "   ";
            foreach (IConsoleCommand cmd in consoleShell.ListCommands().OrderBy(x => x.CommandId))
            {
                env.OutWriter.WriteLine("{0}{1} - {2}", indentation, cmd.CommandId, cmd.Description);

                string usageHelpText = cmd.ConstructUsageHelpText(indentation);
                if (usageHelpText != null)
                    env.OutWriter.WriteLine(usageHelpText);
            }

            return 0;
        }

        private readonly ConsoleShell consoleShell;
    }
}