using ApprovalTests.Reporters;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.ApplicationStyles.Internals;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.Exceptions;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ApplicationStyles.Internals
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestHandlerParameterValidator
    {
        private MethodParameterInjector _injector;

        #region Types for test
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedParameter.Local
        // ReSharper disable IteratorMethodResultIsIgnored

        [Command]
        class SelfHandlingCommandWithBadParameter
        {
            [CommandHandler]
            public void Handle(ParamType notImportant)
            {

            }
        }

        private class ParamType
        {
        }

        // ReSharper restore UnusedParameter.Local
        // ReSharper restore UnusedMember.Local

        #endregion

        [SetUp]
        public void SetUp()
        {
            _injector = new MethodParameterInjector(new object[]
            {
                this
            });
        }

        [Test]
        public void CommandsMayOnlyAcceptSupportedParameterTypes()
        {
            //Arrange
            var commandTypes = new[] { typeof(SelfHandlingCommandWithBadParameter) };
            var methods = CommandHandlerLoader.LoadHandlerMethods(typeof(SelfHandlingCommandWithBadParameter), commandTypes, _injector);

            //Act/Assert
            Assert.Throws<CommandHandlerMethodHasUnsupportedParameter>(() => HandlerParameterValidator.ValidateHandlers(methods, _injector));
        }

        [Test]
        public void NoExceptionIsThrownIfAllParameterTypesAreValid()
        {
            //Arrange
            var commandTypes = new[] { typeof(SelfHandlingCommandWithBadParameter) };
            _injector.AddInstance(new ParamType());
            var methods = CommandHandlerLoader.LoadHandlerMethods(typeof(SelfHandlingCommandWithBadParameter), commandTypes, _injector);

            //Act - no assert is required, the test passes if the following does not throw.
            HandlerParameterValidator.ValidateHandlers(methods, _injector);
        }
    }
}
