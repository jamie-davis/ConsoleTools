using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.GlobalOptionAcceptanceTestCommands;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [UseReporter(typeof (CustomReporter))]
    public class GlobalOptionAcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;
        public GlobalOptionAcceptanceTests()
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
            config.LoadGlobalOptions(typeof(GlobalOptions));
            config.Load(typeof (ClientAddCommand));
            config.Load(typeof (ClientDelCommand));
            GlobalOptions.Environment = null;
        }

        [Fact]
        public void ConfigurationShouldBeDescribed()
        {
            CommandConfigDescriber.Describe(_posix, _console, "POSIX", CommandLineParserConventions.PosixConventions, CommandExecutionMode.CommandLine);
            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Fact]
        public void InteractiveModeConfigurationShouldBeDescribed()
        {
            CommandConfigDescriber.Describe(_posix, _console, "POSIX", CommandLineParserConventions.PosixConventions, CommandExecutionMode.Interactive);
            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Fact]
        public void ClientKeywordShouldBeDescribed()
        {
            _console.WrapLine($"Description of client:");
            CommandConfigDescriber.DescribeKeywords("client", _msStd, _console);
            _console.WriteLine();

            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Fact]
        public void PosixStyle()
        {
            var commands = new[]
            {
                @"client add bill",
                @"client del bill",
                @"client del bill -e",
                @"client add bill -elive",
                @"client del bill --env test",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50, false, () => $"GlobalOptions.Environment = {GlobalOptions.Environment}"));
        }

        [Fact]
        public void MsDosStyleCommand1()
        {
            var commands = new[]
            {
                @"client add bill",
                @"client del bill",
                @"client add bill /e:live",
                @"client del bill /env:test",
                @"client del bill /env staging",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50, false, () => $"GlobalOptions.Environment = {GlobalOptions.Environment}"));
        }

        [Fact]
        public void MsStdStyleCommand1()
        {
            var commands = new[]
            {
                @"client add bill",
                @"client del bill",
                @"client add bill -e live",
                @"client del bill -env test",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50, false, () => $"GlobalOptions.Environment = {GlobalOptions.Environment}"));
        }

        [Fact]
        public void PosixStyleInteractive()
        {
            var commands = new[]
            {
                @"client add bill",
                @"client del bill",
                @"client del bill -e",
                @"client add bill -elive",
                @"client del bill --env test",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50, true, () => $"GlobalOptions.Environment = {GlobalOptions.Environment}"));
        }

        [Fact]
        public void MsDosStyleCommand1Interactive()
        {
            var commands = new[]
            {
                @"client add bill",
                @"client del bill",
                @"client add bill /e:live",
                @"client del bill /env:test",
                @"client del bill /env staging",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50, true, () => $"GlobalOptions.Environment = {GlobalOptions.Environment}"));
        }

        [Fact]
        public void MsStdStyleCommand1Interactive()
        {
            var commands = new[]
            {
                @"client add bill",
                @"client del bill",
                @"client add bill -e live",
                @"client del bill -env test",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50, true, () => $"GlobalOptions.Environment = {GlobalOptions.Environment}"));
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local

    }
}