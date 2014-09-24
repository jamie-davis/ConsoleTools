using System;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandDescriber
    {
        private CommandLineInterpreterConfiguration _config;
        private ConsoleAdapter _console;
        private ConsoleInterfaceForTesting _consoleOutInterface;

        public class TestCommand
        {
            public string StringProp { get; set; }
            public bool BoolProp { get; set; }
            public int IntProp { get; set; }
        }

        public class Adorner : IOptionNameHelpAdorner
        {
            public string Adorn(string name)
            {
                return "**" + name;
            }
        }

        [SetUp]
        public void SetUp()
        {
            _config = new CommandLineInterpreterConfiguration();
            _config
                .Command("first", s => new TestCommand())
                .Description("Description of the first commmand.");
            _config
                .Command("second", s => new TestCommand())
                .Description("The second command is a command with a number of parameters.")
                .Positional<string>("dateofthing", (command, s) => { })
                    .Description("The date the thing should have.")
                .Positional<string>("numberofthing", (command, s) => { })
                    .Description("The number of things that should be.");
            _config
                .Command("third", s => new TestCommand())
                .Description("The third command has a number of options but no parameters.")
                .Option("on", (command, b) => { })
                    .Description("A simple option with no argument.")
                .Option<string, int>("fiddly", (command, s, n) => { })
                    .Alias("f")
                    .Description("An option with two arguments. The arguments need to be described in the text.");
            _config
                .Command("fourth", s => new TestCommand())
                .Description("The fourth command is really complicated with a number of parameters and also options. This is the sort of command that needs lots of text.")
                .Positional<string>("date", (command, s) => { })
                    .Description("The date the complicated nonsense should be forgotten.")
                .Positional<string>("crpyticnum", (command, s) => { })
                    .Description("The amount of nonsense the user needs to forget.")
                .Option("ignore", (command, b) => { })
                    .Description("Use this option to consign this command to history, where it belongs.")
                .Option<string, int>("more", (command, s, n) => { })
                    .Description("Even more.");

            _config
                .Command("desc", s => new TestCommand())
                .Description(
                    @"Descriptions can contain embedded line breaks -->
<-- like that one. These should be respected in the formatting. (This is meant to look a bit odd. Also, you should be aware that the deliberate line break is the only one in this text.)")
                .Positional<string>("pos", (command, s) => { })
                    .Description(@"A parameter with
a line break.")
                .Option("lb", (command, b) => { })
                    .Description("Another\nbreak.");

            _config
                .Command("exp", s => new TestCommand())
                .Description(@"Command with a positional and options configured using a Linq Expression, not a lambda.")
                .Positional("pos", command => command.StringProp)
                    .Description(@"A positional configured with an expression.")
                .Option("B", command => command.BoolProp)
                    .Description("A boolean option configured with an expression.")
                .Option("I", command => command.IntProp)
                    .Description("A boolean option configured with an expression.");

            _consoleOutInterface = new ConsoleInterfaceForTesting();
            _console = new ConsoleAdapter(_consoleOutInterface);
        }

        [Test]
        public void DescriptionOfCommandsIsFormatted()
        {
            _console.WriteLine(RulerFormatter.MakeRuler(_console.WindowWidth));
            CommandDescriber.Describe(_config, _console, "", new Adorner());
            Approvals.Verify(_consoleOutInterface.GetBuffer() );
        }

        [Test]
        public void SelectedCommandDescriptionIsFormatted()
        {
            _console.WriteLine(RulerFormatter.MakeRuler(_console.WindowWidth));
            CommandDescriber.Describe(_config.Commands.First(c => c.Name == "exp"), _console, new Adorner());
            Approvals.Verify(_consoleOutInterface.GetBuffer());
        }

        [Test]
        public void DefaultCommandHelpIsFormatted()
        {
            var config = new CommandLineInterpreterConfiguration();
            config
                .Parameters(() => new TestCommand())
                .Description("Description of the whole program.")
                .Positional<string>("pos", (command, s) => { })
                .Description("A positional parameter.");

            config.DefaultCommand.Name = "shouldnotbedisplayed";

            _console.WriteLine(RulerFormatter.MakeRuler(_console.WindowWidth));
            CommandDescriber.Describe(config, _console, "Test");
            var buffer = _consoleOutInterface.GetBuffer();
            Console.WriteLine(buffer);
            Approvals.Verify(buffer);
        }
    }
}