﻿using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;
using DescriptionAttribute = ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes.DescriptionAttribute;

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
                public bool TestOpt { get; set; }
            }

            public TestApp()
            {
                LastTestApp = this;
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute(args);
            }

            protected override void Initialise()
            {
                Initialised = true;

                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public void HandleCommand(Command c)
            {
                TestOptValue = c.TestOpt;
            }
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
                Toolkit.Execute(args);
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
                Toolkit.Execute(args);
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
                public bool Help { get; set; }
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
                Toolkit.Execute(args);
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
                public bool Help { get; set; }
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute(args);
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
                Toolkit.Execute(args);
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
                public void Handle(IConsoleAdapter console, Options command)
                {
                    console.WriteLine("Text from handler class.");
                }
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            _consoleOut = new ConsoleInterfaceForTesting();
        }

        [TearDown]
        public void TearDown()
        {
            Toolkit.GlobalReset();
        }

        [Test]
        public void InitialiseIsCalled()
        {
            UnitTestAppUtils.Run<TestApp>();
            Assert.That(TestApp.LastTestApp.Initialised);
        }

        [Test]
        public void CommandIsExecuted()
        {
            UnitTestAppUtils.Run<TestApp>(new[] { "-TestOpt" }, new RedirectedConsole());
            Assert.That(TestApp.LastTestApp.TestOptValue, Is.True);
        }

        [Test]
        public void StaticParsingConventionsAreUsed()
        {
            Toolkit.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppUtils.Run<TestApp>(new[] { "/TestOpt" });
            Assert.That(TestApp.LastTestApp.TestOptValue, Is.True);
        }

        [Test, ExpectedException(typeof(MultipleHandlersForCommand))]
        public void ApplicationWithMultipleCommandHandlersWillNotInitialise()
        {
            UnitTestAppUtils.Run<MultipleCommandHandlerApp>(new string[] { });
        }

        [Test, ExpectedException(typeof(MultipleCommandsInvalid))]
        public void ApplicationWithMultipleCommandsWillNotInitialise()
        {
            UnitTestAppUtils.Run<MultipleCommandApp>(new string[] { });
        }

        [Test]
        public void HelpIsProvidedWithIndicatedCommand()
        {
            Toolkit.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppUtils.Run<HelpApp>(new[] { "/h" }, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Test, ExpectedException(typeof(HelpCommandMustBePartOfConfiguration))]
        public void HelpCommandTypeMustBeAConfiguredCommand()
        {
            Toolkit.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppUtils.Run<InvalidHelpApp>(new[] { "/h" }, _consoleOut);
        }

        [Test]
        public void SelfHandledCommandIsExecuted()
        {
            UnitTestAppUtils.Run<SelfHandledCommandApp>(new string[] {}, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

        [Test]
        public void HandlerClassCommandIsExecuted()
        {
            UnitTestAppUtils.Run<HandlerClassApp>(new string[] {}, _consoleOut);
            Approvals.Verify(_consoleOut.GetBuffer());
        }

    }
}