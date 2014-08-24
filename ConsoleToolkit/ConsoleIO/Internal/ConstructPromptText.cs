using System.Linq;
using System.Text;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Construct a prompt string.
    /// </summary>
    internal static class ConstructPromptText
    {
        internal static string FromItem(InputItem item)
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
            var displayPrompt = string.Format("{0}{1}{2}", 
                prompt, 
                string.IsNullOrWhiteSpace(prompt) ? string.Empty : " ", 
                optionString);
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
            promptBuilder.Append(AddPromptSuffix(prompt));
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
    }
}