using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Properties;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;
using DescriptionAttribute = ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes.DescriptionAttribute;

// ReSharper disable ClassNeverInstantiated.Global

namespace ConsoleToolkitTests.ApplicationStyles
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestConsoleApplication
    {
        private ConsoleInterfaceForTesting _consoleOut;

        #region Types for test

        public class TestApp : ConsoleApplication
        {
            public static TestApp LastTestApp { get; set; }
            public bool Initialised { get; set; }
            public bool TestOptValue { get; set; }

            [Command]
            public class Command
            {
                [Option]
                public bool TestOpt { get; [UsedImplicitly] set; }

                [Option]
                public bool Fail { get; [UsedImplicitly] set; }
            }

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
                Initialised = true;

                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public void HandleCommand(Command c)
            {
                TestOptValue = c.TestOpt;
                if (c.Fail)
                    Environment.ExitCode = 100;
            }

            protected override void OnCommandSuccess()
            {
                CommandSuccessCalled = true;
                base.OnCommandSuccess();
            }

            protected override void OnCommandFailure()
            {
                CommandFailureCalled = true;
                base.OnCommandFailure();
            }

            public bool CommandSuccessCalled { get; set; }
            public bool CommandFailureCalled { get; set; }
        }

        public class MultipleCommandHandlerApp : ConsoleApplication
        {
            [Command]
            public class Command
            {
            }

            [CommandHandler(typeof(Command))]
            public class Handler
            {
                public void Handle(Command c)
                {
                }
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute<MultipleCommandHandlerApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public void CommandHandler(Command command)
            { }
        }

        public class MultipleCommandApp : ConsoleApplication
        {
            [Command]
            public class Command
            {
            }

            [Command]
            public class Command2
            {
            }

            [CommandHandler(typeof(Command))]
            public class Handler
            {
                public void Handle(Command c)
                {
                }
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute<MultipleCommandApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public void Command2Handler(Command2 command)
            { }
        }

        public class HelpApp : ConsoleApplication
        {
            [Command]
            [Description("A test application that only exists to check that this text is displayed automatically as part of the help information.")]
            public class Command
            {
                [Option("h", ShortCircuit = true)]
                [Description("Display this help text.")]
                public bool Help { get; [UsedImplicitly] set; }
            }

            [CommandHandler(typeof(Command))]
            public class Handler
            {
                public void Handle(Command c)
                {
                }
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute<HelpApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
                HelpOption<Command>(c => c.Help);
            }
        }

        public class InvalidHelpApp : ConsoleApplication
        {
            [Command]
            public class Options
            {
                
            }

            public class Command
            {
                public bool Help { get; [UsedImplicitly] set; }
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute<InvalidHelpApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
                HelpOption<Command>(c => c.Help);
            }
        }

        public class SelfHandledCommandApp : ConsoleApplication
        {
            [Command]
            public class Options
            {
                [CommandHandler]
                public void Handle(IConsoleAdapter console)
                {
                    console.Write("output");
                }
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute<SelfHandledCommandApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }
        }

        public class HandlerClassApp : ConsoleApplication
        {
            [Command]
            public class Options
            {
            }

            [CommandHandler(typeof(Options))]
            public class HandlerClass
            {
                public void Handle(IConsoleAdapter console, Options command, IErrorAdapter error)
                {
                    console.WriteLine("Text from handler class.");
                    error.WriteLine("Error text");
                }
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute<HandlerClassApp>(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }
        }

        public class CustomInjectionApp : ConsoleApplication
        {
            [Command]
            public class Options
            {
            }

            public class CustomObject
            {
                public string Message { get; set; }
            }

            [CommandHandler(typeof(Options))]
            public class HandlerClass
            {
                public void Handle(IConsoleAdapter console, IErrorAdapter error, Options command, CustomObject custom)
                {
                    console.WriteLine("Custom string is \"{0}\"", custom.Message);
                }
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute<CustomInjectionApp>(args);
            }

            protected override void Initialise()
            {
                RegisterInjectionInstance(new CustomObject { Message = "Custom message" });
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            _consoleOut = new ConsoleInterfaceForTesting();
            Environment.ExitCode = 0;
        }

        [TearDown]
        public void TearDown()
        {
            Toolkit.GlobalReset();
        }

        [Test]
        public void InitialiseIsCalled()
        {
            UnitTestAppRunner.Run<TestApp>();
            Assert.That(TestApp.LastTestApp.Initialised);
        }

        [Test]
        public void CommandIsExecuted()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "-TestOpt" }, new RedirectedConsole(ConsoleStream.Out));
            Assert.That(TestApp.LastTestApp.TestOptValue, Is.True);
        }

        [Test]
        public void StaticParsingConventionsAreUsed()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppRunner.Run<TestApp>(new[] { "/TestOpt" });
            Assert.That(TestApp.LastTestApp.TestOptValue, Is.True);
        }

        [Test, ExpectedException(typeof(MultipleHandlersForCommand))]
        public void ApplicationWithMultipleCommandHandlersWillNotInitialise()
        {
            UnitTestAppRunner.Run<MultipleCommandHandlerApp>(new string[] { });
        }

        [Test, ExpectedException(typeof(MultipleCommandsInvalid))]
        public void ApplicationWithMultipleCommandsWillNotInitialise()
        {
            UnitTestAppRunner.Run<MultipleCommandApp>(new string[] { });
        }

        [Test]
        public void HelpIsProvidedWithIndicatedCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppRunner.Run<HelpApp>(new[] { "/h" }, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Test, ExpectedException(typeof(HelpCommandMustBePartOfConfiguration))]
        public void HelpCommandTypeMustBeAConfiguredCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppRunner.Run<InvalidHelpApp>(new[] { "/h" }, _consoleOut);
        }

        [Test]
        public void SelfHandledCommandIsExecuted()
        {
            UnitTestAppRunner.Run<SelfHandledCommandApp>(new string[] {}, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Test]
        public void HandlerClassCommandIsExecuted()
        {
            UnitTestAppRunner.Run<HandlerClassApp>(new string[] {}, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Test]
        public void CustomInstanceCanBeInjectedIntoHandler()
        {
            UnitTestAppRunner.Run<CustomInjectionApp>(new string[] {}, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Test]
        public void OnCommandSuccessIsCalledAfterSuccessfulRun()
        {
            UnitTestAppRunner.Run<TestApp>(new string[] {}, _consoleOut);
            Assert.That(TestApp.LastTestApp.CommandSuccessCalled, Is.True);
        }

        [Test]
        public void OnCommandSuccessIsNotCalledAfterFailedRun()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "-Fail" }, _consoleOut);
            Assert.That(TestApp.LastTestApp.CommandSuccessCalled, Is.False);
        }

        [Test]
        public void OnCommandFailureIsCalledAfterFailedRun()
        {
            UnitTestAppRunner.Run<TestApp>(new[] { "-Fail" }, _consoleOut);
            Console.WriteLine(_consoleOut.GetBuffer());
            Assert.That(TestApp.LastTestApp.CommandFailureCalled, Is.True);
        }

        [Test]
        public void OnCommandFailureIsNotCalledAfterSuccesfulRun()
        {
            UnitTestAppRunner.Run<TestApp>(new string[] {}, _consoleOut);
            Assert.That(TestApp.LastTestApp.CommandFailureCalled, Is.False);
        }

    }
}