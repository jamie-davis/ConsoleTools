using System;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Exceptions;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestToolkit
    {
        #region Types for test

        public class TestApp : CommandDrivenApplication
        {
            public static TestApp LastTestApp { get; set; }
            public bool Initialised { get; set; }


            public TestApp()
            {
                LastTestApp = this;
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute<TestApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());

                Initialised = true;
            }

            [Command("A")]
            public class TestAppCommand
            {
            }

            [CommandHandler(typeof(TestAppCommand))]
            internal class ACommandHandler
            {
                public void Handle(TestAppCommand command)
                {
                    System.Console.WriteLine("Command handled.");
                }
            }

            public Type[] GetCommandTypesFromConfig()
            {
                return Config.Commands.Select(c => c.CommandType).ToArray();
            }
        }

        [CommandHandler(typeof(ExcludedCommand))]
        internal class ExcludedCommandHandler
        {
            public class ExcludedCommand
            {
            };

            public void Handle(ExcludedCommand command)
            {
            }
        }

        private class BadTestApp
        {
            public static void Main(string[] args)
            {
                Toolkit.Execute<BadTestApp>(args);
            }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            TestApp.LastTestApp = null;
        }

        [Test]
        public void ContainingApplicationIsExecuted()
        {
            var args = new[] {"A"};
            TestApp.Main(args);

            Assert.That(TestApp.LastTestApp.Initialised, Is.EqualTo(true));
        }

        [Test]
        public void ExcludedCommandsAreNotDiscovered()
        {
            //The test app applies a filter to exclude commands declared outside of itself. This allows us to test
            //that the filter is applied.
            var args = new[] {"A"};
            TestApp.Main(args);

            Assert.That(TestApp.LastTestApp.GetCommandTypesFromConfig(), Is.EqualTo(new[] { typeof(TestApp.TestAppCommand) }));
        }

        [Test, ExpectedException(typeof(NoApplicationClassFound))]
        public void ContainingApplicationMustBeAppDerived()
        {
            var args = new[] {"A", "B"};
            BadTestApp.Main(args);
        }
    }
}
