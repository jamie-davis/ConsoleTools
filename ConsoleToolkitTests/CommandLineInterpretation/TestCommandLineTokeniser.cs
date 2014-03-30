using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.CommandLineInterpretation
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandLineTokeniser
    {
        [Test]
        public void SimpleArgsAreExtracted()
        {
            Approvals.Verify(Tokenise("simple args are space delimited"));
        }

        [Test]
        public void ArgsWithStringDelimiterAreExtracted()
        {
            Approvals.Verify(Tokenise("simple \"this one is delimited\""));
        }

        [Test]
        public void EscapedStringDelimitersArePartOfTheToken()
        {
            Approvals.Verify(Tokenise("simple \"containing \\\"embedded speech\\\"\""));
        }

        [Test]
        public void ForcedStringDelimitersArePartOfTheToken()
        {
            Approvals.Verify(Tokenise("simple \"containing \"\"embedded speech\"\"\""));
        }

        [Test]
        public void EmptyTokensAreExtracted()
        {
            Approvals.Verify(Tokenise("\"\""));
        }

        private string Tokenise(string commandLine)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Tokens extracted from:");
            sb.AppendFormat("-->{0}<--", commandLine);
            sb.AppendLine();
            sb.AppendLine();

            var args = CommandLineTokeniser.Tokenise(commandLine);

            var count = 0;
            foreach (var arg in args)
            {
                sb.AppendFormat("{0}: [{1}]", count++, arg);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
