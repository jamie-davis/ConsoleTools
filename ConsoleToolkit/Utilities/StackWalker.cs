using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ConsoleToolkit.Utilities
{
    public static class StackWalker
    {
        public static IEnumerable<Type> StackedTypes()
        {
            var st = new StackTrace(true);
            for (var i = 1; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);
                var method = frame.GetMethod();
                if (method != null)
                {
                    yield return method.DeclaringType;
                }
            }
        }
    }
}
