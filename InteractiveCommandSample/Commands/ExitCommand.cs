using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.InteractiveSession;

namespace InteractiveCommandSample.Commands
{
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