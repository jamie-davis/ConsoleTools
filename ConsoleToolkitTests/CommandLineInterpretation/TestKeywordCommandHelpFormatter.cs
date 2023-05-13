using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [UseReporter(typeof(CustomReporter))]
    public class TestKeywordCommandHelpFormatter
    {
        private UnitTestConsole _output;
        private List<KeywordCommand> _commandSet;
        private IConsoleAdapter _console;

        public TestKeywordCommandHelpFormatter()
        {
            _output = new UnitTestConsole();
            _console = _output.Console;
            _commandSet = new List<KeywordCommand>
            {
                LoadTestCommand("config", "copy", "{config, Config related operations}"),
                LoadTestCommand("config", "delete", "{config, Config related operations}"),
                LoadTestCommand("file settings", "copy", "{file, File related operation}", "{file settings, File settings operations}"),
                LoadTestCommand("file settings", "delete", "{file settings}"),
                LoadTestCommand("file settings", "export", "{file settings}"),
                LoadTestCommand("archive settings", "copy", "{archive, Archive related operation}", "{archive settings, Archive settings operations}"),
                LoadTestCommand("archive settings", "delete", "{archive settings}"),
                LoadTestCommand("archive settings", "export", "{archive settings}"),
                LoadTestCommand("archive", "display", "{archive}"),
            };
        }

        private KeywordCommand LoadTestCommand(string keywords, string commandName, params string[] keywordSets)
        {
            return new KeywordCommand(keywords.Split(' ').ToList(), commandName, ExtractSets(keywordSets));
        }

        private List<KeywordsDesc> ExtractSets(string[] keywordSets)
        {
            var result = new List<KeywordsDesc>();
            foreach (var keywordSet in keywordSets)
            {
                var set = keywordSet;
                if (set[0] == '{')
                    set = set.Substring(1);

                if (set.EndsWith("}"))
                    set = set.Substring(0, set.Length - 1);

                var parts = set.Split(',').Select(s => s.Trim()).ToList();
                result.Add( new KeywordsDesc(parts.Count == 1 ? null : parts[1], parts[0].Split(' ').ToList()));
            }

            return result;
        }

        [Fact]
        public void CorrectKeywordsAreListed()
        {
            //Act
            var list = KeywordCommandHelpFormatter.FormatKeywordList(_commandSet);

            //Assert
            _console.FormatTable(list);
            Approvals.Verify(_output.Interface.GetBuffer());
        }
    }
}
