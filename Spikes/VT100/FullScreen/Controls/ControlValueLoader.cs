using System;

namespace VT100.FullScreen.Controls
{
    internal static class ControlValueLoader
    {
        public static string GetString(Func<object,object> getter, ILayout dataContainer)
        {
            if (dataContainer == null || getter == null)
                return null;

            var objValue = getter(dataContainer);
            if (objValue is string strValue)
                return strValue;

            return objValue?.ToString() ?? string.Empty;
        }
    }
}