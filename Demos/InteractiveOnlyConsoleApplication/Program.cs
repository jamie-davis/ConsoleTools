using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.InteractiveSession;

namespace InteractiveOnlyConsoleApplication
{
    class Program : ConsoleApplication
    {
        static void Main(string[] args)
        {
            Toolkit.Execute<Program>(args);
        }

        #region Overrides of ConsoleApplicationBase

        protected override void Initialise()
        {
            HelpOption<Options>(o => o.Help);
            base.Initialise();
        }

        #endregion
    }

    [Command]
    [Description("A demo of an interactive only console application.")]
    public class Options
    {
        [Option("h")]
        [Description("Display help text")]
        public bool Help { get; set; }

        [CommandHandler]
        public void Handle(IInteractiveSessionService service)
        {
            service.SetPrompt("-->");
            service.BeginSession();
        }
    }

    [InteractiveCommand]
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

    [InteractiveCommand]
    [Description("End the interactive session.")]
    public class ExitCommand
    {
        [CommandHandler]
        public void Handle(IInteractiveSessionService service)
        {
            service.EndSession();
        }
    }
}
