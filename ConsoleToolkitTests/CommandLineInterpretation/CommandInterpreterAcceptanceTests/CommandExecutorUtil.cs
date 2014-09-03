using System;
using System.Collections;
using System.Collections.Generic;
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

        private static void DisplayType(StringBuilder sb, object result, string fieldName = null)
        {
            if (result.GetType().GetInterfaces().Any(i => i.IsGenericType 
                && i.GetGenericTypeDefinition() == typeof(ICollection<>)))
                DisplayCollection(sb, result, fieldName);
            else if (Type.GetTypeCode(result.GetType()) == TypeCode.Object)
                DisplayObject(sb, result, fieldName);
            else
                sb.Append(result);
        }

        private static void DisplayObject(StringBuilder sb, object result, string fieldName)
        {
            if (fieldName != null)
                sb.AppendFormat("{0} = ", fieldName);

            sb.AppendLine(result.GetType().Name);
            sb.AppendLine("{");
            foreach (var propertyInfo in result.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(result) ?? "<null>";
                if (Type.GetTypeCode(value.GetType()) == TypeCode.Object)
                {
                    var nested = new StringBuilder();
                    DisplayType(nested, value, propertyInfo.Name);
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
                {
                    if (value is DateTime)
                        value = ((DateTime) value).ToString("yyyy-MM-dd HH:mm:ss");
                    sb.AppendLine(string.Format("    {0} = {1}", propertyInfo.Name, value));
                }
            }
            sb.AppendLine("}");
        }

        private static void DisplayCollection(StringBuilder sb, object collection, string fieldName)
        {
            if (fieldName != null)
                sb.AppendFormat("{0} = ", fieldName);

            sb.AppendLine(collection.GetType().Name);
            sb.AppendLine("{");
            foreach (var value in (ICollection)collection)
            {
                sb.Append("    ");
                DisplayType(sb, value);
                sb.AppendLine();
            }
            sb.AppendLine("}");
        }
    }
}