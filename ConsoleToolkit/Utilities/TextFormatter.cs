using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleToolkit.Utilities
{
    public static class TextFormatter
    {
        public static string MergeBlocks(string left, int leftWidth, string right)
        {
            var leftLines = BlockToList(left);
            var rightLines = BlockToList(right);

            var sb = new StringBuilder();

            var leftEnumerator = leftLines.GetEnumerator();
            leftEnumerator.MoveNext();

            var rightEnumerator = rightLines.GetEnumerator();
            rightEnumerator.MoveNext();

            var lineFormatter = leftWidth >= 0 ? "{0,-" + leftWidth + "}{1}" : "{0}{1}";
            while (leftEnumerator.Current != null || rightEnumerator.Current != null)
            {
                var nextLine = string.Format(lineFormatter,
                    leftEnumerator.Current ?? string.Empty, rightEnumerator.Current ?? string.Empty);
                sb.AppendLine(nextLine);

                if (leftEnumerator.Current != null) leftEnumerator.MoveNext();
                if (rightEnumerator.Current != null) rightEnumerator.MoveNext();
            }

            return sb.ToString();
        }

        public static List<string> BlockToList(string textBlock)
        {
            using (var stream = new StringReader(textBlock))
            {
                var output = new List<string>();
                string line;
                while ((line = stream.ReadLine()) != null)
                    output.Add(line);
                return output;
            }
        }

        public static string FormatBlock(int textWidth, string text)
        {
            var sb = new StringBuilder();
            AppendWidth(sb, textWidth, text);
            return sb.ToString();
        }

        /// <summary>
        /// Append text using the specified <see cref="formatWidth"/> to add line breaks to the text.
        /// </summary>
        /// <param name="sb">The StringBuilder to append the text to.</param>
        /// <param name="formatWidth">The maximum width allowed for each line of text.</param>
        /// <param name="text">The text to append.</param>
        public static void AppendWidth(StringBuilder sb, int formatWidth, string text)
        {
            var lines = text.Replace("\r\n", "\n").Split('\n');
            foreach (var line in lines)
            {
                var lineLeft = formatWidth;
                var words = line.Split(' ');
                foreach (var word in words)
                {
                    if (word.Length >= lineLeft && lineLeft < formatWidth)
                    {
                        //Start new line
                        sb.AppendLine();
                        lineLeft = formatWidth - word.Length;
                        sb.Append(word);
                    }
                    else
                    {
                        //Add to current line
                        if (lineLeft != formatWidth)
                        {
                            sb.Append(" ");
                            lineLeft--;
                        }
                        sb.Append(word);
                        lineLeft -= word.Length;
                    }
                }
                if (words.Any() && lineLeft != formatWidth)
                    sb.AppendLine();
            }
        }
    }
}