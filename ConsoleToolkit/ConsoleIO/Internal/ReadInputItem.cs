namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Read a value from console in to an <see cref="InputItem"/>.
    /// </summary>
    internal static class ReadInputItem
    {
        public static bool GetValue(InputItem item, IConsoleInInterface consoleIn, IConsoleAdapter consoleOut)
        {
            var redirected = consoleIn.InputIsRedirected;

            var displayPrompt = ConstructPromptText.FromItem(item);

            do
            {
                consoleOut.Wrap(displayPrompt);
                object value;
                if (ReadValue.UsingReadLine(item, consoleIn, consoleOut, out value))
                {
                    item.Value = value;
                    return true;
                }
            } while (!redirected);

            return false;
        }

    }
}