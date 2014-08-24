using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Get input from the user and interpret it.
    /// </summary>
    internal static class ReadValue
    {
        internal static bool UsingReadLine(InputItem item, IConsoleInInterface consoleIn, IConsoleAdapter consoleOut, out object value)
        {
            var input = consoleIn.ReadLine();
            return ConvertInput(item, consoleOut, out value, input)
                   && ApplyValidations(item, value, consoleOut);
        }

        private static bool ApplyValidations(InputItem item, object value, IConsoleAdapter consoleOut)
        {
            if (item.ReadInfo != null)
            {
                var error = item.ReadInfo.GetValidationError(value);
                if (error != null)
                {
                    consoleOut.WrapLine(error);
                    return false;
                }
            }

            return true;
        }

        private static bool ConvertInput(InputItem item, IConsoleAdapter consoleOut, out object value, string input)
        {
            if (item.ReadInfo != null && item.ReadInfo.Options.Any())
                return SelectOption(input, item.ReadInfo.Options, consoleOut, out value);

            return ConvertString(input, item.Type, consoleOut, out value);
        }

        private static bool SelectOption(string input, IEnumerable<OptionDefinition> optionDefinitions, IConsoleAdapter consoleOut, out object result)
        {
            var options = optionDefinitions.ToList();
            var hit = options.FirstOrDefault(o => o.RequiredValue == input)
                ?? options.FirstOrDefault(o => string.Compare(o.RequiredValue, input, StringComparison.OrdinalIgnoreCase) == 0);
            if (hit == null)
            {
                consoleOut.WrapLine(@"""{0}"" is not a valid selection.", input);
                result = null;
                return false;
            }

            result = hit.SelectedValue;
            return true;
        }

        private static bool ConvertString(string input, Type type, IConsoleAdapter consoleOut, out object result)
        {
            try
            {
                var conversion = typeof(Convert).GetMethods()
                    .FirstOrDefault(m => m.ReturnType == type
                                         && m.GetParameters().Length == 1
                                         && m.GetParameters()[0].ParameterType == typeof(string));
                if (conversion != null)
                {
                    result = conversion.Invoke(null, new object[] { input });
                    return true;
                }

                result = null;
            }
            catch (TargetInvocationException e)
            {
                result = null;
                consoleOut.WrapLine(e.InnerException.Message);
            }
            return false;
        }
    }
}