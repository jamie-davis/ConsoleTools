using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.InteractiveSession;

namespace InteractiveCommandSample.Commands
{
    [Command]
    [Description("Start an interactive session")]
    public class InteractiveCommand
    {
        [CommandHandler]
        public void Handle(IInteractiveSessionService service)
        {
            service.BeginSession();
        }
    }
}