using System;
using System.Collections.Generic;
using System.IO;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands
{
    /// <summary>
    /// The import command.
    /// </summary>
    [Command("import")]
    [Description("Import data from a file written previously by the export command.")]
    public class ImportCommand
    {
        [Positional]
        [Description("The file to load.")]
        public string File { get; set; }

        [Option("begin", "b")]
        [Description("Filter the data in the file such that none of the exported sessions were created before this date/time.")]
        public DateTime From { get; set; }

        [Option("end", "e")]
        [Description("Filter the data in the file such that none of the exported sessions were created after this date/time.")]
        public DateTime To { get; set; }

        [OptionSet]
        public DbOptions DbOptions { get; set; }

        [CommandValidator]
        public bool Validate(IList<string> errors)
        {
            if (File.IndexOfAny("*:".ToCharArray()) >= 0)
            {
                errors.Add("Invalid characters in filename.");
                return false;
            }

            return true;
        }


        [CommandHandler]
        public void Handle(IConsoleAdapter adapter)
        {
            if (!DbOptions.ValidateDatabaseParameters(adapter))
                return;

            if (To == default(DateTime))
                To = DateTime.MaxValue;

            adapter.WrapLine("Import data from {0} to {1} from file \"{2}\".", 
                             From.ToString().White(), 
                             To.ToString().White(), 
                             File.White());
        }

    }
}