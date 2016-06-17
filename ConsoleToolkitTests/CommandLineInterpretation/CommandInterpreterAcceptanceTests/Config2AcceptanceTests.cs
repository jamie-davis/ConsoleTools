using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class Config2AcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;
        private static readonly string _applicationName = "AcceptanceTest";

        class Data
        {
            public string FileName { get; set; }
            public bool Delete { get; set; }
            public string Archive { get; set; }
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
            _console = new ConsoleAdapter(_consoleOutInterface);
            _console.WriteLine(RulerFormatter.MakeRuler(40));
        }

        private void Configure(CommandLineInterpreterConfiguration config)
        {
            config.Parameters<Data>()
                .Description("Do something to a file.")
                .Positional("filename")
                .Description("The name of the file.")
                .Option("delete")
                .Alias("D")
                .Description("Delete the file after processing.")
                .Option("archive")
                .Alias("A")
                .Description("Archive after processing");
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
        public void PosixStyleCommand1()
        {
            var commands = new[]
            {
                @"file",
                @"file --delete -Alocation",
                @"file -D --archive=location",
                @"",
                @"-D -Aloc",
                @"-A",
                @"-Ab,56",
                @"-- -Ab,56",
                @"file 4"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50));
        }

        [Test]
        public void MsDosStyleCommand1()
        {
            var commands = new[]
            {
                @"file",
                @"file /delete /A:location",
                @"file /D /archive:location",
                @"",
                @"/D /A:loc",
                @"/A",
                @"file /A:b,56",
                @"name /M:5",
                @"name /A:5,"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50));
        }

        [Test]
        public void MsStdStyleCommand1()
        {
            var commands = new[]
            {
                @"file",
                @"file -delete -A:location",
                @"file -delete -A location",
                @"file -D -archive:location",
                @"file -D -archive location",
                @"file -D:false -A:loc",
                @"file -D:true -A:loc",
                @"file -delete:false -A:loc",
                @"file -delete:true -A:loc",
                @"",
                @"-D -A:loc",
                @"-A",
                @"file -A:b,56",
                @"file -A b,56",
                @"-- -A",
                @"name 4 -maxSize:5"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50));
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedMember.Local

    }
}