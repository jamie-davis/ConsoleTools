using System.Collections.Generic;
using System.Linq;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.Utilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.Utilities
{
    [TestFixture]
    public class TestRuntimeTypeBuilder
    {
        private RunTimeTypeBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new RunTimeTypeBuilder(GetType().Name);
        }

        [Test]
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
            Assert.That(result, Is.EqualTo("A Int32; B Double; C String;"));
        }

        [Test]
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
            Assert.That(subsequentTypeReturned, Is.SameAs(intialTypeReturned));
        }
    }
}
