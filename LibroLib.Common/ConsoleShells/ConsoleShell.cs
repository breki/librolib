using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace LibroLib.ConsoleShells
{
    public class ConsoleShell : IConsoleEnvironment
    {
        public ConsoleShell(string commandExeName)
        {
            Contract.Requires(commandExeName != null);

            this.commandExeName = commandExeName;
            RegisterCommand(new HelpConsoleCommand(this));
        }

        public string CommandExeName
        {
            get { return commandExeName; }
        }

        public string Banner
        {
            get { return banner; }
            set { banner = value; }
        }

        public TextWriter OutWriter
        {
            get { return outWriter; }
            set { outWriter = value; }
        }

        public TextWriter ErrWriter
        {
            get { return errWriter; }
            set { errWriter = value; }
        }

        public IEnumerable<IConsoleCommand> ListCommands()
        {
            Contract.Ensures(Contract.Result<IEnumerable<IConsoleCommand>>() != null);

            return cmds.Values;
        }

        public ConsoleShellResult ParseCommandLine(string[] args)
        {
            Contract.Requires(args != null);
            Contract.Requires(args.Length < 1 || !ReferenceEquals(args[0], null));
            Contract.Ensures(Contract.Result<ConsoleShellResult>() != null);

            ShowBanner();

            if (args.Length == 0)
            {
                ShowHelp();
                return new ConsoleShellResult(0);
            }

            string commandId = args[0];
            Contract.Assume(!ReferenceEquals(commandId, null));

            IConsoleCommand command;
            if (!cmds.TryGetValue(commandId, out command))
            {
                ErrWriter.WriteLine("Unknown command '{0}'. Type '{1} help' for the list of all commands.", commandId, commandExeName);
                return new ConsoleShellResult(1);
            }

            IList<string> argsWithoutCmdName = args.AsQueryable().Skip(1).ToList();
            int? exitCode = command.ParseArgs(this, argsWithoutCmdName);

            if (exitCode.HasValue)
                return new ConsoleShellResult(exitCode.Value);

            return new ConsoleShellResult(command);
        }

        public ConsoleShell RegisterCommand(IConsoleCommand cmd)
        {
            Contract.Requires(cmd != null);
            cmds.Add(cmd.CommandId, cmd);
            return this;
        }

        private void ShowBanner()
        {
            outWriter.WriteLine(banner);
        }

        private void ShowHelp()
        {
            cmds.Values.First(x => x is HelpConsoleCommand).Execute(this);
        }

        private readonly Dictionary<string, IConsoleCommand> cmds = new Dictionary<string, IConsoleCommand>();
        private readonly string commandExeName;
        private string banner;
        private TextWriter outWriter = System.Console.Out;
        private TextWriter errWriter = System.Console.Error;
    }
}