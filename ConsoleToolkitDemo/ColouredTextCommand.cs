using System;
using System.Linq;
using System.Text;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitDemo
{
    [Command("text")]
    [Description("Demonstrates the display of coloured text.")]
    class ColouredTextCommand
    {
        [CommandHandler]
        public void Handle(IConsoleOperations console)
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
                    Red = (("Red" + Environment.NewLine + "Lines").Cyan() + Environment.NewLine + "lines").BGDarkRed() + "Clear",
                    Wrapped = @"Complex data string.
Includes a hard newline.".Yellow()
                });
            console.FormatTable(data);
        }
    }
}