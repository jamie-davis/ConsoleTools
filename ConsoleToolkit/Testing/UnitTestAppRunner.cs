﻿using System;
using System.Diagnostics;
using System.Reflection;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit.Testing
{
    public static class UnitTestAppRunner
    {
        /// <summary>
        /// Run an instance of a console application derived type.
        /// </summary>
        /// <typeparam name="T">The test type.</typeparam>
        /// <param name="consoleInterface">The console interface to use for the test.</param>
        /// <param name="args">The arguments to pass.</param>
        public static void Run<T>(string[] args = null, IConsoleInterface consoleInterface = null) where T : class
        {
            CleanEnvironment();

            var type = typeof (T);
            var toolkitBase = type.BaseType;
            Debug.Assert(toolkitBase != null);

            var runMethod = toolkitBase.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(runMethod != null);

            object instance = null;
            try
            {
                var errorPrefix = string.Format("{0}: ", DefaultApplicationNameExtractor.Extract(typeof(T)));

                instance = Activator.CreateInstance(type, null);
                MethodInvoker.Invoke(runMethod, null, 
                                               instance, 
                                               args ?? new string[]{}, 
                                               new ConsoleAdapter(consoleInterface ?? new ConsoleInterfaceForTesting()),
                                               new ErrorAdapter(consoleInterface ?? new ConsoleInterfaceForTesting(), errorPrefix)
                                           );
            }
            finally
            {
                if (instance is IDisposable)
                {
                    (instance as IDisposable).Dispose();
                }
            }
        }

        private static void CleanEnvironment()
        {
            Environment.ExitCode = 0;
        }
    }
}