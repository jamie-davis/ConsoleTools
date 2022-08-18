using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VT100.FullScreen.ScreenLayers;

namespace VT100.Tests.Utilities
{
    internal static class PlateDumpFormatter
    {
        private static List<string> _makeLines;

        public static string Format(Plate plate)
        {
            var text = Title(plate.Dump(DumpType.Text), "Text");
            var colour = Title(plate.Dump(DumpType.Colour), "Colour");
            var sb = new StringBuilder();
            sb.AppendLine($"Plate dimensions: x{plate.Width} y{plate.Height}");
            sb.AppendLine();
            Align(sb, text, colour);
            
            return sb.ToString();
        }

        private static string Title(string dump, string title)
        {
            var sb = new StringBuilder();
            var lines = MakeLines(dump);
            var width = lines.Max(l => l.Length);
            if (width > title.Length)
            {
                title = new string(' ', (width - title.Length) / 2) + title;
            }

            sb.AppendLine(title);
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        private static void Align(StringBuilder sb, params string[] blocks)
        {
            var blockLineLists = blocks.Select(MakeLines).ToList();
            var longest = blockLineLists.Max(l => l.Count);
            var widths = blockLineLists.Select(l => l.Max(r => r.Length)).ToList();
            for (var ix = 0; ix < longest; ix++)
            {
                for (var blockIx = 0; blockIx < widths.Count; blockIx++)
                {
                    if (blockIx > 0)
                        sb.Append("   ");
                    
                    var block = blockLineLists[blockIx];
                    if (block.Count > ix)
                    {
                        var line = block[ix];
                        line += new String(' ', widths[blockIx] - line.Length);
                        sb.Append(line);
                    }
                    else
                        sb.Append(new string(' ', widths[ix]));

                }
                sb.AppendLine();
            }
        }

        public static List<string> MakeLines(string block)
        {
            var lines = new List<string>();
            using (var reader = new StringReader(block))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    lines.Add(line);
            }
            return lines;
        }
    }
}