using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace VT100.Tests.Fakes
{
    /// <summary>
    /// Convert characters into plausible keys based on an actual keyboard
    /// </summary>
    internal static class ConsoleKeyConverter
    {
        private static Dictionary<char, ConsoleKey?> _fixedConversions = new()
        {
            { '[', ConsoleKey.Oem1 },
            { ']', ConsoleKey.Oem6 },
            { ',', ConsoleKey.OemComma },
            { '.', ConsoleKey.OemPeriod },
            { '-', ConsoleKey.OemPlus },
            { '+', ConsoleKey.OemMinus },
            { '(', null },
            { ')', null },
            { '!', null },
            { '"', null },
            { '£', null },
            { '$', null },
            { '%', null },
            { '^', null },
            { '&', null },
            { '*', null },
            { '_', null },
            { '=', ConsoleKey.OemPlus },
            { '@', null },
            { '~', ConsoleKey.Oem7 },
            { '?', ConsoleKey.Oem2 },
            { '/', ConsoleKey.Oem2 },
            { '<', ConsoleKey.OemComma },
            { '>', ConsoleKey.OemPeriod },
            { '|', ConsoleKey.Oem5 },
            { '\\', ConsoleKey.Oem5 },
            { ';', ConsoleKey.Oem1 },
            { ':', ConsoleKey.Oem1 },
        };
        
        public static ConsoleKey? FromChar(char c)
        {
            if (_fixedConversions.TryGetValue(c, out var fixedResult))
                return fixedResult;
            
            var names = Enum.GetNames(typeof(ConsoleKey));
            var stringValue = c.ToString().ToUpper();
            if (names.Contains(stringValue))
                return Enum.Parse<ConsoleKey>(stringValue);

            var values = Enum.GetValues(typeof(ConsoleKey)).Cast<int>();
            if (values.Contains(c))
                return (ConsoleKey)c;
            
            return null;
        }
    }
}