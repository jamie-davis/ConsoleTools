using System;
using System.Collections.Generic;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;
using Description = ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes.DescriptionAttribute;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class Config4AcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;
        private static readonly string _applicationName = "AcceptanceTest";

        class DbOptions
        {
            [Option("dbname", "d")]
            [Description("The name of the database.")]
            public string DatabaseName { get; set; }

            [Option("server", "s")]
            [Description("The name of the database server.")]
            public string ServerName { get; set; }

            [Option("user", "u")]
            [Description("The database user id.")]
            public string UserId { get; set; }

            [Option("pwd", "p")]
            [Description("The database user's password.")]
            public string Password { get; set; }

            [Option("filter", "f")]
            [Description("Global data filter")]
            public List<string> Filters { get; set; }
        }

        [Command]
        [Description("Import data previously exported from a database.")]
        class ImportCommand
        {
            [Positional]
            [Description("The file to be imported.")]
            public string Filename { get; set; }

            [OptionSet]
            public DbOptions DbOptions { get; set; }

            [Option("unique-ids", "i")]
            [Description("Generate unique ids for the rows imported to the database.")]
            public bool UniqueIds { get; set; }
        }

        [Command]
        [Description("Export data from a database to a file.")]
        class ExportCommand
        {
            [Positional]
            [Description("The file to be created or overwritten.")]
            public string Filename { get; set; }

            [Positional]
            [Description("The lowest timestamp to be included in the exported data.")]
            public DateTime FromDate { get; set; }

            [Positional]
            [Description("The highest timestamp to be included in the exported data.")]
            public DateTime ToDate { get; set; }

            [OptionSet]
            public DbOptions DbOptions { get; set; }
        }

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
            _consoleOutInterface.WindowWidth = 60;
            _console = new ConsoleAdapter(_consoleOutInterface);
        }

        private void Configure(CommandLineInterpreterConfiguration config)
        {
            config.Load(typeof (ImportCommand));
            config.Load(typeof (ExportCommand));
        }

        [Test]
        public void ConfigurationShouldBeDescribed()
        {
            var interpreter = new CommandLineInterpreter(_posix);
            CommandDescriber.Describe(_posix, _console, _applicationName, interpreter.GetOptionNameAdorner());
            var description = _consoleOutInterface.GetBuffer();
            Console.WriteLine(description);
            Approvals.Verify(description);
        }

        [Test]
        public void PosixStyle()
        {
            var commands = new[]
            {
                @"export file.dat 2014-08-22 2014-08-26 -s. --dbname test -uadmin -padm1n",
                @"export file.dat 2014-08-22 2014-08-26 -ffilter1 -ffilter2",
                @"import file.dat --server server2 --dbname test -uadmin -padm1n",
                @"import file.dat --server server2 --dbname test -uadmin -padm1n -ffilter1 -ffilter2",
                @"import file.dat",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50));
        }

        [Test]
        public void MsDosStyleCommand1()
        {
            var commands = new[]
            {
                @"export file.dat 2014-08-22 2014-08-26 /s:. /dbname:test /u:admin /p:adm1n",
                @"export file.dat 2014-08-22 2014-08-26 /filter:filter1 /f:filter2",
                @"import file.dat /server:server2 /dbname:test /u:admin /p:adm1n /i /filter:filter1 /f:filter2",
                @"import file.dat",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50));
        }

        [Test]
        public void MsStdStyleCommand1()
        {
            var commands = new[]
            {
                @"export file.dat 2014-08-22 2014-08-26 -s . -dbname test -u admin -p adm1n",
                @"export file.dat 2014-08-22 2014-08-26 -filter filter1 -f filter2",
                @"import file.dat -server server2 -dbname test -u admin -p adm1n",
                @"import file.dat -server server2 -dbname test -u admin -p adm1n -filter filter1 -f filter2",
                @"import file.dat",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50));
        }

        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local

    }
}
