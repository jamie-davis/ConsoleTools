using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestCommandAnalyser
    {
        private ConsoleAdapter _console;
        private ConsoleInterfaceForTesting _consoleOutInterface;

        public class TestCommand
        {
            public string StringProp { get; set; }
            public bool BoolProp { get; set; }
            public int IntProp { get; set; }
            public bool Override { get; set; }
        }

        public static class GlobalOptions
        {
            public static string Global { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _consoleOutInterface = new ConsoleInterfaceForTesting();
            _console = new ConsoleAdapter(_consoleOutInterface);
        }

        private void DescribeAnalysedOptions(BaseCommandConfig command, CommandLineInterpreterConfiguration config, IEnumerable<OptionsAnalyser.AnalysedOption> allOptions)
        {
            _console.WrapLine("All command options:");
            var commandDetails = command.Options
                .Select(o => new { o.Name, Aliases = string.Join(", ", o.Aliases) });
            _console.FormatTable(commandDetails);

            _console.WriteLine();
            _console.WriteLine();

            _console.WrapLine("All global options:");
            var globalOptions = config.GlobalOptions.SelectMany(g => g.Options).ToList();
            var globalOptionDetails = globalOptions
                .Select(o => new { o.Name, Aliases = string.Join(", ", o.Aliases)});
            _console.FormatTable(globalOptionDetails);

            _console.WriteLine();
            _console.WriteLine();

            _console.WrapLine("Analysed options:");
            var analysedOptions = allOptions
                .Select(a => new {OptionName = a.BaseOption.Name, ValidNames = string.Join(", ", a.ValidNames), Source = globalOptions.Contains(a.BaseOption) ? "Global" : "Command"});
            _console.FormatTable(analysedOptions);
        }

        [Test]
        public void GlobalOptionsAreCombinedWithCommandOptions()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.GlobalOption(typeof(GlobalOptions))
                .Option<string>("global", s => { GlobalOptions.Global = s; })
                .Alias("g");
            config
                .Command("x", t => new TestCommand())
                .Option("one", command => command.IntProp)
                .Alias("o")
                .Option("two", command => command.IntProp)
                .Alias("t");

            var testCommand = config.Commands.First();
            var analyser = new OptionsAnalyser(testCommand, config);
            _console.WrapLine("There are no clashes between the global options and the command options, so all should be valid in the command.");
            _console.WriteLine();
            DescribeAnalysedOptions(testCommand, config, analyser.AllOptions(CommandExecutionMode.CommandLine));
            Approvals.Verify(_consoleOutInterface.GetBuffer());
        }

        [Test]
        public void GlobalOptionsAreNotIncludedForInteractiveAnalyses()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.GlobalOption(typeof(GlobalOptions))
                .Option<string>("global", s => { GlobalOptions.Global = s; })
                .Alias("g");
            config
                .Command("x", t => new TestCommand())
                .Option("one", command => command.IntProp)
                .Alias("o")
                .Option("two", command => command.IntProp)
                .Alias("t");

            var testCommand = config.Commands.First();
            var analyser = new OptionsAnalyser(testCommand, config);
            _console.WrapLine("This is an interactive analysis, so the global option should not be considered at all.");
            _console.WriteLine();
            DescribeAnalysedOptions(testCommand, config, analyser.AllOptions(CommandExecutionMode.Interactive));
            Approvals.Verify(_consoleOutInterface.GetBuffer());
        }

        [Test]
        public void ClashesWithGlobalOptionNamesComeOutInCommandFavour()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.GlobalOption(typeof(GlobalOptions))
                .Option<string>("one", s => { GlobalOptions.Global = s; })
                .Alias("g");
            config
                .Command("x", t => new TestCommand())
                .Option("one", command => command.IntProp)
                .Alias("o")
                .Option("two", command => command.IntProp)
                .Alias("t");

            var testCommand = config.Commands.First();
            var analyser = new OptionsAnalyser(testCommand, config);
            _console.WrapLine("There is a clash between the global option name and one of the command option names. The global option name should not be a valid name for the global option.");
            _console.WriteLine();
            DescribeAnalysedOptions(testCommand, config, analyser.AllOptions(CommandExecutionMode.CommandLine));
            Approvals.Verify(_consoleOutInterface.GetBuffer());
        }

        [Test]
        public void ClashesWithGlobalOptionAliasesComeOutInCommandFavour()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.GlobalOption(typeof(GlobalOptions))
                .Option<string>("global", s => { GlobalOptions.Global = s; })
                .Alias("o");
            config
                .Command("x", t => new TestCommand())
                .Option("one", command => command.IntProp)
                .Alias("o")
                .Option("two", command => command.IntProp)
                .Alias("t");

            var testCommand = config.Commands.First();
            var analyser = new OptionsAnalyser(testCommand, config);
            _console.WrapLine("There is a clash between the global option alias and one of the command option aliases. The global option alias should not be a valid name for the global option.");
            _console.WriteLine();
            DescribeAnalysedOptions(testCommand, config, analyser.AllOptions(CommandExecutionMode.CommandLine));
            Approvals.Verify(_consoleOutInterface.GetBuffer());
        }

        [Test]
        public void GlobalOptionExcludedIfItHasNoUniqueNames()
        {
            var config = new CommandLineInterpreterConfiguration();
            config.GlobalOption(typeof(GlobalOptions))
                .Option<string>("o", s => { GlobalOptions.Global = s; })
                .Alias("one");
            config
                .Command("x", t => new TestCommand())
                .Option("one", command => command.IntProp)
                .Alias("o")
                .Option("two", command => command.IntProp)
                .Alias("t");

            var testCommand = config.Commands.First();
            var analyser = new OptionsAnalyser(testCommand, config);
            _console.WrapLine("There is a clash between the all of the global option names and aliased. The global option should not be selected for the command at all.");
            _console.WriteLine();
            DescribeAnalysedOptions(testCommand, config, analyser.AllOptions(CommandExecutionMode.CommandLine));
            Approvals.Verify(_consoleOutInterface.GetBuffer());
        }
    }
}