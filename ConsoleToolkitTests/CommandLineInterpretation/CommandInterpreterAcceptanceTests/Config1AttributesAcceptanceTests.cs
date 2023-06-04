using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [UseReporter(typeof (CustomReporter))]
    public class Config1AttributesAcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleOutInterface;
        private ConsoleAdapter _console;

        [Command("c1")]
        [Description("Command 1 a file.")]
        class C1Data
        {
            [Positional("filename")]
            [Description("The name of the file.")]
            public string FileName { get; set; }

            [Option("delete", "D")]
            [Description("Delete the file after processing.")]
            public bool DeleteAfter { get; set; }

            [Option("archive", "A")]
            [Description("Archive after processing")]
            public string ArchiveLocation { get; set; }
        }

        [Command("c2")]
        [Description("Command 2 an archive")]
        class C2Data
        {
            [Positional("keep", 1)]
            [Description("The number of days to keep the archive")]
            public int DaysToKeep { get; set; }

            [Positional("name")]
            [Description("The name of the archive.")]
            public string ArchiveName { get; set; }

            [Option("maxSize", "M")]
            [Description("The maximum size of the archive.")]
            public int MaxSize { get; set; }
        }

        [Command("c3")]
        [Description("Generate loads of spam")]
        class C3Data
        {
            [Positional("iterations")]
            [Description("Number of times to repeat")]
            public int Iterations { get; set; }
            
            [Positional(1)]
            [Description("The message to spam.")]
            public string Message { get; set; }
            
            [Positional(2, DefaultValue = "5")]
            [Description("Amount packet should be longer than it claims")]
            public int OverrunLength { get; set; }

            [Option("kidding", "K")]
            [Description("Run in just kidding mode.")]
            public bool Kidding { get; set; }
        }
        public Config1AttributesAcceptanceTests()
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
            config.Load(typeof (C1Data));
            config.Load(typeof (C2Data));
            config.Load(typeof (C3Data));
        }

        [Fact]
        public void ConfigurationShouldBeDescribed()
        {
            CommandConfigDescriber.Describe(_posix, _console, "POSIX", CommandLineParserConventions.PosixConventions, CommandExecutionMode.CommandLine);
            var description = _consoleOutInterface.GetBuffer();
            Approvals.Verify(description);
        }

        [Fact]
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
                @"bogus"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_posix, commands, 50, false));
        }

        [Fact]
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
                @"c1 file /A:b,56",
                @"c2 name 5 /M:5",
                @"c2 name 5 /M:5,"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msDos, commands, 50, false));
        }

        [Fact]
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
                @"c1 file -A:b,56",
                @"c1 file -A b,56",
                @"c1 -- -A",
                @"c2 name 4 -maxSize:5",
                @"c3",
                @"c3 forty text 100",
                @"c3 40 text 100",
                @"c3 40 text 100 -kidding"
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50, false));
        }
    }
}