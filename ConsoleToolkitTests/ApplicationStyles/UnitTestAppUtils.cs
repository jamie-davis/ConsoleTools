﻿using System;
using System.Diagnostics;
using System.Reflection;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitTests.ApplicationStyles
{
    public static class UnitTestAppUtils
    {
        /// <summary>
        /// Run an instance of a console application derived type.
        /// </summary>
        /// <typeparam name="T">The test type.</typeparam>
        /// <param name="consoleInterface">The console interface to use for the test.</param>
        /// <param name="args">The arguments to pass.</param>
        public static void Run<T>(string[] args = null, IConsoleInterface consoleInterface = null) where T : class
        {
            var type = typeof (T);
            var toolkitBase = type.BaseType;
            Debug.Assert(toolkitBase != null);

            var runMethod = toolkitBase.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(runMethod != null);

            object instance = null;
            try
            {
                instance = Activator.CreateInstance(type, null);
                runMethod.Invoke(null, new[] { instance, args ?? new string[]{}, new ConsoleAdapter(consoleInterface ?? new ConsoleInterfaceForTesting())});
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
            finally
            {
                if (instance is IDisposable)
                {
                    (instance as IDisposable).Dispose();
                }
            }
        }
    }
}