using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VT100.Attributes;
using VT100.Utilities;
using VT100.Utilities.ReadConsole;

namespace VT100
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            using (var req = new RequireVTMode())
            {
                if (args.Length == 1 && args[0] == "f")
                {
                    FullScreenTester.Run();
                    return;
                }

                DisplayColours();

                {
                    var codePage = Console.OutputEncoding.CodePage.ToString();
                    var length = codePage.Length;
                    Console.WriteLine($"╔{new String('═', length)}╗");
                    Console.WriteLine($"║{codePage}║");
                    Console.WriteLine($"╚{new String('═', length)}╝");
                }
                Console.Write("\u2501test");
                Console.Write(new[] { (char)0x1b, '[', '1', 'B' });
                Console.Write($"{req.ModeWasSet}");
                Console.Write(new[] { (char)0x1b, '[', '1', 'B' });
                Console.Write($"Input: {req.BaseInputConsoleMode} to {req.RequiredInputConsoleMode}");
                Console.Write(new[] { (char)0x1b, '[', '1', 'B' });
                Console.Write($"Output: {req.BaseOutputConsoleMode} to {req.RequiredOutputConsoleMode}");
                Console.WriteLine();
                Console.WriteLine("Ctrl-Q Quit, Ctrl-S Save keystokes, Ctrl-A Report cursor position, Ctrl-F Full screen UI test");

                var reader = new ConsoleInputReader(CodeAnalyserSettings.PreferPF3Modifiers);
                var monitor = new Monitor();
                reader.KeyMonitor = monitor;
                Task.Factory.StartNew(reader.Read, TaskCreationOptions.LongRunning);
                var goUI = false;
                Console.WriteLine("Requesting device attributes (DA)...\x1b[0c");
                foreach (var item in reader.Items.GetConsumingEnumerable())
                {
                    Console.Write(item.CodeType);
                    foreach (var controlElement in item.Items)
                    {
                        if (controlElement.KeyChar == '\x11' && item.CodeType == AnsiCodeType.None)
                            reader.Stop();
                        if (controlElement.KeyChar == '\x13' && item.CodeType == AnsiCodeType.None)
                            monitor.Save();
                        if (controlElement.KeyChar == '\x1' && item.CodeType == AnsiCodeType.None)
                            Console.Write("\x1b[6n");
                        if (controlElement.KeyChar == '\x6' && item.CodeType == AnsiCodeType.None)
                        {
                            reader.Stop();
                            goUI = true;
                        }

                        Console.Write($"[{controlElement.Key}, {(int)controlElement.KeyChar}|{(int)controlElement.KeyChar:X}|{TryRender(controlElement.KeyChar)}]");
                    }
                    if (item.CodeType == AnsiCodeType.None)
                        Console.Write(item.Items[0].KeyChar);
                    Console.WriteLine($" {item.ResolvedCode}{(item.Parameters.Any() ? $"({string.Join(", ", item.Parameters)})" : string.Empty)}");
                }

                if (goUI)
                {
                    FullScreenTester.Run();
                }
            }
        }

        private static void DisplayColours()
        {
            var consoleColours = typeof(ConsoleColor).GetFields()
                .Where(f => f.IsLiteral)
                .OrderBy(f => f.Name)
                .Select(c => new { Colour = (ConsoleColor)c.GetRawConstantValue() })
                .Select(c => new { c.Colour, VtColour = ConsoleToVtColour.Convert(c.Colour) })
                .ToList();

            foreach (var colour in consoleColours)
            {
                Console.BackgroundColor = colour.Colour;
                Console.Write(" ");
            }
            Console.WriteLine();

            var sb = new StringBuilder();
            foreach (var colour in consoleColours)
            {
                sb.Append(ColourAttribute.GetBackgroundAttribute(colour.VtColour));
                sb.Append(" ");
            }

            sb.Append(ColourAttribute.GetDefaultBackgroundAttribute());
            Console.WriteLine(sb);
            
            foreach (var colour in consoleColours)
            {
                Console.ForegroundColor = colour.Colour;
                Console.Write("X");
            }
            Console.WriteLine();

            var sb2 = new StringBuilder();
            foreach (var colour in consoleColours)
            {
                sb2.Append(ColourAttribute.GetForegroundAttribute(colour.VtColour));
                sb2.Append("X");
            }

            sb2.Append(ColourAttribute.GetDefaultForegroundAttribute());
            sb2.Append(ColourAttribute.GetDefaultBackgroundAttribute());
            Console.WriteLine(sb2);
        }

        private static string TryRender(char keyChar)
        {
            if (keyChar.Between('0', '~'))
                return keyChar.ToString();
            return string.Empty;
        }
    }
}
