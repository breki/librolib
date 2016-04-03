using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LibroLib.ConsoleShells
{
    public abstract class StandardConsoleCommandBase : IConsoleCommand
    {
        public abstract string CommandId { get; }
        public abstract object Description { get; }

        public virtual int? ParseArgs(IConsoleEnvironment consoleEnvironment, IList<string> args)
        {
            int positionalArgsCovered = 0;
            foreach (string arg in args)
            {
                int? exitCode;
                if (arg.StartsWith("-"))
                {
                    exitCode = ParseNonPositionalArg(consoleEnvironment, arg);
                    if (exitCode.HasValue)
                        return exitCode;
                }
                else
                {
                    exitCode = ParsePositionalArg (consoleEnvironment, positionalArgsCovered, arg);
                    if (exitCode.HasValue)
                        return exitCode;
                    positionalArgsCovered++;
                }
            }

            for (int i = positionalArgsCovered; i < positionalArgs.Count; i++)
            {
                ArgInfo arg = positionalArgs[i];
                if (arg.IsRequired)
                {
                    consoleEnvironment.ErrWriter.WriteLine ("Required argument {0} is missing", arg.ArgName);
                    return 2;
                }
            }

            return null;
        }

        public virtual string ConstructUsageHelpText(string indentation)
        {
            StringBuilder oneLiner = new StringBuilder();
            StringBuilder completeText = new StringBuilder();

            completeText.AppendFormat(CultureInfo.InvariantCulture, "{0}{0}USAGE:{1}", indentation, Environment.NewLine);

            oneLiner.AppendFormat(CultureInfo.InvariantCulture, "{0}{0}{1}", indentation, CommandId);
            foreach (ArgInfo arg in positionalArgs)
                oneLiner.AppendFormat(CultureInfo.InvariantCulture, " {1}<{0}>{2}", arg.ArgName, arg.IsRequired ? null : "[", arg.IsRequired ? null : "]");

            if (settingsArgs.Count > 0)
                oneLiner.AppendFormat(CultureInfo.InvariantCulture, " [settings]");

            completeText.AppendLine(oneLiner.ToString());
            completeText.AppendLine();
            completeText.AppendFormat(CultureInfo.InvariantCulture, "{0}{0}PARAMETERS:{1}", indentation, Environment.NewLine);

            foreach (ArgInfo arg in positionalArgs)
            {
                completeText.AppendFormat(
                    CultureInfo.InvariantCulture, "{0}{0}{1}: {2}{3}", indentation, arg.ArgName, arg.Description, Environment.NewLine);
            }

            foreach (ArgInfo arg in settingsArgs.Values.OrderBy(x => x.ArgName))
            {
                if (!arg.IsSwitch)
                    completeText.AppendFormat (CultureInfo.InvariantCulture, "{0}{0}-{1}=value: {2}", indentation, arg.ArgName, arg.Description);
                else
                    completeText.AppendFormat (CultureInfo.InvariantCulture, "{0}{0}-{1}[=true/false]: {2}", indentation, arg.ArgName, arg.Description);

                if (arg.Alias != null)
                    completeText.AppendFormat(CultureInfo.InvariantCulture, " (alias: {0})", arg.Alias);

                completeText.AppendLine();
            }

            return completeText.ToString();
        }

        public abstract int Execute(IConsoleEnvironment env);

        protected PositionalArgBuilder AddArg (string argName, string description)
        {
            Contract.Requires(argName != null);
            Contract.Requires(description != null);
            Contract.Ensures(Contract.Result<PositionalArgBuilder>() != null);
            return new PositionalArgBuilder(this, argName, description);
        }

        protected SettingArgBuilder AddSetting (string argName, string description)
        {
            Contract.Requires(argName != null);
            Contract.Ensures(Contract.Result<SettingArgBuilder>() != null);
            return new SettingArgBuilder(this, argName, description);
        }

        protected SwitchArgBuilder AddSwitch (string argName, string description, Action<bool, CommandLineParsingContext> valueAction)
        {
            Contract.Requires(argName != null);
            Contract.Requires(valueAction != null);
            Contract.Ensures(Contract.Result<SwitchArgBuilder>() != null);
            return new SwitchArgBuilder(this, argName, description, valueAction);
        }

        private int? ParsePositionalArg (IConsoleEnvironment consoleEnvironment, int positionalArgsCovered, string arg)
        {
            Contract.Requires(consoleEnvironment != null);
            Contract.Requires (positionalArgsCovered >= 0);

            ArgInfo positionalArg = positionalArgs[positionalArgsCovered];

            CommandLineParsingContext context = new CommandLineParsingContext (consoleEnvironment);
            context.ArgName = positionalArg.ArgName;

            if (positionalArg.ArgType == typeof(string))
                positionalArg.StringValueAction(arg, context);
            else if (positionalArg.ArgType == typeof(int))
            {
                int intValue;
                if (int.TryParse(arg, out intValue))
                    positionalArg.IntValueAction(intValue, context);
                else
                {
                    consoleEnvironment.ErrWriter.WriteLine ("Argument '{0}' has an invalid value ('{1}') - it should be an integer", positionalArg.ArgName, arg);
                    return 2;
                }
            }
            else
                throw new NotImplementedException();

            return null;
        }

        private int? ParseNonPositionalArg (IConsoleEnvironment consoleEnvironment, string arg)
        {
            Contract.Requires (consoleEnvironment != null);
            Contract.Requires (arg != null);
            Contract.Requires (1 <= arg.Length);

            int valueIndex = arg.IndexOf ('=');

            string settingName;
            if (valueIndex < 0)
                settingName = arg.Substring(1).ToLowerInvariant();
            else
                settingName = arg.Substring (1, valueIndex - 1).ToLowerInvariant ();

            ArgInfo settingArg;
            if (!settingsArgs.TryGetValue(settingName, out settingArg))
            {
                if (!aliases.TryGetValue(settingName, out settingArg))
                {
                    consoleEnvironment.ErrWriter.WriteLine("Unknown setting '{0}' specified", settingName);
                    return 2;
                }
            }

            Contract.Assume(settingArg != null);
            if (!settingArg.IsSwitch)
                return ParseSettingArg(consoleEnvironment, arg, valueIndex, settingArg);
            
            return ParseSwitchArg(consoleEnvironment, arg, valueIndex, settingArg);
        }

        private static int? ParseSettingArg(IConsoleEnvironment consoleEnvironment, string arg, int valueIndex, ArgInfo settingArg)
        {
            Contract.Requires (consoleEnvironment != null);
            Contract.Requires (arg != null);
            Contract.Requires (settingArg != null);
            Contract.Requires (valueIndex == -1 || 0 <= (valueIndex + 1));
            Contract.Requires (valueIndex == -1 || (valueIndex + 1) <= arg.Length);

            if (valueIndex == -1)
            {
                consoleEnvironment.ErrWriter.WriteLine ("Setting '{0}' is missing the value", settingArg.ArgName);
                return 2;
            }

            string settingValueStr = arg.Substring(valueIndex + 1);

            CommandLineParsingContext context = new CommandLineParsingContext (consoleEnvironment);
            context.ArgName = settingArg.ArgName;

            if (settingArg.ArgType == typeof(string))
                settingArg.StringValueAction (settingValueStr, context);
            else if (settingArg.ArgType == typeof(int))
            {
                int intValue;
                if (int.TryParse (settingValueStr, out intValue))
                    settingArg.IntValueAction (intValue, context);
                else
                {
                    consoleEnvironment.ErrWriter.WriteLine ("Setting '{0}' has an invalid value ('{1}') - it should be an integer", settingArg.ArgName, settingValueStr);
                    return 2;
                }
            }
            else
                throw new NotImplementedException ();

            return null;
        }

        private static int? ParseSwitchArg(IConsoleEnvironment consoleEnvironment, string arg, int valueIndex, ArgInfo switchArg)
        {
            Contract.Requires (consoleEnvironment != null);
            Contract.Requires (arg != null);
            Contract.Requires (switchArg != null);
            Contract.Requires (valueIndex < 0 || (valueIndex + 1) <= arg.Length);

            bool switchValue;
            if (valueIndex < 0)
                switchValue = true;
            else
            {
                string settingValueStr = arg.Substring(valueIndex + 1);

                if (!bool.TryParse(settingValueStr, out switchValue))
                {
                    consoleEnvironment.ErrWriter.WriteLine(
                        @"Switch '{0}' has an invalid value ('{1}') - it should either be empty, 'true' or 'false'",
                        switchArg.ArgName,
                        settingValueStr);
                    return 2;
                }
            }

            CommandLineParsingContext context = new CommandLineParsingContext(consoleEnvironment);
            context.ArgName = switchArg.ArgName;
            switchArg.BoolValueAction(switchValue, context);

            return null;
        }

        private readonly List<ArgInfo> positionalArgs = new List<ArgInfo>();
        private readonly Dictionary<string, ArgInfo> settingsArgs = new Dictionary<string, ArgInfo>();
        private readonly Dictionary<string, ArgInfo> aliases = new Dictionary<string, ArgInfo>();

        public class PositionalArgBuilder
        {
            public PositionalArgBuilder(StandardConsoleCommandBase command, string argName, string description)
            {
                Contract.Requires(command != null);
                Contract.Requires(argName != null);
                argInfo = new ArgInfo(argName, description);
                argInfo.IsRequired = true;
                command.positionalArgs.Add(argInfo);
            }

            public PositionalArgBuilder IsOptional ()
            {
                Contract.Ensures(Contract.Result<PositionalArgBuilder>() != null);
                argInfo.IsRequired = false;
                return this;
            }

            public PositionalArgBuilder IntValue (Action<int, CommandLineParsingContext> valueAction)
            {
                Contract.Requires(valueAction != null);
                Contract.Ensures(Contract.Result<PositionalArgBuilder>() != null);
                argInfo.ArgType = typeof(int);
                argInfo.IntValueAction = valueAction;
                return this;
            }

            public PositionalArgBuilder Value (Action<string, CommandLineParsingContext> valueAction)
            {
                Contract.Requires(valueAction != null);
                Contract.Ensures(Contract.Result<PositionalArgBuilder>() != null);
                argInfo.ArgType = typeof(string);
                argInfo.StringValueAction = valueAction;
                return this;
            }

            private readonly ArgInfo argInfo;
        }

        public class SettingArgBuilder
        {
            public SettingArgBuilder (StandardConsoleCommandBase command, string argName, string description)
            {
                Contract.Requires(command != null);
                Contract.Requires(argName != null);
                this.command = command;
                argInfo = new ArgInfo(argName, description);
                argInfo.IsSetting = true;
                argInfo.IsRequired = true;
                command.settingsArgs.Add(argInfo.ArgName.ToLowerInvariant(), argInfo);
            }

            public SettingArgBuilder Alias (string alias)
            {
                Contract.Requires(alias != null);
                Contract.Ensures(Contract.Result<SettingArgBuilder>() != null);
                command.aliases.Add(alias.ToLowerInvariant(), argInfo);
                argInfo.Alias = alias;
                return this;
            }

            public SettingArgBuilder IntValue (Action<int, CommandLineParsingContext> valueAction)
            {
                Contract.Requires(valueAction != null);
                Contract.Ensures(Contract.Result<SettingArgBuilder>() != null);
                argInfo.ArgType = typeof(int);
                argInfo.IntValueAction = valueAction;
                return this;
            }

            public SettingArgBuilder Value (Action<string, CommandLineParsingContext> valueAction)
            {
                Contract.Requires(valueAction != null);
                Contract.Ensures(Contract.Result<SettingArgBuilder>() != null);
                argInfo.ArgType = typeof(string);
                argInfo.StringValueAction = valueAction;
                return this;
            }

            private readonly ArgInfo argInfo;
            private readonly StandardConsoleCommandBase command;
        }

        public class SwitchArgBuilder
        {
            public SwitchArgBuilder (
                StandardConsoleCommandBase command, string argName, string description, Action<bool, CommandLineParsingContext> valueAction)
            {
                Contract.Requires(command != null);
                Contract.Requires(argName != null);
                Contract.Requires(valueAction != null);
                this.command = command;
                argInfo = new ArgInfo(argName, description);
                argInfo.IsSetting = true;
                argInfo.IsRequired = true;
                argInfo.ArgType = typeof(bool);
                argInfo.BoolValueAction = valueAction;
                command.settingsArgs.Add(argInfo.ArgName.ToLowerInvariant(), argInfo);
            }

            public SwitchArgBuilder Alias (string alias)
            {
                Contract.Requires(alias != null);
                Contract.Ensures(Contract.Result<SwitchArgBuilder>() != null);
                command.aliases.Add(alias.ToLowerInvariant(), argInfo);
                argInfo.Alias = alias;
                return this;
            }

            private readonly ArgInfo argInfo;
            private readonly StandardConsoleCommandBase command;
        }

        public class CommandLineParsingContext : IConsoleEnvironment
        {
            public CommandLineParsingContext(IConsoleEnvironment env)
            {
                Contract.Requires(env != null);
                this.env = env;
            }

            public TextWriter OutWriter
            {
                get { return env.OutWriter; }
            }

            public TextWriter ErrWriter
            {
                get { return env.ErrWriter; }
            }

            public string ArgName { get; set; }

            private readonly IConsoleEnvironment env;
        }

        private class ArgInfo
        {
            public ArgInfo (string argName, string description)
            {
                Contract.Requires(argName != null);
                this.argName = argName;
                this.description = description;
            }

            public string ArgName
            {
                get { return argName; }
            }

            public string Alias { get; set; }

            public string Description
            {
                get { return description; }
            }

            public Type ArgType { get; set; }
            
            public bool IsSetting { get; set; }

            public bool IsRequired
            {
                get { return isRequired; }
                set { isRequired = value; }
            }

            public bool IsSwitch
            {
                get { return ArgType == typeof(bool); }
            }

            public Action<bool, CommandLineParsingContext> BoolValueAction { get; set; }
            public Action<int, CommandLineParsingContext> IntValueAction { get; set; }
            public Action<string, CommandLineParsingContext> StringValueAction { get; set; }

            private readonly string argName;
            private readonly string description;
            private bool isRequired;
        }
    }
}