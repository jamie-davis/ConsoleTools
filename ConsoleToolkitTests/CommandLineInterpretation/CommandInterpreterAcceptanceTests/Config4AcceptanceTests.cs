using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [UseReporter(typeof (CustomReporter))]
    public class Config4AcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;
        public Config4AcceptanceTests()
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
            config.Load(typeof (ImportCommand));
            config.Load(typeof (ExportCommand));
            config.Load(typeof (HelpCommand));
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
                @"export file.dat 2014-08-22 2014-08-26 -s. --database test -uadmin -padm1n",
                @"export file.dat 2014-08-22 2014-08-26 -ffilter1 -ffilter2",
                @"import file.dat --server server2 --database test -uadmin -padm1n",
                @"import file.dat --server server2 --database test -uadmin -padm1n -ffilter1 -ffilter2",
                @"import file.dat"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50, false));
        }

        [Fact]
        public void MsDosStyleCommand1()
        {
            var commands = new[]
            {
                @"export file.dat 2014-08-22 2014-08-26 /s:. /database:test /u:admin /p:adm1n",
                @"export file.dat 2014-08-22 2014-08-26 /filter:filter1 /f:filter2",
                @"import file.dat /server:server2 /database:test /u:admin /p:adm1n /filter:filter1 /f:filter2",
                @"import file.dat"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50, false));
        }

        [Fact]
        public void MsStdStyleCommand1()
        { 
            var commands = new[]
            {
                @"export -to 2014-08-26 -from 2014-08-22 file.dat",
                @"export file.dat 2014-08-22 2014-08-26 -s . -database test -u admin -p adm1n",
                @"export file.dat 2014-08-22 2014-08-26 -filter filter1 -f filter2",
                @"import file.dat -server server2 -database test -u admin -p adm1n",
                @"import file.dat -server server2 -database test -u admin -p adm1n -filter filter1 -f filter2",
                @"import file.dat"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50, false));
        }

        [Fact]
        public void ValidationIsRunOnParameters()
        {
            var commands = new[]
            {
                @"export file.dat 2014-08-22 2014-08-26 -s 123456789ABC -database test -u admin -p adm1n",
                @"export file*.dat 2014-08-22 2014-08-26 -filter filter1 -f filter2",
                @"import file:.dat -server server2 -database test -u admin -p adm1n",
                @"import file*.dat -server server2 -database test -u admin -p adm1n -filter filter1 -f filter2"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50, false));
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local

    }
}
