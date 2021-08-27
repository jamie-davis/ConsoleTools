using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.Utilities.ReadConsole;
using Xunit;

namespace VT100.Tests.Utilities
{
    public class TestAnsiRecognition
    {
        private readonly List<ControlElement?> _keys;

        public TestAnsiRecognition()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "VT100.Tests.Data.SimpleKeys.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var array = JArray.ReadFrom(jsonReader);
                var array2 = array.ToList();
                var keys = array2.SelectMany(t => t.OfType<JObject>()).ToList();
                _keys = keys.Select(o => o.ToObject<ControlElement>()).ToList();
            }
        }

        [Fact]
        public void ControlCodesAreSeparated()
        {
            string TryRender(char keyChar)
            {
                if (keyChar.Between('0', '~'))
                    return keyChar.ToString();
                return string.Empty;
            }

            var codes = AnsiRecognition.Split(_keys);
            var result = codes.Select(c => new
            {
                c.CodeType,
                Sequence = string.Join(" ",
                    c.Items.Select(controlElement =>
                        $"[{controlElement.Key.Key}, {(int)controlElement.Key.KeyChar}|{(int)controlElement.Key.KeyChar:X}|{TryRender(controlElement.Key.KeyChar)}]")),
                ResolvedCode = c.ResolvedCode
            });

            var output = new Output();
            output.FormatTable(result);
            output.Report.Verify();
        }
    }
}
