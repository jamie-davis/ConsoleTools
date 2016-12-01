using System.IO;
using System.Linq;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace InteractiveCommandSample.Commands
{
    [Command]
    [Description("List directories")]
    public class DirCommand
    {
        [Positional(".")]
        [Description("Optional path to list.")]
        public string Path { get; set; }

        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {

            console.FormatTable(Directory.EnumerateDirectories(Path).Select(d => new { Directory = d }));
        }
    }
}