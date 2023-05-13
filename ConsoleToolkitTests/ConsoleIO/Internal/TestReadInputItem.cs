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
    [UseReporter(typeof (CustomReporter))]
    public class TestReadInputItem
    {
        private ConsoleInterfaceForTesting _interface;
        private ConsoleAdapter _adapter;

        private static readonly PropertyInfo StringProp = typeof (TestReadInputItem).GetProperty("StringVal");
        private static readonly PropertyInfo IntProp = typeof (TestReadInputItem).GetProperty("IntVal");
        private TextReader _goodStream;
        private TextReader _selectStream;
        private TextReader _stringOnlyStream;
        private TextReader _validationStream;

        public string StringVal { get; set; }
        public int IntVal { get; set; }
        public TestReadInputItem()
        {
            _interface = new ConsoleInterfaceForTesting();
            _adapter = new ConsoleAdapter(_interface);

            StringVal = null;
            IntVal = 0;

            _goodStream = MakeStream(new [] {"text", "45"});
            _stringOnlyStream = MakeStream(new [] {"text"});
            _selectStream = MakeStream(new [] {"bad", "2", "C"});
            _validationStream = MakeStream(new[] { "2", "10", "11" });
        }

        private TextReader MakeStream(IEnumerable<string> input)
        {
            return new StringReader(input.JoinWith(Environment.NewLine));
        }

        [Fact]
        public void StringCanBeRead()
        {
            _interface.SetInputStream(_goodStream);
            var item = new InputItem
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string)
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Assert.Equal("text", item.Value);
        }

        [Fact]
        public void ValidReadReturnsTrue()
        {
            _interface.SetInputStream(_stringOnlyStream);
            var item = new InputItem
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string)
            };

            ReadInputItem.GetValue(item, _interface, _adapter).Should().BeTrue();
        }

        [Fact]
        public void InvalidReadReturnsFalse()
        {
            _interface.SetInputStream(_stringOnlyStream);
            _interface.InputIsRedirected = true;
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int)
            };

            ReadInputItem.GetValue(item, _interface, _adapter).Should().BeFalse();
        }

        [Fact]
        public void TextIsConvertedToRequiredType()
        {
            _interface.SetInputStream(_goodStream);
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int)
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Assert.Equal(45, item.Value);
        }

        [Fact]
        public void PromptIsDisplayed()
        {
            _interface.SetInputStream(_goodStream);
            var item = new InputItem
            {
                Name = "StringVal",
                Property = StringProp,
                Type = typeof(string),
                ReadInfo = Read.String().Prompt("prompt")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }

        [Fact]
        public void ErrorIsDisplayedWhenInputIsInvalid()
        {
            _interface.SetInputStream(_goodStream);
            _interface.InputIsRedirected = true;
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }

        [Fact]
        public void InteractiveInputContinuesUntilGoodInputReceived()
        {
            _interface.SetInputStream(_goodStream);
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }

        [Fact]
        public void OptionsCanBeSpecified()
        {
            _interface.SetInputStream(_selectStream);
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                .Option(1, "1", "First")
                .Option(2, "2", "Second")
                .Option(3, "3", "Third")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }

        [Fact]
        public void OptionsCanBeDisplayedAsAMenu()
        {
            _interface.SetInputStream(_selectStream);
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Prompt("prompt")
                .Option(1, "1", "First")
                .Option(2, "2", "Second")
                .Option(3, "3", "Third")
                .AsMenu("Select one")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }

        [Fact]
        public void OptionIsSelected()
        {
            _interface.SetInputStream(_selectStream);
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

            ReadInputItem.GetValue(item, _interface, _adapter);
            var value = ((Read<int>)item.Value).Value;
            Assert.Equal(200, value);
        }

        [Fact]
        public void ValidationsAreApplied()
        {
            _interface.SetInputStream(_validationStream);
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            var value = ((Read<int>)item.Value).Value;
            Assert.Equal(11, value);
        }

        [Fact]
        public void ValidationErrorMessageIsDisplayed()
        {
            _interface.SetInputStream(_validationStream);
            var item = new InputItem
            {
                Name = "IntVal",
                Property = IntProp,
                Type = typeof(int),
                ReadInfo = Read.Int().Validate(i => i > 10, "Value must be greater than 10")
            };

            ReadInputItem.GetValue(item, _interface, _adapter);
            Approvals.Verify(_interface.GetBuffer());
        }
    }
}