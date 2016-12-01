using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using InteractiveCommandSample.Commands;

namespace InteractiveCommandSample
{
    class Program : CommandDrivenApplication
    {
        static void Main(string[] args)
        {
            Toolkit.Execute<Program>(args);
        }

        #region Overrides of ConsoleApplicationBase

        protected override void Initialise()
        {
            HelpCommand<HelpCommand>(h => h.Topic);
            base.Initialise();
        }

        #endregion
    }
}

