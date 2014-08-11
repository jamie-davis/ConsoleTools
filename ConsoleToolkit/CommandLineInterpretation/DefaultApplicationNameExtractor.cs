using System;
using System.Diagnostics;

namespace ConsoleToolkit.CommandLineInterpretation
{
    public static class DefaultApplicationNameExtractor
    {
        public static string Extract(Type applicationClass)
        {
            var trace = new StackTrace();
            var index = 0;
            string bestName = null;
            while (index < trace.FrameCount)
            {
                var frame = trace.GetFrame(index++);
                var method = frame.GetMethod();
                if (method.DeclaringType != null)
                {
                    var assembly = method.DeclaringType.Assembly;
                    if (method.Name == "Main")
                    {
                        if (method.DeclaringType != null)
                            return assembly.GetName().Name;
                    }
                    else
                    {
                        if (bestName == null && assembly != applicationClass.Assembly)
                            bestName = assembly.GetName().Name;
                    }
                }
            }
            return bestName;
        }
    }
}