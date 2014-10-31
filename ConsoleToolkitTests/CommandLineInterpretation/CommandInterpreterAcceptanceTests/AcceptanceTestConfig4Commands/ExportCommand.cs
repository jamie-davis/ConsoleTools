using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

// ReSharper disable UnusedMember.Global

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands
{
    /// <summary>
    /// The export command.
    /// </summary>
    [Command("export")]
    [Description("Export the data from a specified time range to a file.")]
    public class ExportCommand
    {
        [Positional]
        [Description("The file to create.")]
        public string File { get; set; }

        [Positional]
        [Description("Start date/time for the extract.")]
        public DateTime From { get; set; }

        [Positional]
        [Description("End date/time for the extract.")]
        public DateTime To { get; set; }

        [OptionSet]
        public DbOptions DbOptions { get; set; }

        [CommandValidator]
        public bool Validate()
        {
            if (File.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new Exception("Invalid characters in filename.");

            return true;
        }

        [CommandHandler]
        public void Handle(IConsoleAdapter adapter)
        {
            if (!DbOptions.ValidateDatabaseParameters(adapter))
            {
                return;
            }

            adapter.WrapLine("Server: {0}  Database: {1}", DbOptions.Server ?? "(config)", DbOptions.Database ?? "(config)");
            adapter.WriteLine();

            adapter.WrapLine("Export data from {0} to {1} to file \"{2}\".",
                             From.ToString().White(),
                             To.ToString().White(),
                             File.White());

            adapter.WriteLine();
        }
    }
}
