using System;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitDemo
{
    class HelpCommand
    {
        
    }

    class Program
    {
        private static CommandLineInterpreterConfiguration _config;
        private static ConsoleAdapter _console;


        static void Main(string[] args)
        {
            _console = new ConsoleAdapter();
            _config = ConfigureCommandLine();
            var interpreter = new CommandLineInterpreter(_config);
            string[] errors;
            var command = interpreter.Interpret(args, out errors);
            if (command == null)
            {
                foreach (var error in errors)
                {
                    _console.WriteLine(error.Red());
                }
                return;
            }

            ProcessCommand(command);
        }

        private static void ProcessCommand(object command)
        {
            var method = typeof (Program)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == "Handle"
                                     && m.GetParameters()
                                         .Any(p => p.ParameterType == command.GetType()));
            if (method != null)
            {
                method.Invoke(null, new[] {command});
                return;
            }

            _console.WriteLine("Internal error: No handler for command.");
        }

        private static void Handle(HelpCommand command)
        {
            //_console.WriteLine(_config.Describe(_console.Width));
        }

        private static void Handle(TableDataCommand command)
        {
            var data = Enumerable.Range(0, 20)
                .Select(i => new {Text = string.Format("item {0}", i), Index = i});
            _console.FormatTable(data);
        }

        private static void Handle(ColouredTextCommand command)
        {
            _console.WriteLine("Coloured".Red() + " text is " + "easy".Yellow() + " to configure.");
            _console.WriteLine();
            _console.WriteLine("For example:");
            _console.WriteLine();
            _console.WriteLine(@"    ""red on green"".Red().BGDarkGreen()");
            _console.WriteLine();
            _console.WriteLine("Displays like this:");
            _console.WriteLine();
            _console.WriteLine("red on green".Red().BGGreen());
            _console.WriteLine();
            _console.WriteLine("It's".Cyan() 
                + "easy".BGYellow().Black() 
                + "to".BGDarkCyan().Cyan() 
                + "overuse".BGDarkBlue().White() 
                + "it!".Magenta().BGGray());
            _console.WriteLine();
            _console.WriteLine();
            var data = Enumerable.Range(1, 10)
                .Select(i => new 
                {
                    Number = i, 
                    String = string.Join(" ", Enumerable.Repeat("blah", i)).Cyan(), 
                    Red = (("Red" + Environment.NewLine +"Lines").Cyan() + Environment.NewLine + "lines").BGDarkRed()+ "Clear",
                    Wrapped = @"Complex data string.
Includes a hard newline.".Yellow()
                });
            _console.FormatTable(data);
        }

        private static CommandLineInterpreterConfiguration ConfigureCommandLine()
        {
            var config = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MsDosConventions);
            config.Command<HelpCommand>("help")
                .Description("Display help text.");
            config.Command<TableDataCommand>("tables")
                .Description("Displays tabulated test data.");
            config.Command<ColouredTextCommand>("text")
                .Description("Demonstrates the display of coloured text.");
            return config;
        }
    }

    class TableDataCommand
    {
    }

    class ColouredTextCommand
    {
        
    }
}
