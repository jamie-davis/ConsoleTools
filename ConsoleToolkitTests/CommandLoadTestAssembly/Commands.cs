using System.Runtime.CompilerServices;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;

namespace CommandLoadTestAssembly
{
    [Command("one")]
    public class Command1
    {

    }

    [Command("two")]
    public class Command2
    {

    }

    [Command("three")]
    public class Command3
    {

    }

    [Command("four")]
    public class Command4
    {
        [CommandHandler]
        public void Handle()
        {}
    }

    [InteractiveCommand("ione")]
    public class InteractiveCommand1
    {

    }

    [InteractiveCommand("itwo")]
    public class InteractiveCommand2
    {

    }

    [NonInteractiveCommand("none")]
    public class NonInteractiveCommand1
    {

    }

    [NonInteractiveCommand("ntwo")]
    public class NonInteractiveCommand2
    {

    }

    public class Program : CommandDrivenApplication
    {
        public static Program LastProgram { get; set; }
        public static bool Executed3 { get; set; }

        public Program()
        {
            LastProgram = this;
            Executed3 = false;
        }

        public CommandLineInterpreterConfiguration GetConfig()
        {
            return Config;
        }
               
        protected override void Initialise()
        {
            SetConfigTypeFilter(t => t.Namespace == GetType().Namespace);
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static void XMain(string[] args)
        {
            Toolkit.Execute<Program>(args);
        }
    }

    [CommandHandler(typeof (Command1))]
    public class Command1Handler
    {
        public void Handle(Command1 command)
        {
            
        }
    }

    [CommandHandler(typeof (Command2))]
    public class Command2Handler
    {
        public void Handle(Command2 command)
        {

        }
    }

    [CommandHandler(typeof (Command3))]
    public class Command3Handler
    {
        public void Handle(Command3 command)
        {
            Program.Executed3 = true;
        }
    }

}
