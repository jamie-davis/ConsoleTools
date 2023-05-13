using System;
using System.Linq;
using ApprovalTests.Reporters;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Exceptions;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests
{
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

            public static void XMain(string[] args)
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
            public static void XMain(string[] args)
            {
                Toolkit.Execute<BadTestApp>(args);
            }
        }
        
        #endregion
        public TestToolkit()
        {
            TestApp.LastTestApp = null;
        }

        [Fact]
        public void ContainingApplicationIsExecuted()
        {
            var args = new[] {"A"};
            TestApp.XMain(args);

            Assert.Equal(true, TestApp.LastTestApp.Initialised);
        }

        [Fact]
        public void ExcludedCommandsAreNotDiscovered()
        {
            //The test app applies a filter to exclude commands declared outside of itself. This allows us to test
            //that the filter is applied.
            var args = new[] {"A"};
            TestApp.XMain(args);

            Assert.Equal(new[] { typeof(TestApp.TestAppCommand) }, TestApp.LastTestApp.GetCommandTypesFromConfig());
        }

        [Fact]
        public void ContainingApplicationMustBeAppDerived()
        {
            Assert.Throws<NoApplicationClassFound>(() =>
            {
                var args = new[] {"A", "B"};
                BadTestApp.XMain(args);
            });
        }
    }
}
