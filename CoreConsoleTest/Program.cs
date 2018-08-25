using System;
using System.Collections.Generic;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace CoreConsoleTest
{
    class Program : CommandDrivenApplication
    {
        static void Main(string[] args)
        {
            Toolkit.Execute<Program>(args);
        }
    }


    [Command]
    [Description("Display command line help")]
    public class HelpCommand
    {
        [Positional(DefaultValue = null)]
        [Description("The command on which help is required.")]
        public List<string> Topic { get; set; }
    }

    [Command]
    [Description("Display coloured text")]
    public class TextCommand
    {
        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
            var coloured = "coloured".Red();
            console.WrapLine($"Some nice {coloured} text!".Cyan());
        }
    }
}