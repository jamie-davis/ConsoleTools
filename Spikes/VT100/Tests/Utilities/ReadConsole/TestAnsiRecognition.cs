using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Testing;
using VT100.Utilities.ReadConsole;
using Xunit;

namespace VT100.Tests.Utilities.ReadConsole
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
                        $"[{controlElement.Key}, {(int)controlElement.KeyChar}|{(int)controlElement.KeyChar:X}|{TryRender(controlElement.KeyChar)}]")),
                ResolvedCode = c.ResolvedCode,
                Parameters = string.Join(", ", c.Parameters)
            });

            var output = new Output();
            output.FormatTable(result, ReportFormattingOptions.UnlimitedBuffer);
            output.Report.Verify();
        }
    }
}
