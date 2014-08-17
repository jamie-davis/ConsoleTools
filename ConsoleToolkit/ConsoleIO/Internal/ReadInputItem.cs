using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;

/// <summary>
/// Read a value from console in to an <see cref="InputItem"/>.
/// </summary>
internal static class ReadInputItem
{
    public static bool GetValue(InputItem item, IConsoleInInterface consoleIn, IConsoleAdapter consoleOut)
    {
        var redirected = consoleIn.InputIsRedirected;

        var displayPrompt = ConstructPromptText(item);

        do
        {
            consoleOut.Wrap(displayPrompt);
            object value;
            if (ReadValue(item, consoleIn, consoleOut, out value))
            {
                item.Value = value;
                return true;
            }
        } while (!redirected);

        return false;
    }

    private static string ConstructPromptText(InputItem item)
    {
        if (item.ReadInfo == null)
            return AddPromptSuffix(PropertyNameConverter.ToPrompt(item.Property));

        var prompt = item.ReadInfo.Prompt ?? PropertyNameConverter.ToPrompt(item.Property);

        if (item.ReadInfo.Options.Any())
            return MakeOptionsPrompt(item, prompt);

        return AddPromptSuffix(prompt);
    }

    private static string MakeOptionsPrompt(InputItem item, string prompt)
    {
        return !item.ReadInfo.ShowAsMenu 
            ? MakeSelectPrompt(item, prompt) 
            : MakeMenuPrompt(item, prompt);
    }

    private static string MakeSelectPrompt(InputItem item, string prompt)
    {
        var options = item.ReadInfo.Options.Select(MakeOptionText);
        var optionString = string.Format("[{0}]", string.Join(", ", options));
        var displayPrompt = string.Format("{0} {1}", prompt, optionString);
        return AddPromptSuffix(displayPrompt);
    }

    private static string MakeMenuPrompt(InputItem item, string prompt)
    {
        var promptBuilder = new StringBuilder();
        if (item.ReadInfo.MenuHeading != null)
        {
            promptBuilder.AppendLine(item.ReadInfo.MenuHeading);
            promptBuilder.AppendLine();
        }

        foreach (var option in item.ReadInfo.Options)
            promptBuilder.AppendLine(MakeOptionText(option));

        promptBuilder.AppendLine();
        promptBuilder.AppendLine(AddPromptSuffix(prompt));
        return promptBuilder.ToString();
    }

    private static string MakeOptionText(OptionDefinition option)
    {
        return string.Format("{0}-{1}", option.RequiredValue, option.Prompt);
    }

    private static string AddPromptSuffix(string prompt)
    {
        return prompt + ": ";
    }

    private static bool ReadValue(InputItem item, IConsoleInInterface consoleIn, IConsoleAdapter consoleOut, out object value)
    {
        var input = consoleIn.ReadLine();
        if (item.ReadInfo != null && item.ReadInfo.Options.Any())
            return SelectOption(input, item.ReadInfo.Options, consoleOut, out value);
        
        return ConvertString(input, item.Type, consoleOut, out value);
    }

    private static bool SelectOption(string input, IEnumerable<OptionDefinition> options, IConsoleAdapter consoleOut, out object result)
    {
        var hit = options.FirstOrDefault(o => o.RequiredValue == input);
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