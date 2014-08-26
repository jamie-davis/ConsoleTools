using System;
using System.IO;
using System.Linq;
using System.Text;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkitTests.CommandLineInterpretation.CommandInterpreterAcceptanceTests
{
    public static class CommandExecutorUtil
    {
        public static string Do(CommandLineInterpreterConfiguration config, string[] commands, int width)
        {
            var sb = new StringBuilder();
            var interpreter = new CommandLineInterpreter(config);
            foreach (var command in commands)
            {
                sb.AppendLine(string.Format("Test: {0}", command));
                sb.AppendLine();

                var args = CommandLineTokeniser.Tokenise(command);
                string[] errors;
                var result = interpreter.Interpret(args, out errors);
                if (errors.Any())
                {
                    foreach (var e in errors)
                    {
                        sb.AppendLine(e);
                    }
                }
                else
                {
                    DisplayType(sb, result);
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static void DisplayType(StringBuilder sb, object result)
        {
            sb.AppendLine(result.GetType().Name);
            sb.AppendLine("{");
            foreach (var propertyInfo in result.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(result) ?? "<null>";
                if (Type.GetTypeCode(value.GetType()) == TypeCode.Object)
                {
                    var nested = new StringBuilder();
                    DisplayType(nested, value);
                    using (var stream = new StringReader(nested.ToString()))
                    {
                        string line;
                        while ((line = stream.ReadLine()) != null)
                        {
                            sb.AppendLine("    " + line);
                        }                       
                    }
                }
                else
                    sb.AppendLine(string.Format("    {0} = {1}", propertyInfo.Name, value));
            }
            sb.AppendLine("}");
        }
    }
}