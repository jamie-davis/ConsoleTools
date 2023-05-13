using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestWordSplitter
    {
        [Fact]
        public void WordsAreExractedWithTrailingSpaceCounts()
        {
            const string testPhrase = "one two  three   none.";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.Equal(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]", result);
        }

        [Fact]
        public void SplitToListExtractsWordsWithTrailingSpaceCounts()
        {
            const string testPhrase = "one two  three   none.";
            var words = WordSplitter.SplitToList(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.Equal(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]", result);
        }

        [Fact]
        public void ColourControlItemsCanBeSplit()
        {
            var testPhrase = "one ".White() + "two  ".Red() + "three   none.";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.Equal(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]", result);
        }

        [Fact]
        public void ColourControlItemsCanBeSplitToList()
        {
            var testPhrase = "one ".White() + "two  ".Red() + "three   none.";
            var words = WordSplitter.SplitToList(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.Equal(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]", result);
        }

        [Fact]
        public void TabsAreConvertedIntoSpaces()
        {
            const string testPhrase = "one\ttwo\t three \t eight.\t\t";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.Equal(@"[3,4,""one""][3,5,""two""][5,6,""three""][6,8,""eight.""]", result);
        }

        [Fact]
        public void NewlinesCreateTerminatesLineWord()
        {
            const string testPhrase = "one\ttwo\t \nthree \t eight.\t\t\r";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            const string expected = 
                     "[3,4,T:no]"                   // one
                   + "[3,5,T:no]"                   // two
                   + "[0,0,T:yes<(NewLine Black)]"  // \n
                   + "[5,6,T:no]"                   // three
                   + "[6,8,T:no]"                   // eight.
                   + "[0,0,T:yes<(NewLine Black)]"  // \r
                   ;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NewlinesAreStrippedFromWords()
        {
            const string testPhrase = "text\n\nmore";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            Assert.Equal(new[] { "text", string.Empty, string.Empty, "more" }, words.Select(w => w.WordValue).ToArray());
        }

        [Fact]
        public void NewlinesWithColourInformationAreDiscreetWords()
        {
            var testPhrase = "text" + "\n".Red() + "\n".BGRed() + "more";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            var expected =
                  "[4,0,T:no]"
                + "[0,0,T:yes>(Push Black,SetForeground Red)<(NewLine Black,Pop Black)]"
                + "[0,0,T:yes>(Push Black,SetBackground Red)<(NewLine Black,Pop Black)]" 
                + "[4,0,T:no]";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CrLfCountsAsOneLineEnd()
        {
            const string testPhrase = "one\r\ntwo\r\n\r\nthree\n\n\r\r";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            const string expected =
                  "[3,0,T:no]"                    // one
                + "[0,0,T:yes<(NewLine Black)]"   // \r\n
                + "[3,0,T:no]"                    // two
                + "[0,0,T:yes<(NewLine Black)]"   // \r\n
                + "[0,0,T:yes<(NewLine Black)]"   // \r\n
                + "[5,0,T:no]"                    // three
                + "[0,0,T:yes<(NewLine Black)]"   // \n
                + "[0,0,T:yes<(NewLine Black)]"   // \n
                + "[0,0,T:yes<(NewLine Black)]"   // \r
                + "[0,0,T:yes<(NewLine Black)]"   // \r
                ;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ColourControlCodesAreNotCountedInWordWidth()
        {
            var testPhrase = "one".Red() +" two".Green() + "\r\n\r\n" +"three".Blue() + "\n\n\r\r";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            const string expected =
                  "[3,0,T:no>(Push Black,SetForeground Red)<(Pop Black)]"   // one
                + "[0,1,T:no>(Push Black,SetForeground Green)]"             // blank 
                + "[3,0,T:no<(Pop Black)]"                                  // two
                + "[0,0,T:yes<(NewLine Black)]"                             // \r\n
                + "[0,0,T:yes<(NewLine Black)]"                             // \r\n
                + "[5,0,T:no>(Push Black,SetForeground Blue)<(Pop Black)]"  // three
                + "[0,0,T:yes<(NewLine Black)]"                             // \n
                + "[0,0,T:yes<(NewLine Black)]"                             // \n
                + "[0,0,T:yes<(NewLine Black)]"                             // \n
                + "[0,0,T:yes<(NewLine Black)]"                             // \n
                ;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SpacesWithColourInstructionsAreIndividualElements()
        {
            var testPhrase = " ".Red() +" ".Green() + " ".Blue();
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            const string expected =
                  "[0,1,T:no>(Push Black,SetForeground Red)<(Pop Black)]" 
                + "[0,1,T:no>(Push Black,SetForeground Green)<(Pop Black)]" 
                + "[0,1,T:no>(Push Black,SetForeground Blue)<(Pop Black)]";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TrailingSpacesWithinColourInstructionsAreSoftSpaces()
        {
            var testPhrase = "red ".Red() +"green ".Green() + "blue ".Blue();
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            const string expected =
                  "[3,1,T:no>(Push Black,SetForeground Red)<(Pop Black)]"   //red then blank 
                + "[5,1,T:no>(Push Black,SetForeground Green)<(Pop Black)]" //green then blank
                + "[4,1,T:no>(Push Black,SetForeground Blue)<(Pop Black)]"; //blue then blank
            Assert.Equal(expected, result);
        }

        [Fact]
        public void LeadingSpacesWithinColourInstructionsAreSoftSpaces()
        {
            var testPhrase = " red".Red() + " green".Green() + " blue".Blue();
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            const string expected =
                  "[0,1,T:no>(Push Black,SetForeground Red)]"     // blank
                + "[3,0,T:no<(Pop Black)]"                        // red
                + "[0,1,T:no>(Push Black,SetForeground Green)]"   // blank
                + "[5,0,T:no<(Pop Black)]"                        // green
                + "[0,1,T:no>(Push Black,SetForeground Blue)]"    // blank
                + "[4,0,T:no<(Pop Black)]";                       // blue
            Assert.Equal(expected, result);
        }

        [Fact]
        public void EmptyTextWithinColourControlItemsIsExtracted()
        {
            var testPhrase = " red".Red() + string.Empty.Green() + " blue".Blue();
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            const string expected =
                  "[0,1,T:no>(Push Black,SetForeground Red)]"               // blank
                + "[3,0,T:no<(Pop Black)]"                                  // red
                + "[0,0,T:no>(Push Black,SetForeground Green)<(Pop Black)]" // string.Empty
                + "[0,1,T:no>(Push Black,SetForeground Blue)]"              // blank
                + "[4,0,T:no<(Pop Black)]";                                 // blue
            Assert.Equal(expected, result);
        }

        private static string DescribeWords(IEnumerable<SplitWord> words)
        {
            return words
                .Select(wo => string.Format(@"[{0},{1},""{2}""]", wo.Length, wo.TrailingSpaces, wo.WordValue))
                .Aggregate((t, i) => t + i);
        }

        private static string DescribeWords2(IEnumerable<SplitWord> words)
        {
            return words
                .Select(wo => string.Format("[{0},{1},T:{2}{3}]", 
                    wo.Length, wo.TrailingSpaces, 
                    wo.TerminatesLine() ? "yes" : "no",
                    FormatInstructions(wo)))
                .Aggregate((t, i) => t + i);
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

        private static string FormatInstructions(IEnumerable<ColourControlItem.ControlInstruction>instructions)
        {
            return string.Format("({0})", string.Join(",", instructions.Select(i => string.Format("{0} {1}", i.Code, i.Arg))));
        }
    }
}