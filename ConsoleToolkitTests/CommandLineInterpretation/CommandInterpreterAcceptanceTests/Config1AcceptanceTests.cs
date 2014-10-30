using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    // ReSharper disable UnusedAutoPropertyAccessor.Local
    // ReSharper disable UnusedMember.Local
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class Config1AcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;

        class C1Data
        {
            public C1Data(string name)
            {
                CommandName = name;
            }
            public string CommandName { get; private set; }
            public string FileName { get; set; }
            public bool DeleteAfter { get; set; }
            public string ArchiveLocation { get; set; }
        }

        class C2Data
        {
            public string CommandName { get; set; }
            public int DaysToKeep { get; set; }
            public string ArchiveName { get; set; }
            public int MaxSize { get; set; }
        }

        class C3Data
        {
            public string CommandName { get; set; }
            public int Iterations { get; set; }
            public string Message { get; set; }
            public int OverrunLength { get; set; }
            public bool Kidding { get; set; }
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

        }

        private void Configure(CommandLineInterpreterConfiguration config)
        {
            config.Command("c1", s => new C1Data(s))
                .Description("Command 1 a file.")
                .Positional<string>("filename", (c, s) => c.FileName = s)
                    .Description("The name of the file.")
                .Option("delete", (c, b) => c.DeleteAfter = b)
                    .Alias("D")
                    .Description("Delete the file after processing.")
                .Option<string>("archive", (c, s) => c.ArchiveLocation = s)
                    .Alias("A")
                    .Description("Archive after processing");

            config.Command<C2Data>("c2")
                .Description("Command 2 an archive")
                .Positional("name", c => c.ArchiveName)
                    .Description("The name of the archive.")
                .Positional("keep", c => c.DaysToKeep)
                    .Description("The number of days to keep the archive")
                .Option("maxSize", c => c.MaxSize)
                    .Alias("M")
                    .Description("The maximum size of the archive.");

            config.Command<C3Data>("c3")
                .Description("Generate loads of spam")
                .Positional("iterations")
                    .Description("Number of times to repeat")
                .Positional("Message")
                    .Description("The message to spam.")
                .Positional("OverrunLength")
                    .Description("Amount packet should be longer than it claims.")
                    .DefaultValue("5")
                .Option("kidding")
                    .Alias("K")
                    .Description("Run in just kidding mode.");
        }

        [Test]
        public void ConfigurationShouldBeDescribed()
        {
            CommandDescriber.Describe(_posix, _console,  "POSIX");

            var adorner = new PosixCommandLineParser();
            foreach (var baseCommandConfig in _posix.Commands)
            {
                _console.WriteLine();
                _console.WriteLine("Description of {0} command:", baseCommandConfig.Name);
                CommandDescriber.Describe(baseCommandConfig, _console, adorner);
            }

            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Test]
        public void PosixStyleCommand1()
        {
            var commands = new[]
            {
                @"c1 file",
                @"c1 file --delete -Alocation",
                @"c1 file -D --archive=location",
                @"c1",
                @"c1 -D -Aloc",
                @"c1 -A",
                @"c1 -Ab,56",
                @"c1 -- -Ab,56",
                @"c3 40 text 50",
                @"c3 40 text",
                @"bogus"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50));
        }

        [Test]
        public void MsDosStyleCommand1()
        {
            var commands = new[]
            {
                @"c1 file",
                @"c1 file /delete /A:location",
                @"c1 file /D /archive:location",
                @"c1",
                @"c1 /D /A:loc",
                @"c1 /A",
                @"c1 /A:b,56",
                @"c2 name 5 /M:5",
                @"c2 name 5 /M:5,",
                @"c3 40 text 50",
                @"c3 40 text"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50));
        }

        [Test]
        public void MsStdStyleCommand1()
        {
            var commands = new[]
            {
                @"c1 file",
                @"c1 file -delete -A:location",
                @"c1 file -delete -A location",
                @"c1 file -D -archive:location",
                @"c1 file -D -archive location",
                @"c1 file -D:false -A:loc",
                @"c1 file -D:true -A:loc",
                @"c1 file -delete:false -A:loc",
                @"c1 file -delete:true -A:loc",
                @"c1",
                @"c1 -D -A:loc",
                @"c1 -A",
                @"c1 -A:b,56",
                @"c1 -A b,56",
                @"c1 -- -A",
                @"c2 name 4 -maxSize:5",
                @"c3",
                @"c3 forty text 100",
                @"c3 40 text",
                @"c3 40 text 100",
                @"c3 40 text 100 -kidding"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50));
        }
    }
}
