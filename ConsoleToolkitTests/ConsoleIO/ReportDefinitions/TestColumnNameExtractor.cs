using System;
using System.Linq.Expressions;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    [TestFixture]
    public class TestColumnNameExtractor
    {
        #region Type for test

        class TestType
        {
            public string PropertyOne { get; set; }
            public string FieldOne;
        }

        #endregion

        [Test]
        public void PropertyNameIsExtractedFromAPropertyReference()
        {
            //Arrange
            var lambda = MakeLambdaExpression(t => t.PropertyOne);

            //Act
            var name = ColumnNameExtractor.FromExpression(lambda);

            //Assert
            Assert.That(name, Is.EqualTo("Property One"));
        }

        [Test]
        public void FieldNameIsExtractedFromAFieldReference()
        {
            //Arrange
            var lambda = MakeLambdaExpression(t => t.FieldOne);

            //Act
            var name = ColumnNameExtractor.FromExpression(lambda);

            //Assert
            Assert.That(name, Is.EqualTo("Field One"));
        }

        [Test]
        public void CalculatedExpressionReturnsExp()
        {
            //Arrange
            var lambda = MakeLambdaExpression(t => t.PropertyOne + "X");

            //Act
            var name = ColumnNameExtractor.FromExpression(lambda);

            //Assert
            Assert.That(name, Is.EqualTo("exp"));
        }

        private Expression MakeLambdaExpression<T>(Expression<Func<TestType, T>> expression)
        {
            return expression;
        }
    }
}
