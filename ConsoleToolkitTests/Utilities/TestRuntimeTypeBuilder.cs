using System.Collections.Generic;
using System.Linq;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.Utilities;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.Utilities
{
    public class TestRuntimeTypeBuilder
    {
        private RunTimeTypeBuilder _builder;
        public TestRuntimeTypeBuilder()
        {
            _builder = new RunTimeTypeBuilder(GetType().Name);
        }

        [Fact]
        public void SimpleTypeCanBeCreated()
        {
            //Arrange
            var props = new List<RuntimeTypeBuilderProperty>
                            {
                                new RuntimeTypeBuilderProperty("A", typeof (int)),
                                new RuntimeTypeBuilderProperty("B", typeof (double)),
                                new RuntimeTypeBuilderProperty("C", typeof (string))
                            };

            //Act
            var type = _builder.MakeRuntimeType(props);

            //Assert
            var result =type.GetProperties()
                .Select(p => string.Format("{0} {1};", p.Name, p.PropertyType.Name))
                .JoinWith(" ");
            Assert.Equal("A Int32; B Double; C String;", result);
        }

        [Fact]
        public void TheSameTypeIsReturnedIfItIsRequestedTwice()
        {
            //Arrange
            var props = new List<RuntimeTypeBuilderProperty>
                            {
                                new RuntimeTypeBuilderProperty("A", typeof (int)),
                                new RuntimeTypeBuilderProperty("B", typeof (double)),
                                new RuntimeTypeBuilderProperty("C", typeof (string))
                            };
            var intialTypeReturned = _builder.MakeRuntimeType(props);

            //Act
            var subsequentTypeReturned = _builder.MakeRuntimeType(props);

            //Assert
            subsequentTypeReturned.Should().BeSameAs(intialTypeReturned);
        }
    }
}
