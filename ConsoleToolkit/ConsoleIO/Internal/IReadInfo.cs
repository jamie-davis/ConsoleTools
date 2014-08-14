using System;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Internal interface used to extract data required by the input process.
    /// </summary>
    public interface IReadInfo
    {
        string Prompt { get; }
        Type ValueType { get; }
        object MakeValueInstance(object value);
    }
}