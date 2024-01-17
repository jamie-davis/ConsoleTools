using System;
using FluentAssertions;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen;
using Xunit;

namespace VT100.Tests.Fullscreen.Controls
{
    public class TestLayoutProperties
    {
        #region Types for test

        [CaptionPosition(CaptionPosition.Top)]
        class TypeWithAttributes
        {
            
        }

        #endregion
        
        [Fact]
        public void CaptionPositionHasCorrectDefault()
        {
            //Act
            var layoutPropsInstance = new LayoutProperties();

            //Assert
            layoutPropsInstance.CaptionPosition.Should().Be(CaptionPosition.Left);
        }
        
        [Fact]
        public void CaptionPositionIsSet()
        {
            //Arrange
            var layoutPropsInstance = new LayoutProperties();
            var extractedProps = ControlPropertyExtractor.Extract(typeof(TypeWithAttributes));

            //Act
            ControlPropertySetter.Set(layoutPropsInstance, extractedProps);

            //Assert
            layoutPropsInstance.CaptionPosition.Should().Be(CaptionPosition.Top);
        }
    }
}