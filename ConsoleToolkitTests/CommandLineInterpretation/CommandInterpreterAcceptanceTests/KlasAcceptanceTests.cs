using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.KlasAcceptanceTestCommands;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [UseReporter(typeof (CustomReporter))]
    public class KlasAcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;
        public KlasAcceptanceTests()
        {
            _posix = new CommandLineInterpreterConfiguration(CommandLineParserConventions.PosixConventions);
            _msDos = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MsDosConventions);
            _msStd = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MicrosoftStandard);
            Configure(_posix);
            Configure(_msDos);
            Configure(_msStd);

            _consoleOutInterface = new ConsoleInterfaceForTesting();
            _consoleOutInterface.WindowWidth = 133;
            _consoleOutInterface.BufferWidth = 133;
            _console = new ConsoleAdapter(_consoleOutInterface);
        }

        private void Configure(CommandLineInterpreterConfiguration config)
        {
            config.Load(typeof (AddLogCommand));
        }

        [Fact]
        public void ConfigurationShouldBeDescribed()
        {
            CommandConfigDescriber.Describe(_posix, _console, "POSIX", CommandLineParserConventions.PosixConventions, CommandExecutionMode.CommandLine);
            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Fact]
        public void PosixStyle()
        {
            var commands = new[]
            {
                @"log add 192.168.0.10 tags.csv 1000 :",
                @"log add 192.168.0.10 tags.csv 1000 ,",
                @"log add 192.168.0.10 tags.csv 1000 ",
                @"log add 192.168.0.10 tags.csv",
                @"log add 192.168.0.10",
                @"log add",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50, false));
        }

        [Fact]
        public void MsDosStyleCommand1()
        {
            var commands = new[]
            {
                @"log add 192.168.0.10 tags.csv 1000 :",
                @"log add 192.168.0.10 tags.csv 1000 ,",
                @"log add 192.168.0.10 tags.csv",
                @"log add 192.168.0.10",
                @"log add",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50, false));
        }

        [Fact]
        public void MsStdStyleCommand1()
        {
            var commands = new[]
            {
                @"log add 192.168.0.10 tags.csv 1000 -delim :",
                @"log add 192.168.0.10 tags.csv 1000 :",
                @"log add 192.168.0.10 tags.csv 1000 ,",
                @"log add 192.168.0.10 tags.csv 1000 -delim ,",
                @"log add 192.168.0.10 tags.csv",
                @"log add 192.168.0.10",
                @"log add",
                @"log add -delim ,",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50, false));
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local

    }
}