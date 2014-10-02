using System;
using System.Collections.Generic;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Properties;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.ApplicationStyles;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class ConsoleInputAcceptanceTests
    {
        private ConsoleInterfaceForTesting _console;

        #region Test programs

        [UsedImplicitly]
        public class Program1 : ConsoleApplication
        {
            [Command]
            public class Options
            {
                [CommandHandler]
                public void Handle(IConsoleAdapter adapter)
                {
                    var item = adapter.ReadInput(new {String = string.Empty, Int = 0});
                    adapter.WriteLine();
                    if (item == null)
                    {
                        adapter.WrapLine("Invalid input. No item received.");
                    }
                    else
                    {
                        adapter.WrapLine("Input received.");
                        adapter.WrapLine("String was \"{0}\", integer was {1}", item.String, item.Int);
                    }

                }
            }

            public Program1()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute<Program1>(args);
            }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            _console = new ConsoleInterfaceForTesting();
        }

        private void AttachInput(IEnumerable<string> strings)
        {
            _console.SetInputStream(new StringReader(strings.JoinWith(Environment.NewLine)));
        }

        [Test]
        public void ValidInputIsAccepted()
        {
            AttachInput(new []
            {
                "text",
                "45"
            });
            UnitTestAppRunner.Run<Program1>(consoleInterface: _console);
            Approvals.Verify(_console.GetBuffer());
        }

        [Test]
        public void TheUserMustEnterAValidValue()
        {
            AttachInput(new []
            {
                "text",
                "bad",
                "still bad",
                "45"
            });
            UnitTestAppRunner.Run<Program1>(consoleInterface: _console);
            var buffer = _console.GetBuffer();
            Console.WriteLine(buffer);
            Approvals.Verify(buffer);
        }

        [Test]
        public void BadInputResultsInNullObjectWhenInpoutRedirected()
        {
            AttachInput(new []
            {
                "text",
                "bad",
                "still bad",
                "45"
            });
            _console.InputIsRedirected = true;

            UnitTestAppRunner.Run<Program1>(consoleInterface: _console);
            var buffer = _console.GetBuffer();
            Console.WriteLine(buffer);
            Approvals.Verify(buffer);
        }
    }
}
