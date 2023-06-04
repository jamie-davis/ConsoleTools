using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestColourInstructionRebuilder
    {
        [Fact]
        public void ForegroundSetterIsRebuilt()
        {
            var source = "red".Red();
            var split = WordSplitter.Split(source, 4);
            Approvals.Verify(Analyse(source, split));
        }

        [Fact]
        public void BackgroundSetterIsRebuilt()
        {
            var source = "red".BGRed();
            var split = WordSplitter.Split(source, 4);
            Approvals.Verify(Analyse(source, split));
        }

        [Fact]
        public void EmbeddedNewlineIsNotRebuilt()
        {
            var source = ("red" + Environment.NewLine + "still").BGRed();
            var split = WordSplitter.Split(source, 4);
            Approvals.Verify(Analyse(source, split));
        }

        [Fact]
        public void MultipleEmbeddedNewlinesAreNotRebuilt()
        {
            var source = ("red" + Environment.NewLine + "\r\n\r\n\r\nstill").BGRed();
            var split = WordSplitter.Split(source, 4);
            Approvals.Verify(Analyse(source, split));
        }

        private string Analyse(string source, SplitWord[] split)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Original:");
            sb.AppendLine(source);
            sb.AppendLine();
            sb.AppendLine("Split:");
            sb.AppendLine(DescribeWords(split));
            sb.AppendLine();
            sb.AppendLine("Reconstituted:");
            var reconstituted = Reconstitute(split);
            sb.AppendLine(reconstituted);

            var reSplit = WordSplitter.Split(reconstituted, 4);
            sb.AppendLine();
            sb.AppendLine("Re-split:");
            sb.AppendLine(DescribeWords(reSplit));

            return sb.ToString();
        }

        private string Reconstitute(SplitWord[] split)
        {
            var sb = new StringBuilder();

            foreach (var splitWord in split)
            {
                sb.Append(ColourInstructionRebuilder.Rebuild(AdapterConfiguration.PrefixAffinity,
                    splitWord.PrefixInstructions.ToList()));
                sb.Append(splitWord.WordValue);
                sb.Append(ColourInstructionRebuilder.Rebuild(AdapterConfiguration.SuffixAffinity,
                    splitWord.SuffixInstructions.ToList()));
            }

            return sb.ToString();
        }

        private static string DescribeWords(IEnumerable<SplitWord> words)
        {
            return string.Join(Environment.NewLine, words
                .Select(wo => string.Format("[{0},{1},T:{2}{3}]",
                    wo.Length, wo.TrailingSpaces,
                    wo.TerminatesLine() ? "yes" : "no",
                    FormatInstructions(wo))));
        }

        private static string FormatInstructions(SplitWord splitWord)
        {
            var output = string.Empty;
            if (splitWord.PrefixInstructions.Any())
                output += ">" + FormatInstructions(splitWord.PrefixInstructions);

            if (splitWord.SuffixInstructions.Any())
                output += "<" + FormatInstructions(splitWord.SuffixInstructions);

            return output;
        }

        private static string FormatInstructions(IEnumerable<ColourControlItem.ControlInstruction> instructions)
        {
            return string.Format("({0})", string.Join(",", instructions.Select(i => string.Format("{0} {1}", i.Code, i.Arg))));
        }

    }
}