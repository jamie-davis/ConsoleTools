using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
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
                Console.Write("test");
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

        private static string TryRender(char keyChar)
        {
            if (keyChar.Between('0', '~'))
                return keyChar.ToString();
            return string.Empty;
        }
    }
}
