using System;
using System.IO;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace InteractiveCommandSample.Commands
{
    [Command("path")]
    [Description("Get the full path of a file.")]
    [Keyword("file")]
    class FileFullPathCommand
    {
        [Positional]
        [Description("The file for which the path is required.")]
        public string File { get; set; }


        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
            if (!System.IO.File.Exists(File))
            {
                error.WrapLine($"File not found: {File.Yellow()}".Red());
                Environment.ExitCode = -100;
                return;
            }

            try
            {
                console.WrapLine($"{Path.GetFullPath(File)}".Yellow());

            }
            catch (Exception e)
            {
                error.WrapLine($"Unable to retrieve the full path for {File.Yellow()}".Cyan());
                error.WrapLine(e.ToString().Red());
                Environment.ExitCode = -200;
            }

        }
    }
}