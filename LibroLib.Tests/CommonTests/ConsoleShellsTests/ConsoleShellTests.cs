using System.IO;
using System.Text;
using LibroLib.ConsoleShells;
using Moq;
using NUnit.Framework;

namespace LibroLib.Tests.CommonTests.ConsoleShellsTests
{
    [TestFixture]
    public class ConsoleShellTests
    {
        [Test]
        public void ParseEmptyCommandLine()
        {
            string[] args = { };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.AreEqual(0, result.ExitCode);
            Assert.AreEqual(0, result.CommandsToExecute.Count);

            AssertOutputIs(@"Unit test console

USAGE: unittest.exe <command> {<parameters>}

COMMANDS:
   cmd1 - performs action 1
   something to be used
   bla bla
   help - displays the command line help
   test - dummy command used for unit testing
      USAGE:
      test <arg1> [<Arg2>] [settings]

      PARAMETERS:
      arg1: a string argument
      Arg2: an int argument
      -setting1=value: an int setting (alias: s1)
      -Setting2=value: a string setting
      -switch1[=true/false]: a switch (alias: sw1)

");
            AssertErrOutputIs(string.Empty);
        }

        [Test]
        public void UnrecognizedCommand()
        {
            string[] args = { "cmd2" };
            ConsoleShellResult result = shell.ParseCommandLine(args);

            Assert.AreEqual(1, result.ExitCode);

            AssertOutputIs(@"Unit test console
");
            AssertErrOutputIs(@"Unknown command 'cmd2'. Type 'unittest.exe help' for the list of all commands.
");
        }

        [Test]
        public void ParseConsoleCommandSuccessfully()
        {
            string[] args = { "cmd1", "positional1", "-switch=true" };
            ConsoleShellResult result = shell.ParseCommandLine(args);

            Assert.IsNull(result.ExitCode);
            Assert.AreEqual(1, result.CommandsToExecute.Count);
            Assert.AreEqual("cmd1", result.CommandsToExecute[0].CommandId);
        }

        [Test]
        public void ParseStandardCommandSuccessfully()
        {
            string[] args = { "test", "value1", "123", "-setting1=345", "-setting2=haha" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.IsNull(result.ExitCode);
            TestStandardConsoleCommand parsedCmd = (TestStandardConsoleCommand)result.CommandsToExecute[0];
            Assert.AreEqual("value1", parsedCmd.Arg1);
            Assert.AreEqual(123, parsedCmd.Arg2);
            Assert.AreEqual(345, parsedCmd.IntSetting);
            Assert.AreEqual("haha", parsedCmd.StringSetting);
        }

        [Test]
        public void OptionalPositionalArgumentNotSpecified()
        {
            string[] args = { "test", "xyz", "-setting1=123" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.IsNull(result.ExitCode);
            TestStandardConsoleCommand parsedCmd = (TestStandardConsoleCommand)result.CommandsToExecute[0];
            Assert.AreEqual("xyz", parsedCmd.Arg1);
            Assert.AreEqual(0, parsedCmd.Arg2);
            Assert.AreEqual(123, parsedCmd.IntSetting);
        }

        [Test]
        public void UseSettingAlias()
        {
            string[] args = { "test", "xyz", "-s1=123" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.IsNull(result.ExitCode);
            TestStandardConsoleCommand parsedCmd = (TestStandardConsoleCommand)result.CommandsToExecute[0];
            Assert.AreEqual("xyz", parsedCmd.Arg1);
            Assert.AreEqual(0, parsedCmd.Arg2);
            Assert.AreEqual(123, parsedCmd.IntSetting);
        }

        [Test]
        public void SwitchArgWithoutValue()
        {
            string[] args = { "test", "value1", "-switch1" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.IsNull(result.ExitCode);
            TestStandardConsoleCommand parsedCmd = (TestStandardConsoleCommand)result.CommandsToExecute[0];
            Assert.IsTrue(parsedCmd.Switch1);
        }

        [Test]
        public void SwitchArgWithValue()
        {
            string[] args = { "test", "value1", "-switch1=true" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.IsNull(result.ExitCode);
            TestStandardConsoleCommand parsedCmd = (TestStandardConsoleCommand)result.CommandsToExecute[0];
            Assert.IsTrue(parsedCmd.Switch1);
        }

        [Test] 
        public void PositionalArgumentValueIsInvalid()
        {
            string[] args = { "test", "value1", "xyz", "-setting1=true" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.AreEqual(2, result.ExitCode);
            AssertErrOutputIs(@"Argument 'Arg2' has an invalid value ('xyz') - it should be an integer
");
        }

        [Test]
        public void UnknownSetting()
        {
            string[] args = { "test", "value1", "-dont=true" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.AreEqual(2, result.ExitCode);
            AssertErrOutputIs(@"Unknown setting 'dont' specified
");
        }

        [Test] 
        public void SettingArgumentValueIsInvalid()
        {
            string[] args = { "test", "value1", "-setting1=xyz" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.AreEqual(2, result.ExitCode);
            AssertErrOutputIs(@"Setting 'setting1' has an invalid value ('xyz') - it should be an integer
");
        }

        [Test] 
        public void SettingArgumentValueIsMissing()
        {
            string[] args = { "test", "value1", "-setting1" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.AreEqual(2, result.ExitCode);
            AssertErrOutputIs(@"Setting 'setting1' is missing the value
");
        }

        [Test] 
        public void SwitchArgumentValueIsInvalid()
        {
            string[] args = { "test", "value1", "-switch1=xyz" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.AreEqual(2, result.ExitCode);
            AssertErrOutputIs(@"Switch 'switch1' has an invalid value ('xyz') - it should either be empty, 'true' or 'false'
");
        }

        [Test] 
        public void RequiredArgIsMissing()
        {
            string[] args = { "test", "-setting1=123" };
            ConsoleShellResult result = shell.ParseCommandLine(args);
            Assert.AreEqual(2, result.ExitCode);
            AssertErrOutputIs(@"Required argument arg1 is missing
");
        }

        [SetUp]
        public void Setup()
        {
            outStream = new MemoryStream();
            errStream = new MemoryStream();

            shell = new ConsoleShell("unittest.exe");
            shell.OutWriter = new StreamWriter(outStream, new UTF8Encoding(false));
            shell.ErrWriter = new StreamWriter(errStream, new UTF8Encoding(false));
            shell.Banner = "Unit test console";

            Mock<IConsoleCommand> mockCommand = new Mock<IConsoleCommand>();
            mockCommand.Setup(x => x.CommandId).Returns("cmd1");
            mockCommand.Setup(x => x.Description).Returns("performs action 1");
            mockCommand.Setup(x => x.ConstructUsageHelpText(It.IsAny<string>()))
                .Returns(@"   something to be used
   bla bla");

            shell.RegisterCommand(mockCommand.Object);

            TestStandardConsoleCommand cmd2 = new TestStandardConsoleCommand();
            shell.RegisterCommand(cmd2);
        }

        private void AssertOutputIs(string expectedOutput)
        {
            shell.OutWriter.Flush();
            string actualOutput = new UTF8Encoding(false).GetString(outStream.ToArray());
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        private void AssertErrOutputIs(string expectedErrOutput)
        {
            shell.ErrWriter.Flush();
            string actualErrOutput = new UTF8Encoding(false).GetString(errStream.ToArray());
            Assert.AreEqual(expectedErrOutput, actualErrOutput);
        }

        private ConsoleShell shell;
        private MemoryStream outStream;
        private MemoryStream errStream;
    }
}