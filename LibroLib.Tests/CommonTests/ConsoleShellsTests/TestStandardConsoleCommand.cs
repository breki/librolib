using System;
using LibroLib.ConsoleShells;

namespace LibroLib.Tests.CommonTests.ConsoleShellsTests
{
    public class TestStandardConsoleCommand : StandardConsoleCommandBase
    {
        public TestStandardConsoleCommand()
        {
            AddArg("arg1", "a string argument").Value((x, env) => arg1 = x);
            AddArg ("Arg2", "an int argument").IntValue ((x, env) => arg2 = x).IsOptional ();
            AddSetting ("setting1", "an int setting").Alias("s1").IntValue((x, env) => intSetting = x);
            AddSetting ("Setting2", "a string setting").Value((x, env) => stringSetting = x);
            AddSwitch("switch1", "a switch", (x, env) => Switch1 = x).Alias("sw1");
        }

        public bool Switch1 { get; set; }

        public override string CommandId
        {
            get { return "test"; }
        }

        public override object Description
        {
            get { return "dummy command used for unit testing"; }
        }

        public string Arg1
        {
            get { return arg1; }
        }

        public int Arg2
        {
            get { return arg2; }
        }

        public int IntSetting
        {
            get { return intSetting; }
        }

        public string StringSetting
        {
            get { return stringSetting; }
        }

        public override int Execute(IConsoleEnvironment env)
        {
            throw new InvalidOperationException("nothing to see here");
        }

        private string arg1;
        private int arg2;
        private int intSetting;
        private string stringSetting;
    }
}