using ApprovalTests;
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
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandDrivenApplication
    {
        private ConsoleInterfaceForTesting _console;

        #region Types for test
#pragma warning disable 649

        public class TestApp : CommandDrivenApplication
        {
            public static TestApp LastTestApp { get; set; }
            public bool Initialised { get; set; }
            public bool TestOptValue { get; set; }

            [Command("c")]
            public class Command
            {
                [Option]
                public bool TestOpt { get; set; }
            }

            [Command("d")]
            public class SelfCommand
            {
                [Positional]
                public string Pos { get; set; }

                [Option]
                public string TestOpt { get; set; }

                [CommandHandler]
                public void Handle(IConsoleAdapter console)
                {
                    console.WrapLine("Self handled command.");
                    console.WrapLine("Positional parameter: {0}", Pos);
                    console.WrapLine("Option TestOpt      : {0}", TestOpt);

                }
            }

            [Command("e")]
            public class ClassHandledCommand
            {
                [Positional]
                public string Pos { get; set; }

                [Option]
                public string TestOpt { get; set; }
            }

            [CommandHandler]
            public class ClassHandledHandler
            {
                public void Handle(IConsoleAdapter console, ClassHandledCommand cmd)
                {
                    console.WrapLine("Class handled command.");
                    console.WrapLine("Positional parameter: {0}", cmd.Pos);
                    console.WrapLine("Option TestOpt      : {0}", cmd.TestOpt);

                }
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
        
        public class DuplicateCommandHandlerApp : CommandDrivenApplication
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

            [CommandHandler(typeof(Command2))]
            public class Handler2
            {
                public void Handle(Command2 c)
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
            {}
        }
        
        public class HelpCommandApp : CommandDrivenApplication
        {
            [Command]
            [Description("A test command that does nothing. This text is here just to add help information.")]
            public class Command
            {
            }

            [Command]
            [Description("Get help on the commands supported by this application.")]
            private class HelpMe
            {
                [Positional(0, DefaultValue = null)]
                [Description("Optional command on which help is required.")]
                public string Subject;
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
                HelpCommand<HelpMe>(h => h.Subject);
            }
        }
        
        public class InvalidHelpCommandApp : CommandDrivenApplication
        {
            [Command]
            public class Command
            {
            }

            private class HelpMe
            {
            }

            public static void Main(string[] args)
            {
                Toolkit.Execute(args);
            }

            protected override void Initialise()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
                HelpCommand<HelpMe>(h => null);
            }
        }

#pragma warning restore 649
        #endregion

        [SetUp]
        public void SetUp()
        {
            _console = new ConsoleInterfaceForTesting();
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
            UnitTestAppUtils.Run<TestApp>(new[] { "C", "-TestOpt" }, new RedirectedConsole());
            Assert.That(TestApp.LastTestApp.TestOptValue, Is.True);
        }

        [Test]
        public void SelfHandledCommandIsExecuted()
        {
            UnitTestAppUtils.Run<TestApp>(new[] { "d", "positional", "-TestOpt:opt" }, _console);
            Approvals.Verify(_console.GetBuffer());
        }

        [Test]
        public void ClassHandledCommandIsExecuted()
        {
            UnitTestAppUtils.Run<TestApp>(new[] { "e", "positional", "-TestOpt:opt" }, _console);
            Approvals.Verify(_console.GetBuffer());
        }

        [Test]
        public void StaticParsingConventionsAreUsed()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppUtils.Run<TestApp>(new [] {"c","/TestOpt"});
            Assert.That(TestApp.LastTestApp.TestOptValue, Is.True);
        }

        [Test, ExpectedException(typeof(MultipleHandlersForCommand))]
        public void ApplicationWithDuplicateCommandHandlersWillNotInitialise()
        {
            UnitTestAppUtils.Run<DuplicateCommandHandlerApp>(new string[] {});
        }

        [Test]
        public void HelpIsProvidedWithIndicatedCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppUtils.Run<HelpCommandApp>(new[] { "helpme" }, _console);
            Approvals.Verify(_console.GetBuffer());
        }

        [Test]
        public void CommandLevelHelpIsProvidedWithIndicatedCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppUtils.Run<HelpCommandApp>(new[] { "helpme", "helpme" }, _console);
            Approvals.Verify(_console.GetBuffer());
        }

        [Test,ExpectedException(typeof(HelpCommandMustBePartOfConfiguration))]
        public void HelpCommandTypeMustBeAConfiguredCommand()
        {
            Toolkit.Options.ParsingConventions = CommandLineParserConventions.MsDosConventions;
            UnitTestAppUtils.Run<InvalidHelpCommandApp>(new[] { "helpme" }, _console);
        }
    }
}
