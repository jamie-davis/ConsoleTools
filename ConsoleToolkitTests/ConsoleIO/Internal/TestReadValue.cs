using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.TestingUtilities;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [UseReporter(typeof(CustomReporter))]
    public class TestReadValue
    {
        private ConsoleInterfaceForTesting _interface;
        private ConsoleAdapter _adapter;

        private static readonly PropertyInfo StringProp = typeof(TestReadInputItem).GetProperty("StringVal");
        private static readonly PropertyInfo IntProp = typeof(TestReadInputItem).GetProperty("IntVal");

        public string StringVal { get; set; }
        public int IntVal { get; set; }
        public TestReadValue()
        {
            _interface = new ConsoleInterfaceForTesting();
            _adapter = new ConsoleAdapter(_interface);

            StringVal = null;
            IntVal = 0;
        }

        private TextReader MakeStream(IEnumerable<string> input)
        {
            return new StringReader(input.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void StringCanBeRead()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "text" }));
            var item = new InputItem
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string)
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.Equal("text", value);
        }

        [Fact]
        public void ValidReadReturnsTrue()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "text" }));
            var item = new InputItem
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string)
            };

            //Act
            object value;
            var result = ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void InvalidReadReturnsFalse()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "text" }));
            _interface.InputIsRedirected = true;
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int)
            };

            //Act
            object value;
            var result = ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void TextIsConvertedToRequiredType()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "45" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int)
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.Equal(45, value);
        }

        [Fact]
        public void ErrorIsDisplayedWhenInputIsInvalid()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "text" }));
            _interface.InputIsRedirected = true;
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Approvals.Verify(_interface.GetBuffer());
        }

        [Fact]
        public void OptionIsSelected()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "C" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                    .Option(100, "B", "First")
                    .Option(200, "C", "Second")
                    .Option(300, "D", "Third")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.Equal(200, value);
        }

        [Fact]
        public void OptionInputIsNotCaseSensitive()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "c" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                    .Option(100, "B", "First")
                    .Option(200, "C", "Second")
                    .Option(300, "D", "Third")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.Equal(200, value);
        }

        [Fact]
        public void OptionsCanBeDifferentiatedByCase()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "a", "A" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                    .Option(100, "a", "First")
                    .Option(200, "A", "Second")
            };

            //Act
            object value1, value2;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value1);
            ReadValue.UsingReadLine(item, _interface, _adapter, out value2);

            //Assert
            Assert.Equal("Value1 = 100, Value2 = 200", string.Format("Value1 = {0}, Value2 = {1}", value1, value2));
        }

        [Fact]
        public void ValidationErrorMessageIsDisplayed()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "10" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Approvals.Verify(_interface.GetBuffer());
        }

        [Fact]
        public void ValidationErrorCausesFalseReturn()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "10" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            //Act
            object value;
            var result = ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidationPassReturnsTrue()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "11" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            //Act
            object value;
            var result = ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidationPassSetsValue()
        {
            //Arrange
            _interface.SetInputStream(MakeStream(new[] { "11" }));
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            //Act
            object value;
            ReadValue.UsingReadLine(item, _interface, _adapter, out value);

            //Assert
            Assert.Equal(11, value);
        }
    }
}