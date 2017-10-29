using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace InteractiveCommandSample.Commands
{
    [Command]
    [Description("A command that sets the environment error code, simlating a failure.")]
    public class ErrorCommand
    {
        [CommandHandler]
        public void Handle(IErrorAdapter error)
        {
            error.WrapLine("Simulated error.".Red());
            Environment.ExitCode = -100;
        }
    }
}
