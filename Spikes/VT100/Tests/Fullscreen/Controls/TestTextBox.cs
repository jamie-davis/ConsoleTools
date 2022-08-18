using FluentAssertions;
using VT100.FullScreen.Controls;
using VT100.Tests.Fakes;
using VT100.Tests.TestUtilities;
using VT100.Utilities.ReadConsole;
using TestConsoleLib.Testing;
using Xunit;

namespace VT100.Tests.Fullscreen.Controls
{
    public class TestTextBox
    {
        [Fact]
        public void TextEntryIsDisplayed()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test");
            var data = new object[] { 'A', ' ' };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void TextCanBeExtended()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("T");
            var data = new object[] { ResolvedCode.CursorForward, 'e', 's', 't' };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void TextIsPaddedToRequiredLength()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("T");
            var data = new object[] { ResolvedCode.CursorForward,ResolvedCode.CursorForward, 'T' };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void BackspaceDeletesCharactersBehindCursor()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test");
            var data = new object[] { ResolvedCode.End, ResolvedCode.CursorBackwards, ResolvedCode.Backspace };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void DeleteRemovesCharactersUnderCursor()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test");
            var data = new object[] { ResolvedCode.Home, ResolvedCode.CursorForward, ResolvedCode.Delete, ResolvedCode.Delete , ResolvedCode.Delete , ResolvedCode.Delete };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void BackspaceHasNoEffectAtTheStartOfTheEditBox()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test");
            var data = new object[] { ResolvedCode.Backspace };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void EndMovesCursorToEndOfText()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Tes");
            var data = new object[] { ResolvedCode.End, 't' };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void LongContentIsTruncatedForDisplay()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test long string");
            var data = new object[] {  };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void LongContentScrollsWhenCursorIsMovedForward()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test long string");
            var data = new object[] { ResolvedCode.CursorForward, ResolvedCode.CursorForward, ResolvedCode.CursorForward, ResolvedCode.CursorForward, ResolvedCode.CursorForward, ResolvedCode.CursorForward};
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void LongContentScrollsWhenCursorIsMovedBackwards()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test long string");
            var data = new object[] { ResolvedCode.End, ResolvedCode.CursorBackwards, ResolvedCode.CursorBackwards, ResolvedCode.CursorBackwards, ResolvedCode.CursorBackwards, ResolvedCode.CursorBackwards, ResolvedCode.CursorBackwards};
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void LongContentScrollsWhenCursorIsMovedToEnd()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test long string");
            var data = new object[] { ResolvedCode.End };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void LongContentScrollsWhenCursorIsMovedToHome()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test long string");
            var data = new object[] { ResolvedCode.End, ResolvedCode.Home };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void TheEndOfTheTextMovesAsNewDataIsAdded()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("Test long string");
            var data = new object[] { ResolvedCode.End, 'A', 'B', 'C', ResolvedCode.Home, ResolvedCode.End };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
        
        [Fact]
        public void TheEndOfShortTextMovesAsNewDataIsAdded()
        {
            //Arrange
            var textBox = new TextBox();
            textBox.Position(0,0,5,1);
            var valueWrapper = new ValueWrapper<string>("A");
            var data = new object[] { ResolvedCode.End, 'B', 'C', ResolvedCode.Home, ResolvedCode.End };
            var testRig = new ControlTestRig<string>(textBox, valueWrapper, data);
            
            //Act
            testRig.RunTest();

            //Assert
            testRig.GetReport().Verify();
        }
    }
}