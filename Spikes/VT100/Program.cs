using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

                var reader = new ConsoleInputReader();
                var monitor = new Monitor();
                reader.KeyMonitor = monitor;
                Task.Factory.StartNew(reader.Read, TaskCreationOptions.LongRunning);
                foreach (var item in reader.Items.GetConsumingEnumerable())
                {
                    Console.Write(item.CodeType);
                    foreach (var controlElement in item.Items)
                    {
                        if (controlElement.Key.KeyChar == '\x11' && item.CodeType == AnsiCodeType.None)
                            reader.Stop();
                        if (controlElement.Key.KeyChar == '\x13' && item.CodeType == AnsiCodeType.None)
                            monitor.Save();

                        Console.Write($"[{controlElement.Key.Key}, {(int)controlElement.Key.KeyChar}|{(int)controlElement.Key.KeyChar:X}|{TryRender(controlElement.Key.KeyChar)}]");
                    }
                    if (item.CodeType == AnsiCodeType.None)
                        Console.Write(item.Items[0].Key.KeyChar);
                    Console.WriteLine($" {item.ResolvedCode}");
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

    class Monitor : IInputMonitor
    {
        public List<List<ControlElement>> History { get; } = new List<List<ControlElement>>();
        public void SequenceCaptured(IEnumerable<ControlElement> elements)
        {
            History.Add(elements.ToList());
        }

        public void Save()
        {
            var temp = Path.GetTempPath();
            var file = Path.Combine(temp, $"Keys_{DateTime.Now:yyyyMMMMdd-hhmmss}.json");
            using (var textWriter = new StreamWriter(file))
            using (var writer = new JsonTextWriter(textWriter))
            {
                writer.Formatting = Formatting.Indented;
                JArray.FromObject(History).WriteTo(writer);
            }
            Console.WriteLine($"Saved to {file}");
            Process.Start("code", file);
        }
    }
}
