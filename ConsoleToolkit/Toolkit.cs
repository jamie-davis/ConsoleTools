using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Utilities;

namespace ConsoleToolkit
{
    public class ToolkitOptions
    {
        internal class ConfirmationDetails
        {
            public string YesText { get; set; }
            public string YesPrompt { get; set; }
            public string NoText { get; set; }
            public string NoPrompt { get; set; }
        }

        private const CommandLineParserConventions DefaultParserConventions = CommandLineParserConventions.MicrosoftStandard;

        private CommandLineParserConventions _parsingConventions = DefaultParserConventions;

        public CommandLineParserConventions ParsingConventions
        {
            get { return _parsingConventions; }
            set { _parsingConventions = value; }
        }

        private ConfirmationDetails _confirmationInfo;
        internal ConfirmationDetails ConfirmationInfo { get { return _confirmationInfo; } }

        internal ToolkitOptions()
        {
            OverrideConfirmOptions("Y", "Yes", "N", "No");
        }

        public void OverrideConfirmOptions(string trueText, string truePrompt, string falseText, string falsePrompt)
        {
            _confirmationInfo = new ConfirmationDetails
            {
                YesText = trueText,
                YesPrompt = truePrompt,
                NoText = falseText,
                NoPrompt = falsePrompt
            };
        }
    }
    public static class Toolkit
    {
        private static ToolkitOptions _options = new ToolkitOptions();
        public static ToolkitOptions Options { get { return _options; } }

        private static List<Type> _toolkitBaseClassTypes = new List<Type>
        {
            typeof(ConsoleApplication),
            typeof(CommandDrivenApplication),
        };


        /// <summary>
        /// Reset the toolkit to its default state. Only useful to unit tests.
        /// </summary>
        internal static void GlobalReset()
        {
            _options = new ToolkitOptions();
        }

        public static void Execute<T>(string[] args)
        {
            Execute(args, typeof(T));
        }

        private static void Execute(string[] args, Type type)
        {
            var consoleAdapter = CreateConsoleAdapter();
            var toolkitBase = type == null ? null : GetToolkitBaseClass(type);
            if (toolkitBase == null)
                throw new NoApplicationClassFound();

            var runMethod = toolkitBase.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(runMethod != null);

            object instance = null;
            try
            {
                instance = Activator.CreateInstance(type, null);
                runMethod.Invoke(null, new[] {instance, args, consoleAdapter});
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
