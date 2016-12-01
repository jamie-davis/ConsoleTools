using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.InteractiveSession;

namespace InteractiveCommandSample.Commands
{
    [Command]
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