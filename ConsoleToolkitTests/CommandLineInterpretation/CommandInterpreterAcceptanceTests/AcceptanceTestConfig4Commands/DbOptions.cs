using System;
using System.Collections.Generic;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.AcceptanceTestConfig4Commands
{
    /// <summary>
    /// The base class for all commands that can accept database configuration parameters.
    /// </summary>
    public class DbOptions
    {
        [Option("server", "s")]
        [Description("The name of the database server.")]
        public string Server { get; set; }

        [Option("database", "d")]
        [Description("The name of the database on the specified server.")]
        public string Database { get; set; }

        [Option("user", "u")]
        [Description("The user ID to use to connect to the database")]
        public string User { get; set; }

        [Option("pwd", "p")]
        [Description("The password to use to connect to the database")]
        public string Password { get; set; }

        [Option("filter", "f")]
        [Description("Global data filter")]
        public List<string> Filters { get; set; }

        [CommandValidator]
        public bool Validate()
        {
            if (Server != null && Server.Length > 10)
                throw new Exception("Server name too long.");

            return true;
        }

        public bool ValidateDatabaseParameters(IConsoleAdapter adapter)
        {
            var serverSpecified = Server != null;
            var databaseSpecified = Database != null;
            var userSpecified = User != null;
            var passwordSpecified = Password != null;

            if ((serverSpecified && !databaseSpecified)
                || (!serverSpecified && databaseSpecified))
            {
                adapter.WrapLine("Server name and database name must be specified together.");
                return false;
            }

            if ((userSpecified && !passwordSpecified)
                || (!userSpecified && passwordSpecified))
            {
                adapter.WrapLine("User name and password must be specified together.");
                return false;
            }

            if (userSpecified && !serverSpecified)
            {
                adapter.WrapLine("User name and password cannot be specified unless a database is specified.");
                return false;
            }
            return true;
        }
    }
}