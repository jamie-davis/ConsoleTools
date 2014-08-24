using System.Linq;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitDemo
{
    [Command("input")]
    [Description("Demonstrates getting input from the user via console input.")]
    class GetInputCommand
    {
        /// <summary>
        /// This is the self handler for the input command.
        /// </summary>
        /// <param name="console"></param>
        [CommandHandler]
        public void Handle(IConsoleAdapter console)
        {
            console.WrapLine("Get user input".BGWhite().Black());
            console.WriteLine();

            var item = console.ReadInput(new {String = Read.String().Prompt("Enter some text".Yellow())});

            var characters = item.String.Value
                .Select(c => string.Format(@"""{0}"" = {1}".Red(), 
                    ConsoleIOExtensions.Yellow(c.ToString()),
                    string.Format("{0:X}", (byte) c).PadLeft(2, '0').Green()));

            console.WriteLine();
            console.WrapLine(string.Join("  ", characters));
            console.WriteLine();
            
            console.ReadLine();
        }
    }
}