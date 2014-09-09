using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    /*
     * Notes about splitting text for wrapping:
     * 
     * Initially, the problem was splitting plain text. It was simple, because white space was simply a seperator for words,
     * and the blanks could therefore be assigned to the word that precedes them and they could later be dropped if the word
     * is last on the line.
     * 
     * However, introducing colour instructions adds a great deal of complexity.
     * 
     * For example:
     * 
     * "The dog   ate the cat."
     * 
     * Becomes:
     * 
     * The:1
     * dog:3
     * ate:1
     * the:1
     * cat.:0
     * 
     * (In this notation, the text is followed by a colon, and then the number of spaces that seperate the word from the next.)
     * 
     * Consider the same text with added colour:
     * 
     * "The " + "dog   ".BGBlue() + "ate".BGRed() + " the " + "cat".BGYellow() + "."
     * 
     * Here some of the text has a coloured background. "dog" has blue, "ate" has red and "cat" has yellow.
     * 
     * Consider "dog". The word is trailed by 3 blue background spaces. What does that actually mean
     * Essentially it means that if the spaces exist, they are blue.
     * 
     * Consider "ate" and " the ". If the space between "ate" and "the" exists, it is not coloured.
     * 
     * In the "dog" case, if we simply have:
     * 
     * (Colour instructions have been represented like XML for clarity, which is an approximation, not a reflection of reality.)
     * <blue>dog</blue>:3
     *
     * This would not result in blue spaces.
     * 
     * By contrast:
     * 
     * <red>ate</red>:1
     * 
     * Does exactly what we want. The space following "ate" is not coloured.
     * 
     * So, the problem is that the very specific arrangement of the colour instructions is crucial to rendering the text as
     * requested when it is wrapped.
     * 
     * There is more trouble here:
     * 
     * " ".BGRed() + " ".BGGreen() + " ".BGBlue()
     * 
     * This is presumably a request for three coloured blocks. The splitter would need to return:
     * 
     * <red> </red>:0
     * <green> </green>:0
     * <blue> </blue>:0
     * 
     * But spaces are interpreted as gaps between words, and are not "output" in the same way as other characters. Therefore
     * the splitter wants to attach them to words and treats them as optional.
     * 
     * Also:
     * 
     * "word".Red() + " more data".Blue()
     * 
     * If the line were to be broken after "word", the space preceding "more data" would be dropped. However, that space is
     * adjacent (read includes) the "blue foreground" formatting, and dropping that would mean "more data" is not rendered
     * in blue.
     * 
     * Therefore a very different approach is needed.
     * 
     * It would be better if words and whitespace were seperate. Whitespace must be able to carry formatting instructions.
     * When whitespace is omitted, the formatting instructions should not be. Whitespace runs are valid when they carry 
     * different formatting instructions. It would not be incorrect to split words on formatting instructions.
     * 
     * For example:
     *
     * "The dog   ate the cat."
     * 
     * Becomes:
     * 
     * The
     * <space:1>
     * dog
     * <space:3>
     * ate
     * <space:1>
     * the
     * <space:1>
     * cat.
     * 
     * "word".Red() + " more data".Blue()
     * 
     * Becomes:
     * <red>word</red>
     * <blue><space:1>
     * more
     * <space:1>
     * data</blue>
     * 
     * " ".BGRed() + " ".BGGreen() + " ".BGBlue()
     * 
     * <red><space:1></red>
     * <green><space:1></green>
     * <blue><space:1></blue>
     * 
     * "The " + "dog   ".BGBlue() + "ate".BGRed() + " the " + "cat".BGYellow() + "."
     * 
     * Becomes:
     * 
     * The
     * <space:1>
     * <blue>dog
     * <space:3></blue>
     * <red>ate</red>
     * <space:1>
     * the
     * <space:1>
     * <yellow>cat</yellow>.
     * 
     * The rule for the wrapping logic will be that whitespace that is to be skipped has the spaces removed, but the rest
     * of the component remains intact. Whether the formatting information is part of the end of the previous line, or the
     * beginning of the next line is a problem for the wrapper. Arguably, however, the beginning of the text that followed the
     * whitespace is probably the most appropriate position. This will only change the output if spaces were included at the
     * beginning or end of a block of coloured text.
     * 
    */


    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestWordSplitter
    {
        [Test]
        public void WordsAreExractedWithTrailingSpaceCounts()
        {
            const string testPhrase = "one two  three   none.";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]"));
        }

        [Test]
        public void SplitToListExtractsWordsWithTrailingSpaceCounts()
        {
            const string testPhrase = "one two  three   none.";
            var words = WordSplitter.SplitToList(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]"));
        }

        [Test]
        public void ColourControlItemsCanBeSplit()
        {
            var testPhrase = "one ".White() + "two  ".Red() + "three   none.";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]"));
        }

        [Test]
        public void ColourControlItemsCanBeSplitToList()
        {
            var testPhrase = "one ".White() + "two  ".Red() + "three   none.";
            var words = WordSplitter.SplitToList(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,1,""one""][3,2,""two""][5,3,""three""][5,0,""none.""]"));
        }

        [Test]
        public void TabsAreConvertedIntoSpaces()
        {
            const string testPhrase = "one\ttwo\t three \t eight.\t\t";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords(words);
            Console.WriteLine(result);
            Assert.That(result, Is.EqualTo(@"[3,4,""one""][3,5,""two""][5,6,""three""][6,8,""eight.""]"));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void NewlinesAreStrippedFromWords()
        {
            const string testPhrase = "text\n\nmore";
            var words = WordSplitter.Split(testPhrase, 4);
            var result = DescribeWords2(words);
            Console.WriteLine(result);
            Assert.That(words.Select(w => w.WordValue).ToArray(), Is.EqualTo(new [] { "text", string.Empty, string.Empty, "more"}));
        }

        [Test]
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

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
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

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
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
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LeadingSpacesWithinColourInstructionsAreSoftSpaces()
        {
            var testPhrase = " red".Red() +" green".Green() + " blue".Blue();
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
            Assert.That(result, Is.EqualTo(expected));
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