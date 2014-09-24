namespace ConsoleToolkit.ConsoleIO
{
    internal interface IConsoleRedirectTester
    {
        bool IsOutputRedirected();
        bool IsErrorRedirected();
    }
}