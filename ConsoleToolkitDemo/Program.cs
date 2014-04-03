using System;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.CommandLineInterpretation;

namespace ConsoleToolkitDemo
{
    class HelpCommand
    {
        
    }

    class Program
    {
        private static CommandLineInterpreterConfiguration _config;


        static void Main(string[] args)
        {
            _config = ConfigureCommandLine();
            var interpreter = new CommandLineInterpreter(_config);
            string[] errors;
            var command = interpreter.Interpret(args, out errors);
            if (command == null)
            {
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
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
            Console.WriteLine("Internal error: No handler for command.");
        }

        private static void Handle(HelpCommand command)
        {
            Console.WriteLine(_config.Describe(50));
        }

        private static CommandLineInterpreterConfiguration ConfigureCommandLine()
        {
            var config = new CommandLineInterpreterConfiguration(CommandLineParserConventions.MsDosConventions);
            config.Command<HelpCommand>("help")
                .Description("Display help text.");
            return config;
        }
    }
}
