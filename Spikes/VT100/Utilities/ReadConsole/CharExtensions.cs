using System;
using System.Collections.Generic;
using System.Text;

namespace VT100.Utilities.ReadConsole
{
    internal static class CharExtensions
    {
        internal static bool Between(this char input, char lower, char upper)
        {
            return input >= lower && input <= upper;
        }
    }
}
