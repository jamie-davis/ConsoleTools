using System.Reflection;
using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof (CustomReporter))]
    public class TestConstructPromptText
    {
        private ConsoleInterfaceForTesting _interface;
        public int IntVal { get; set; }
        public string StringVal { get; set; }

        private static readonly PropertyInfo StringProp = typeof(TestReadInputItem).GetProperty("StringVal");
        private static readonly PropertyInfo IntProp = typeof(TestReadInputItem).GetProperty("IntVal");
        public TestConstructPromptText()
        {
            _interface = new ConsoleInterfaceForTesting();

            StringVal = null;
            IntVal = 0;
        }

        [Fact]
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
            Assert.Equal("Int Val: ", result);
        }

        [Fact]
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
            Assert.Equal("My prompt: ", result);
        }

        [Fact]
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
            Assert.Equal("Int Val [A-Four, B-Ten]: ", result);
        }

        [Fact]
        public void NoPropertyInfoGivesNoPromptText()
        {
            //Arrange
            var inputItem = new InputItem
            {
                Name = "IntVal",
                Property = null,
                Type = typeof(int)
            };

            //Act
            var result = ConstructPromptText.FromItem(inputItem);

            //Assert
            Assert.Equal(": ", result);
        }

        [Fact]
        public void NoPropertyInfoOnSelectionGivesSelectionText()
        {
            //Arrange
            var inputItem = new InputItem
            {
                Name = "IntVal",
                Property = null,
                Type = typeof(int),
                ReadInfo = Read.Int().Option(4, "A", "Four").Option(10, "B", "Ten")
            };

            //Act
            var result = ConstructPromptText.FromItem(inputItem);

            //Assert
            Assert.Equal("[A-Four, B-Ten]: ", result);
        }

        [Fact]
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
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NoPropertyInfoOnMenuGivesMenuText()
        {
            //Arrange
            var inputItem = new InputItem
            {
                Name = "IntVal",
                Property = null,
                Type = typeof(int),
                ReadInfo = Read.Int().Option(4, "A", "Four").Option(10, "B", "Ten").AsMenu("No Property")
            };

            //Act
            var result = ConstructPromptText.FromItem(inputItem);

            //Assert
            var expected = @"No Property

A-Four
B-Ten

: ";
            Assert.Equal(expected, result);
        }

    }
}