using System;
using System.Collections.Generic;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace CommandAppWithGlobalOptions
{
    class Program : CommandDrivenApplication
    {
        static void Main(string[] args)
        {
            Toolkit.Execute<Program>(args);
        }

        #region Overrides of ConsoleApplicationBase

        protected override void Initialise()
        {
            base.HelpCommand<HelpCommand>(h => h.Topic);
            base.Initialise();
        }

        #endregion
    }

    [Command]
    [Description("Get help text")]
    public class HelpCommand
    {
        [Positional(DefaultValue = null)]
        [Description("Help topic")]
        public List<string> Topic { get; set; }
    }

    [Command]
    [Description("First command")]
    public class FirstCommand
    {
        [CommandHandler]
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine("First".Yellow());
        }
    }

    [Command]
    [Description("Second command")]
    public class SecondCommand
    {
        [CommandHandler]
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine("Second".Yellow());
        }
    }

    [Command]
    [Description("Second command")]
    public class ThirdCommand
    {
        [CommandHandler]
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine("Third".Yellow());
        }
    }

    [GlobalOptions]
    public static class GlobalOptions
    {
        [Option("global")]
        [Description("Global option")]
        public static string Option { get; set; }
    }

}
