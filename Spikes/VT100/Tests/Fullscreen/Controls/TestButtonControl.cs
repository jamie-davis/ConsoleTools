using System;
using FluentAssertions;
using TestConsoleLib.Testing;
using VT100.Attributes;
using VT100.FullScreen.Controls;
using VT100.Tests.TestUtilities;
using Xunit;

namespace VT100.Tests.Fullscreen.Controls
{
    public class TestButtonControl
    {
        [Fact]
        public void ButtonIsDisplayed()
        {
            //Arrange
            var button = new ButtonControl();
            button.AcceptConfig(new ButtonAttribute("test")); 
            button.Position(0,0,6,5);
            var data = new object[] {};
            var testRig = new ControlTestRig<string>(button, _ => true, data, width: 6, height:5);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }

        [Fact]
        public void CaptionIsTruncated()
        {
            //Arrange
            var button = new ButtonControl();
            button.AcceptConfig(new ButtonAttribute("long text")); 
            button.Position(0,0,6,1);
            var data = new object[] {};
            var testRig = new ControlTestRig<string>(button, _ => true, data, width: 6, height: 1);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }

        [Fact]
        public void MethodIsExecutedOnSpace()
        {
            //Arrange
            var button = new ButtonControl();
            button.AcceptConfig(new ButtonAttribute("Button")); 
            button.Position(0,0,8,1);
            var data = new object[] { ' ' };
            var pushed = false;
            var testRig = new ControlTestRig<string>(button, _ => pushed = true, data, width: 8, height: 1);
            
            //Act
            testRig.RunTest();

            //Assert
            pushed.Should().Be(true);
        }

        [Fact]
        public void MethodIsNotExecutedOnTab()
        {
            //Arrange
            var button = new ButtonControl();
            button.AcceptConfig(new ButtonAttribute("Button")); 
            button.Position(0,0,8,1);
            var data = new object[] { '\t' };
            var pushed = false;
            var testRig = new ControlTestRig<string>(button, _ => pushed = true, data, width: 8, height: 1);
            
            //Act
            testRig.RunTest();

            //Assert
            pushed.Should().Be(false);
        }

        [Fact]
        public void MinimumSizeIsComputed()
        {
            //Arrange
            var button = new ButtonControl();
            button.AcceptConfig(new ButtonAttribute("Button")); 
           
            //Act
            var result = button.GetMinSize();

            //Assert
            result.Should().Be((8, 1));
        }

        [Fact]
        public void NoCaptionButtonSizeIsComputed()
        {
            //Arrange
            var button = new ButtonControl();
           
            //Act
            var result = button.GetMinSize();

            //Assert
            result.Should().Be((2, 1));
        }

        [Fact]
        public void MaximumSizeIsComputed()
        {
            //Arrange
            var button = new ButtonControl();
            button.AcceptConfig(new ButtonAttribute("Button")); 
           
            //Act
            var result = button.GetMaxSize(20, 20);

            //Assert
            result.Should().Be((8, 1));
        }
    }
}