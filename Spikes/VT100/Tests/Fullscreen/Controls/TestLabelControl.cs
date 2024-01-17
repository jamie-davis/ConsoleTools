using System;
using FluentAssertions;
using TestConsoleLib.Testing;
using VT100.Attributes;
using VT100.FullScreen.Controls;
using VT100.Tests.TestUtilities;
using Xunit;

namespace VT100.Tests.Fullscreen.Controls
{
    public class TestLabelControl
    {
        [Fact]
        public void ValueIsDisplayed()
        {
            //Arrange
            var label = new LabelControl();
            label.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test");
            var data = new object[] {};
            var testRig = new ControlTestRig<string>(label, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }

        [Fact]
        public void ValueIsTruncated()
        {
            //Arrange
            var label = new LabelControl();
            label.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("1234567890");
            var data = new object[] {};
            var testRig = new ControlTestRig<string>(label, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }

        [Fact]
        public void MinSizeForVariableLabelIsComputed()
        {
            //Arrange
            var label = new LabelControl();
            label.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("1234567890");
            var data = new object[] {};
            _ = new ControlTestRig<string>(label, valueWrapper, data);
            
            //Act
            var result = label.GetMinSize();

            //Assert
            result.Should().Be((1, 1));
        }

        [Fact]
        public void MinSizeIsComputed()
        {
            //Arrange
            var label = new LabelControl();
            label.AcceptConfig(new LabelAttribute("1234567890"));
            label.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("");
            var data = new object[] {};
            _ = new ControlTestRig<string>(label, valueWrapper, data);
            
            //Act
            var result = label.GetMinSize();

            //Assert
            result.Should().Be((1, 1));
        }

        [Fact]
        public void MaxSizeIsComputed()
        {
            //Arrange
            var label = new LabelControl();
            label.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("1234567890");
            var data = new object[] {};
            _ = new ControlTestRig<string>(label, valueWrapper, data);
            
            //Act
            var result = label.GetMaxSize(100,2);

            //Assert
            result.Should().Be((100, 1));
        }
    }
}