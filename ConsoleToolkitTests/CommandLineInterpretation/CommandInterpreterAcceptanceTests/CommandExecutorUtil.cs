using System.Linq;
using System.Text;
using ConsoleToolkit.CommandLineInterpretation;

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
                    sb.AppendLine(result.GetType().Name);
                    sb.AppendLine("{");
                    foreach (var propertyInfo in result.GetType().GetProperties())
                    {
                        sb.AppendLine(string.Format("    {0} = {1}", propertyInfo.Name, propertyInfo.GetValue(result)));
                    }
                    sb.AppendLine("}");
                }

                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}