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
        private static ConsoleAdapter _adapter;


        static void Main(string[] args)
        {
            _adapter = new ConsoleAdapter(Console.Out, Console.Error, Console.BufferWidth);
            _config = ConfigureCommandLine();
            var interpreter = new CommandLineInterpreter(_config);
            string[] errors;
            var command = interpreter.Interpret(args, out errors);
            if (command == null)
            {
                foreach (var error in errors)
                {
                    _adapter.ErrorLine(error);
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

            _adapter.ErrorLine("Internal error: No handler for command.");
        }

        private static void Handle(HelpCommand command)
        {
            _adapter.PrintLine(_config.Describe(_adapter.Width));
        }

        private static void Handle(TableDataCommand command)
        {
            var data = Enumerable.Range(0, 20)
                .Select(i => new {Text = string.Format("item {0}", i), Index = i});
            _adapter.Report(data);
        }

        private static CommandLineInterpreterConfiguration ConfigureCommandLine()
        {
            var config = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MsDosConventions);
            config.Command<HelpCommand>("help")
                .Description("Display help text.");
            config.Command<TableDataCommand>("tables")
                .Description("Displays tabulated test data.");
            return config;
        }
    }

    class TableDataCommand
    {
    }
}
