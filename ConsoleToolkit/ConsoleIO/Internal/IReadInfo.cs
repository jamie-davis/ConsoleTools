using System;
using System.Collections.Generic;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// Internal interface used to extract data required by the input process.
    /// </summary>
    public interface IReadInfo
    {
        string Prompt { get; }
        IEnumerable<OptionDefinition> Options { get; }
        Type ValueType { get; }
        bool ShowAsMenu { get; }
        string MenuHeading { get; }
        object MakeValueInstance(object value);
        string GetValidationError(object value);
    }
}