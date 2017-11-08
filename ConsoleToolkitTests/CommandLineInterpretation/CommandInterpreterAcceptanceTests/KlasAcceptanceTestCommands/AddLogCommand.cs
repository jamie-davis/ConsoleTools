using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.KlasAcceptanceTestCommands
{
    /// <summary>
    /// This command was part of an issue raised by klashagel on GitHub that demonstrated an issue in command line parsing.
    /// </summary>
    [Keyword("log")]
    [InteractiveCommand("add")]
    [Description("Add a log job for controller")]
    public class AddLogCommand
    {
        [Positional(DefaultValue = "99.99.99.99")]
        [Description("IP adress of controller , ex 99.99.99.99")]
        public string ip { get; set; }

        [Positional(DefaultValue = "tags.csv")]
        [Description("Tag file to be polled , ex epictags.csv")]
        public string tagfile { get; set; }

        [Positional(DefaultValue = "1000")]
        [Description("Polling with interval in milliseconds, ex 1000 ")]
        public int interval { get; set; }

        [Positional(DefaultValue = ";")]
        [Description("Delimiter in log file")]
        public string delim { get; set; }

        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
            console.WrapLine("Polling to \"{0}\".", ip);
        }
    }
}
