using System;
using System.Security.Cryptography;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;
using Description = ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes.DescriptionAttribute;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class Config1AcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleInterface;
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

            _consoleInterface = new ConsoleInterfaceForTesting();
            _console = new ConsoleAdapter(_consoleInterface);

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
            var description = _consoleInterface.GetBuffer();
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
                @"bogus",
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
                @"c3 40 text",
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
                @"c3 40 text 100 -kidding",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50));
        }
    }

    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class Config1AttributesAcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleInterface;
        private ConsoleAdapter _console;

        [Command("c1")]
        [Description("Command 1 a file.")]
        class C1Data
        {
            [Positional("filename", 0)]
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

            [Positional("name", 0)]
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
            [Positional("iterations", 0)]
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

        [SetUp]
        public void SetUp()
        {
            _posix = new CommandLineInterpreterConfiguration(CommandLineParserConventions.PosixConventions);
            _msDos = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MsDosConventions);
            _msStd = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MicrosoftStandard);
            Configure(_posix);
            Configure(_msDos);
            Configure(_msStd);

            _consoleInterface = new ConsoleInterfaceForTesting();
            _console = new ConsoleAdapter(_consoleInterface);
        }

        private void Configure(CommandLineInterpreterConfiguration config)
        {
            config.Load(typeof (C1Data));
            config.Load(typeof (C2Data));
            config.Load(typeof (C3Data));
        }

        [Test]
        public void ConfigurationShouldBeDescribed()
        {
            CommandDescriber.Describe(_posix, _console, "POSIX");
            var description = _consoleInterface.GetBuffer();
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
                @"bogus",
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
                @"c3 40 text 100",
                @"c3 40 text 100 -kidding",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50));
        }
    }
    
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class Config2AcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;
        private ConsoleInterfaceForTesting _consoleInterface;
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

            _consoleInterface = new ConsoleInterfaceForTesting();
            _console = new ConsoleAdapter(_consoleInterface);
            _console.WriteLine(RulerFormatter.MakeRuler(40));
        }

        private void Configure(CommandLineInterpreterConfiguration config)
        {
            config.Parameters<Data>("testApp")
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
            var description = _consoleInterface.GetBuffer();
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
                @"file 4",
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
                @"/A:b,56",
                @"name /M:5",
                @"name /A:5,",
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
                @"-A:b,56",
                @"-A b,56",
                @"-- -A",
                @"name 4 -maxSize:5",
            };

            Approvals.Verify(CommandExecutorUtil.Do(_msStd, commands, 50));
        }
    }
}
