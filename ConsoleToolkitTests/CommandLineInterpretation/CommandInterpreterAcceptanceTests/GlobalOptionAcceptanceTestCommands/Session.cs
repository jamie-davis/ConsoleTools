using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.InteractiveSession;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests.GlobalOptionAcceptanceTestCommands
{
    [NonInteractiveCommand]
    [Description("Start an interactive session")]
    class SessionCommand
    {
        [CommandHandler]
        public void Handle(IInteractiveSessionService interactiveSessionService)
        {
            interactiveSessionService.SetPrompt("-->");
            interactiveSessionService.BeginSession();
        }

    }
}
