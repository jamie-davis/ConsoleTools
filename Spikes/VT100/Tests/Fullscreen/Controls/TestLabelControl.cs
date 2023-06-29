using TestConsoleLib.Testing;
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
            var textBox = new LabelControl();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("1234567890");
            var data = new object[] {};
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }

    }
}