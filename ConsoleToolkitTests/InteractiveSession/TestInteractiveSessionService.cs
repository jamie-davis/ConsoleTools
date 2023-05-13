using System;
using System.IO;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.InteractiveSession;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.InteractiveSession
{
    [UseReporter(typeof(CustomReporter))]
    public class TestInteractiveSessionService
    {
        #region Types for test

        [Command("start")]
        public class StartCommand
        {
            [Positional(DefaultValue = null)]
            public string Prompt { get; set; }

            [CommandHandler]
            public void Handle(IInteractiveSessionService service, IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
                if (Prompt != null)
                    service.SetPrompt(Prompt);
                service.BeginSession();
            }
        }

        [Command("one")]
        public class Command1
        {
            [CommandHandler]
            public void Handle(IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
            }
        }

        [Command("two")]
        public class Command2
        {
            [CommandHandler]
            public void Handle(IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
            }
        }

        [Command("three")]
        public class Command3
        {
            [CommandHandler]
            public void Handle(IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
            }
        }

        [Command("fail")]
        public class CommandFail
        {
            [CommandHandler]
            public void Handle(IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
                Environment.ExitCode = 100;
            }
        }

        [Command]
        public class SetPromptCommand
        {
            [Positional]
            public string Prompt { get; set; }

            [CommandHandler]
            public void Handle(IInteractiveSessionService service)
            {
                service.SetPrompt(Prompt);
            }
        }

        [Command("exit")]
        public class ExitCommand
        {
            [CommandHandler]
            public void Handle(IInteractiveSessionService service, IConsoleAdapter console)
            {
                console.WrapLine(GetType().Name);
                service.EndSession();
            }
        }

        #endregion

        private ConsoleInterfaceForTesting _testConsole;
        private IConsoleAdapter _console;
        private IErrorAdapter _error;
        private FakeApplication _app;
        public TestInteractiveSessionService()
        {
            _testConsole = new ConsoleInterfaceForTesting();
            _console = new ConsoleAdapter(_testConsole);
            _error = new ErrorAdapter(_testConsole, "ERROR: ");
            _app = FakeApplication.MakeFakeApplication(_console, _error, typeof(StartCommand), typeof(Command1), typeof(Command2), typeof(Command3), typeof(CommandFail), typeof(ExitCommand), typeof(SetPromptCommand));
        }

        [Fact]
        public void InteractiveSessionRunsCommands()
        {
            //Arrange
            const string data = @"one
two
exit";
            using (var s = new StringReader(data))
            {
                _testConsole.SetInputStream(s);

                //Act
                UnitTestAppRunner.Run(_app, new [] { "start" }, _testConsole);

                //Assert
                _console.WrapLine("<Session exited>");
                ApprovalTests.Approvals.Verify(_testConsole.GetBuffer());
            }
        }

        [Fact]
        public void InteractiveSessionHandlesErrors()
        {
            //Arrange
            const string data = @"error
two
exit";
            using (var s = new StringReader(data))
            {
                _testConsole.SetInputStream(s);

                //Act
                UnitTestAppRunner.Run(_app, new [] { "start" }, _testConsole);

                //Assert
                _console.WrapLine("<Session exited>");
                Approvals.Verify(_testConsole.GetBuffer());
            }
        }

        [Fact]
        public void InteractiveSessionHandlesBlankInput()
        {
            //Arrange
            const string data = @"
";
            using (var s = new StringReader(data))
            {
                _testConsole.SetInputStream(s);

                //Act
                UnitTestAppRunner.Run(_app, new [] { "start" }, _testConsole);

                //Assert
                _console.WrapLine("<Session exited>");
                Approvals.Verify(_testConsole.GetBuffer());
            }
        }

        [Fact]
        public void CommandFailuresDoNotEndSession()
        {
            var originalExitCode = Environment.ExitCode;
            try
            {
                //Arrange
                const string data = @"fail
two
";
                using (var s = new StringReader(data))
                {
                    _testConsole.SetInputStream(s);

                    //Act
                    UnitTestAppRunner.Run(_app, new[] {"start"}, _testConsole);

                    //Assert
                    _console.WrapLine("<Session exited>");
                    _console.WrapLine($"Environment.ExitCode = {Environment.ExitCode}");
                    Approvals.Verify(_testConsole.GetBuffer());
                }
            } 
            finally
            {
                Environment.ExitCode = originalExitCode;
            }
        }

        [Fact]
        public void PromptCanBeSetBeforeSessionStarts()
        {
            //Arrange
            const string data = @"one
two
exit";
            using (var s = new StringReader(data))
            {
                _testConsole.SetInputStream(s);

                //Act
                UnitTestAppRunner.Run(_app, new [] { "start", "##>" }, _testConsole);

                //Assert
                _console.WrapLine("<Session exited>");
                Approvals.Verify(_testConsole.GetBuffer());
            }
        }

        [Fact]
        public void PromptCanBeChangedDuringASession()
        {
            //Arrange
            const string data = @"one
setprompt ""NewPrompt:""
exit";
            using (var s = new StringReader(data))
            {
                _testConsole.SetInputStream(s);

                //Act
                UnitTestAppRunner.Run(_app, new [] { "start", ":" }, _testConsole);

                //Assert
                _console.WrapLine("<Session exited>");
                Approvals.Verify(_testConsole.GetBuffer());
            }
        }

    }
}
