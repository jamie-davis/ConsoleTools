using System;
using System.Linq.Expressions;
using ConsoleToolkit.ConsoleIO.ReportDefinitions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    public class TestColumnNameExtractor
    {
        #region Type for test

        class TestType
        {
            public string PropertyOne { get; set; }
            public string FieldOne;
        }

        #endregion

        [Fact]
        public void PropertyNameIsExtractedFromAPropertyReference()
        {
            //Arrange
            var lambda = MakeLambdaExpression(t => t.PropertyOne);

            //Act
            var name = ColumnNameExtractor.FromExpression(lambda);

            //Assert
            Assert.Equal("Property One", name);
        }

        [Fact]
        public void FieldNameIsExtractedFromAFieldReference()
        {
            //Arrange
            var lambda = MakeLambdaExpression(t => t.FieldOne);

            //Act
            var name = ColumnNameExtractor.FromExpression(lambda);

            //Assert
            Assert.Equal("Field One", name);
        }

        [Fact]
        public void CalculatedExpressionReturnsExp()
        {
            //Arrange
            var lambda = MakeLambdaExpression(t => t.PropertyOne + "X");

            //Act
            var name = ColumnNameExtractor.FromExpression(lambda);

            //Assert
            Assert.Equal("exp", name);
        }

        private Expression MakeLambdaExpression<T>(Expression<Func<TestType, T>> expression)
        {
            return expression;
        }
    }
}
