using System;
using System.Linq;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitDemo
{
    class Program : CommandDrivenApplication
    {
        static void Main(string[] args)
        {
            Toolkit.Execute(args);
        }

        public void Handle(TableDataCommand command, IConsoleOperations console)
        {
            var data = Enumerable.Range(0, 20)
                .Select(i => new {Text = string.Format("item {0}", i), Index = i});
            console.FormatTable(data);
        }

        public void Handle(ColouredTextCommand command, IConsoleOperations console)
        {
            console.WriteLine("Coloured".Red() + " text is " + "easy".Yellow() + " to configure.");
            console.WriteLine();
            console.WriteLine("For example:");
            console.WriteLine();
            console.WriteLine(@"    ""red on green"".Red().BGDarkGreen()");
            console.WriteLine();
            console.WriteLine("Displays like this:");
            console.WriteLine();
            console.WriteLine("red on green".Red().BGGreen());
            console.WriteLine();
            console.WriteLine("It's".Cyan()
                + "easy".BGYellow().Black()
                + "to".BGDarkCyan().Cyan()
                + "overuse".BGDarkBlue().White()
                + "it!".Magenta().BGGray());
            console.WriteLine();
            console.WriteLine();
            var data = Enumerable.Range(1, 10)
                .Select(i => new 
                {
                    Number = i, 
                    String = string.Join(" ", Enumerable.Repeat("blah", i)).Cyan(), 
                    Red = (("Red" + Environment.NewLine +"Lines").Cyan() + Environment.NewLine + "lines").BGDarkRed()+ "Clear",
                    Wrapped = @"Complex data string.
Includes a hard newline.".Yellow()
                });
            console.FormatTable(data);
        }
protected override void Initialise()
        {
            HelpCommand<HelpCommand>(h => h.Topic);
        }
    }

    [Command("help")]
    [Description("Display help text.")]
    class HelpCommand
    {
        [Positional(0, DefaultValue = null)]
        public string Topic { get; set; }
    }

    [Command("tables")]
    [Description("Displays tabulated test data.")]
    class TableDataCommand
    {
    }

    [Command("text")]
    [Description("Demonstrates the display of coloured text.")]
    class ColouredTextCommand
    {        
    }
}
