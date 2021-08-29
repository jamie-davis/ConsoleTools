using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VT100.Utilities.ReadConsole;

namespace VT100
{
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
        }
    }
}