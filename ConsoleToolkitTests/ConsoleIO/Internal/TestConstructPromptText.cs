using System.Reflection;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestConstructPromptText
    {
        private ConsoleInterfaceForTesting _interface;
        public int IntVal { get; set; }
        public string StringVal { get; set; }

        private static readonly PropertyInfo StringProp = typeof(TestReadInputItem).GetProperty("StringVal");
        private static readonly PropertyInfo IntProp = typeof(TestReadInputItem).GetProperty("IntVal");

        [SetUp]
        public void SetUp()
        {
            _interface = new ConsoleInterfaceForTesting();

            StringVal = null;
            IntVal = 0;
        }

        [Test]
        public void PromptIsAutomaticallyDerivedFromPropertyName()
        {
            //Arrange
            var inputItem = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int)
            };

            //Act
            var result = ConstructPromptText.FromItem(inputItem);

            //Assert
            Assert.That(result, Is.EqualTo("Int Val: "));
        }

        [Test]
        public void PromptIsTakenFromPropertyInfo()
        {
            //Arrange
            var inputItem = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("My prompt")
            };

            //Act
            var result = ConstructPromptText.FromItem(inputItem);

            //Assert
            Assert.That(result, Is.EqualTo("My prompt: "));
        }

        [Test]
        public void PromptIncludesSelection()
        {
            //Arrange
            var inputItem = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Option(4, "A", "Four").Option(10, "B", "Ten")
            };

            //Act
            var result = ConstructPromptText.FromItem(inputItem);

            //Assert
            Assert.That(result, Is.EqualTo("Int Val [A-Four, B-Ten]: "));
        }

        [Test]
        public void PromptCanDisplaySelectionAsMenu()
        {
            //Arrange
            var inputItem = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Option(4, "A", "Four").Option(10, "B", "Ten")
                .AsMenu("Which value?")
            };

            //Act
            var result = ConstructPromptText.FromItem(inputItem);

            //Assert
            var expected = @"Which value?

A-Four
B-Ten

Int Val: ";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}