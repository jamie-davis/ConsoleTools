using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit
{
    public static class Toolkit
    {
        private const CommandLineParserConventions DefaultParserConventions = CommandLineParserConventions.MicrosoftStandard;
        private static List<Type> _toolkitBaseClassTypes = new List<Type>
        {
            typeof(ConsoleApplication),
            typeof(CommandDrivenApplication),
        };

        private static CommandLineParserConventions _parsingConventions = DefaultParserConventions;

        /// <summary>
        /// Reset the toolkit to its default state. Only useful to unit tests.
        /// </summary>
        internal static void GlobalReset()
        {
            ParsingConventions = DefaultParserConventions;
        }

        public static CommandLineParserConventions ParsingConventions
        {
            get { return _parsingConventions; }
            set { _parsingConventions = value; }
        }

        public static void Execute(string[] args)
        {
            var consoleAdapter = CreateConsoleAdapter();
            var typesInCallStack = StackWalker.StackedTypes().Reverse();
            var type = typesInCallStack.FirstOrDefault(IsToolkitDerived);

            var toolkitBase = type == null ? null : GetToolkitBaseClass(type);
            if (toolkitBase == null)
                throw new NoApplicationClassFound();

            var runMethod = toolkitBase.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(runMethod != null);

            object instance = null;
            try
            {
                instance = Activator.CreateInstance(type, null);
                runMethod.Invoke(null, new [] {instance, args, consoleAdapter});
            }
            finally
            {
                if (instance is IDisposable)
                {
                    (instance as IDisposable).Dispose();
                }
            }
        }

        private static IConsoleAdapter CreateConsoleAdapter()
        {
            return new ConsoleAdapter();
        }

        private static bool IsToolkitDerived(Type type)
        {
            if (type.GetConstructor(new Type[] {}) == null)
                return false;

            return GetToolkitBaseClass(type) != null;
        }

        private static Type GetToolkitBaseClass(Type type)
        {
            while (true)
            {
                if (type == null || type.BaseType == null) 
                    return null;

                var toolkitBase = _toolkitBaseClassTypes.FirstOrDefault(t => t == type.BaseType);
                if (toolkitBase != null)
                    return toolkitBase;

                type = type.BaseType;
            }
        }
    }
}
