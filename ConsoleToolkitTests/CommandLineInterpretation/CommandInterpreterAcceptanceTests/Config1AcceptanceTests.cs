using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class Config1AcceptanceTests
    {
        private CommandLineInterpreterConfiguration _posix;
        private CommandLineInterpreterConfiguration _msDos;
        private CommandLineInterpreterConfiguration _msStd;

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
                    .Description("Amount packet should be longer than it claims")
                .Option("kidding")
                    .Alias("K")
                    .Description("Run in just kidding mode.");
        }

        [Test]
        public void ConfigurationShouldBeDescribed()
        {
            Approvals.Verify(_posix.Describe(50));
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
                @"c2 name --maxSize=5",
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

    public static class CommandExecutorUtil
    {
        public static string Do(CommandLineInterpreterConfiguration config, string[] commands, int width)
        {
            var sb = new StringBuilder();
            var interpreter = new CommandLineInterpreter(config);
            foreach (var command in commands)
            {
                sb.AppendLine(string.Format("Test: {0}", command));
                sb.AppendLine();

                var args = CommandLineTokeniser.Tokenise(command);
                string[] errors;
                var result = interpreter.Interpret(args, out errors);
                if (errors.Any())
                {
                    foreach (var e in errors)
                    {
                        sb.AppendLine(e);
                    }
                }
                else
                {
                    sb.AppendLine(result.GetType().Name);
                    sb.AppendLine("{");
                    foreach (var propertyInfo in result.GetType().GetProperties())
                    {
                        sb.AppendLine(string.Format("    {0} = {1}", propertyInfo.Name, propertyInfo.GetValue(result)));
                    }
                    sb.AppendLine("}");
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
