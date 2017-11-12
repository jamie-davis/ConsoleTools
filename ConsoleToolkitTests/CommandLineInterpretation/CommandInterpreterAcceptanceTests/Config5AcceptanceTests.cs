using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands;
using ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.GlobalOptionAcceptanceTestCommands;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class Config5AcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;

        [SetUp]
        public void SetUp()
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
            config.Load(typeof (AccountClearCommand));
            config.Load(typeof (AccountListCommand));
            config.Load(typeof (AccountShowCommand));
            config.Load(typeof (MonitorAlertCommand));
            config.Load(typeof (MonitorLogProfilesCommand));
            config.Load(typeof (StorageBlobCommand));
            config.Load(typeof (StorageQueueCommand));
            config.Load(typeof (StorageTableCommand));
        }

        [Test]
        public void ConfigurationShouldBeDescribed()
        {
            CommandConfigDescriber.Describe(_posix, _console, "POSIX", CommandLineParserConventions.PosixConventions);
            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Test]
        public void ConfigurationKeywordsShouldBeDescribed()
        {
            var keywords = new [] {"account", "monitor", "storage"};
            foreach (var keyword in keywords)
            {
                _console.WrapLine($"Description of {keyword}:");
                CommandConfigDescriber.DescribeKeywords(keyword, _msStd, _console);
                _console.WriteLine();
            }
            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Test]
        public void PosixStyle()
        {
            var commands = new[]
            {
                @"account",
                @"account clear",
                @"account not",
                @"account list",
                @"account show unexpected",
                @"monitor",
                @"monitor log-profiles",
                @"monitor alert",
                @"storage",
                @"storage blob",
                @"storage queue",
                @"storage table"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50));
        }

        [Test]
        public void MsDosStyleCommand1()
        {
            var commands = new[]
            {
                @"account",
                @"account clear",
                @"account not",
                @"account list",
                @"account show unexpected",
                @"monitor",
                @"monitor log-profiles",
                @"monitor alert",
                @"storage",
                @"storage blob",
                @"storage queue",
                @"storage table"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50));
        }

        [Test]
        public void MsStdStyleCommand1()
        {
            var commands = new[]
            {
                @"account",
                @"account clear",
                @"account not",
                @"account list",
                @"account show unexpected",
                @"monitor",
                @"monitor log-profiles",
                @"monitor alert",
                @"storage",
                @"storage blob",
                @"storage queue",
                @"storage table"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50));
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local

    }
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class GlobalOptionAcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;

        [SetUp]
        public void SetUp()
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
            config.Load(typeof (SessionCommand));
            config.Load(typeof (ClientAddCommand));
            config.Load(typeof (ClientDelCommand));
        }

        [Test]
        public void ConfigurationShouldBeDescribed()
        {
            CommandConfigDescriber.Describe(_posix, _console, "POSIX", CommandLineParserConventions.PosixConventions);
            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Test]
        public void ClientKeywordShouldBeDescribed()
        {
            _console.WrapLine($"Description of client:");
            CommandConfigDescriber.DescribeKeywords("client", _msStd, _console);
            _console.WriteLine();

            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Test]
        public void PosixStyle()
        {
            var commands = new[]
            {
                @"client add bill",
                @"client del bill",
                @"client add bill ben",
                @"client del bill ben",
                @"session",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50));
        }

        [Test]
        public void MsDosStyleCommand1()
        {
            var commands = new[]
            {
                @"client add bill",
                @"client del bill",
                @"client add bill ben",
                @"client del bill ben",
                @"session",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50));
        }

        [Test]
        public void MsStdStyleCommand1()
        {
            var commands = new[]
            {
                @"client add bill",
                @"client del bill",
                @"client add bill ben",
                @"client del bill ben",
                @"session",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50));
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local

    }
}