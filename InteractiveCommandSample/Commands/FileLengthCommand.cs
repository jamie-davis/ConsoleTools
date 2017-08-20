using System;
using System.IO;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace InteractiveCommandSample.Commands
{
    [Command("length")]
    [Keyword("file", "File related operations")]
    [Description("Get the length of a file in bytes.")]
    class FileLengthCommand
    {
        [Positional]
        [Description("The file to examine.")]
        public string Path { get; set; }


        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
            if (!File.Exists(Path))
            {
                error.WrapLine($"File not found: {Path.Yellow()}".Red());
                Environment.ExitCode = -100;
                return;
            }

            try
            {
                var info = new FileInfo(Path);
                console.WrapLine($"{Path} : {info.Length} bytes.".Yellow());

            }
            catch (Exception e)
            {
                error.WrapLine($"Unable to retrieve the file length for {Path.Yellow()}".Cyan());
                error.WrapLine(e.ToString().Red());
                Environment.ExitCode = -200;
            }

        }
    }
}
