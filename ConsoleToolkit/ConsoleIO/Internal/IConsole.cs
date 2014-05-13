using System.Collections.Generic;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    public interface IConsole
    {
        void DisplayColumns<T>(IEnumerable<T> report, bool showHeadings = true);
        void Print(string data, params object[] args);
        void WrapOutput(string data, params object[] args);
        void PrintLine(string data = null, params object[] args);
        void WrapError(string data, params object[] args);
        void Error(string data, params object[] args);
        void ErrorLine(string data, params object[] args);
        int Width { get; }
        void DisplayColumnsOnError<T>(IEnumerable<T> input, bool showHeadings = true);
    }
}
