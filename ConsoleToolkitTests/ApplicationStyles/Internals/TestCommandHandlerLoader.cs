using System;
using System.Linq;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ApplicationStyles.Internals
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestCommandHandlerLoader
    {
        private Type[] _commandTypes;
        private MethodParameterInjector _injector;

        #region Types for test
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        // ReSharper disable IteratorMethodResultIsIgnored

        [Command]
        private class Command
        {

        }

        [Command]
        private class Command2
        {

        }

        [CommandHandler]
        private class Handler
        {
            public void Handle(Command command) { }
        }

        [CommandHandler]
        private class Handler2
        {
            public void Handle(Command command) { }
            public void Handle(Command2 command) { }
        }

        [CommandHandler]
        private class Handler3
        {
        }

        [CommandHandler]
        private class Handler4
        {
            public void Handle(Command command) { }

            public Handler4(string param)
            {
                
            }
        }

        [CommandHandler(typeof(Command))]
        private class Handler5
        {
            public void Handle1(Command command) { }
            public void Handle2(Command2 command) { }
        }

        private class Handler6
        {
            public void Handle1(Command command) { }
            public void Handle1B(Command command) { }
            public void Handle2(Command2 command) { }
        }

        [CommandHandler(typeof(Command))]
        private class Handler7
        {
            public void Handle(Handler5 five, Handler handler, Command command) { }
        }

        private class HandlerMethodTest
        {
            public void Handle(Handler5 five, Handler handler, Command command) { }
        }

        [Command]
        class SelfHandlingCommand
        {
            [CommandHandler]
            public void Handle()
            {
                
            }
        }

        [Command]
        class SelfMultiHandlingCommand
        {
            [CommandHandler]
            public void Handle()
            {
                
            }

            [CommandHandler]
            public void Handle2()
            {
                
            }
        }

        [Command]
        class SelfHandlingCommandWithBadParameter
        {
            [CommandHandler]
            public void Handle(Handler7 notImportant)
            {
                
            }
        }

        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local
        #endregion

        [SetUp]
        public void SetUp()
        {
            _commandTypes = new[] {typeof (Command), typeof(Command2)};
            //The handlers in the injector are just doubling up as parameter fodder - the specific types in the injector do not matter a jot.
            _injector = new MethodParameterInjector(new object[]
                                                        {
                                                            this, new Handler(), new Handler2(), new Handler3(),
                                                            new Handler4(""), new Handler5(), new Handler6(),
                                                        });
        }

        [Test]
        public void CommandHandlerClassIsLoaded()
        {
            var handler = CommandHandlerLoader.Load(typeof (Handler), _commandTypes, _injector);
            Assert.That(handler.CommandType, Is.EqualTo(typeof(Command)));
        }

        [Test]
        public void SpecificCommandHandlerClassIsLoaded()
        {
            var handler = CommandHandlerLoader.Load(typeof(Handler5), _commandTypes, _injector);
            Assert.That(handler.CommandType, Is.EqualTo(typeof(Command)));
        }

        [Test, ExpectedException(typeof (AmbiguousCommandHandler))]
        public void MultiHandlerRoutinesThrow()
        {
            CommandHandlerLoader.Load(typeof(Handler2), _commandTypes, _injector);
        }

        [Test, ExpectedException(typeof (NoCommandHandlerMethodFound))]
        public void MissingHandlerRoutineThrows()
        {
            CommandHandlerLoader.Load(typeof(Handler3), _commandTypes, _injector);
        }

        [Test, ExpectedException(typeof(CommandHandlerDoesNotHaveAttribute))]
        public void NoHandlerAttributeThrows()
        {
            CommandHandlerLoader.Load(typeof(Command), _commandTypes, _injector);
        }

        [Test, ExpectedException(typeof(CommandHandlerMustHaveDefaultConstructor))]
        public void NoDefaultConstructorThrows()
        {
            CommandHandlerLoader.Load(typeof(Handler4), _commandTypes, _injector);
        }

        [Test]
        public void HandlerMethodsAreLoaded()
        {
            var handlers = CommandHandlerLoader.LoadHandlerMethods(typeof(Handler5), _commandTypes, _injector);
            Assert.That(handlers.Select(h => h.CommandType.Name).JoinWith(","), Is.EqualTo("Command,Command2"));
        }
        // ReSharper disable ReturnValueOfPureMethodIsNotUsed

        [Test, ExpectedException(typeof(MultipleHandlersForCommand))]
        public void MultipleHandlerMethodsForSameCommandThrows()
        {
            CommandHandlerLoader.LoadHandlerMethods(typeof(Handler6), _commandTypes, _injector).Count();
        }

        [Test]
        public void HandlersMayAcceptTheCommandTypeAsAnyParameter()
        {
            var handler = CommandHandlerLoader.Load(typeof(Handler7), _commandTypes, _injector);
            Assert.That(handler.CommandType, Is.EqualTo(typeof(Command)));
        }

        [Test]
        public void HandlerMethodsMayAcceptTheCommandTypeAsAnyParameter()
        {
            var handler = CommandHandlerLoader.LoadHandlerMethods(typeof(HandlerMethodTest), _commandTypes, _injector)
                .First();
            Assert.That(handler.CommandType, Is.EqualTo(typeof(Command)));
        }

        [Test]
        public void CommandsMayContainTheirOwnHandlerMethod()
        {
            var commandTypes = new[] {typeof (SelfHandlingCommand)};
            var handler = CommandHandlerLoader.LoadHandlerMethods(typeof(SelfHandlingCommand), commandTypes, _injector)
                .First();
            Assert.That(handler.CommandType, Is.EqualTo(typeof(SelfHandlingCommand)));
        }

        [Test, ExpectedException(typeof(CommandsMayOnlyDeclareOneHandlerMethod))]
        public void SelfHandlingCommandsMayOnlyDeclareOneHandler()
        {
            var commandTypes = new[] { typeof(SelfMultiHandlingCommand) };
            var methods = CommandHandlerLoader.LoadHandlerMethods(typeof (SelfMultiHandlingCommand), commandTypes, _injector);
            foreach (var commandType in methods)
            {
                Console.WriteLine(commandType.CommandType);
            }
        }

        [Test, ExpectedException(typeof(CommandHandlerMethodHasUnsupportedParameter))]
        public void SelfHandlingCommandsMayOnlyAcceptSupportedParameterTypes()
        {
            var commandTypes = new[] { typeof(SelfHandlingCommandWithBadParameter) };
            var methods = CommandHandlerLoader.LoadHandlerMethods(typeof(SelfHandlingCommandWithBadParameter), commandTypes, _injector);
            foreach (var commandType in methods)
            {
                Console.WriteLine(commandType.CommandType);
            }
        }
        // ReSharper restore IteratorMethodResultIsIgnored
        // ReSharper restore ReturnValueOfPureMethodIsNotUsed
    }
}