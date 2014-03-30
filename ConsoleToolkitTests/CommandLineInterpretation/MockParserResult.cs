using System.Linq;
using System.Text;
using ConsoleToolkit.CommandLineInterpretation;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    class MockParserResult : IParserResult
    {
        private StringBuilder _log = new StringBuilder();
        private int _countToHalt = -1;

        public string Log {get { return _log.ToString(); }}

        public ParseOutcome OptionExtracted(string optionName, string[] arguments)
        {
            var args = string.Empty;
            if (arguments.Length > 0)
                args = arguments.Select(a => string.Format((string) "\"{0}\"", (object) a)).Aggregate((t, i) => t + "," + i);
            _log.AppendLine(string.Format("Option(\"{0}\", [{1}])", optionName, args));
            return CheckCountdown();
        }

        public ParseOutcome PositionalArgument(string value)
        {
            _log.AppendLine(string.Format("Positional(\"{0}\")", value));
            return CheckCountdown();
        }

        private ParseOutcome CheckCountdown()
        {
            if (_countToHalt == -1) return ParseOutcome.Continue;

            if (_countToHalt == 0)
                return ParseOutcome.Halt;

            --_countToHalt;
            return ParseOutcome.Continue;
        }

        public void HaltAfter(int index)
        {
            _countToHalt = index;
        }
    }
}
