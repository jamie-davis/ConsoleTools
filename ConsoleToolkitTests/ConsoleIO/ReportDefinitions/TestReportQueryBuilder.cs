using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions;
using FluentAssertions;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    public class TestReportQueryBuilder
    {
        private QueryType[] _testData;

        #region Types for test
        
        class QueryType
        {
            public int Integer { get; set; }
            public double Double { get; set; }
            public string String { get; set; }

            public override string ToString()
            {
                return "QueryType";
            }
        }
        #endregion

        public TestReportQueryBuilder()
        {
            _testData = new[]
                            {
                                new QueryType {Double = 1.5, Integer = 100, String = "alpha"},
                                new QueryType {Double = 3.5, Integer = 200, String = "beta"}
                            };
        }

        private string GetPropertyDesc(Type enumeratedType)
        {
            return enumeratedType
                .GetProperties()
                .Select(p => string.Format("{0} {1};", p.PropertyType.Name, p.Name))
                .JoinWith(" ");
        }

        private Expression<Func<T, T1>>  Exp<T, T1>(Expression<Func<T, T1>> func)
        {
            return func;
        }

        [Fact]
        public void TypeGeneratedHasAppropriatePropertyTypes()
        {
            //Arrange
            var expressions = new Expression[]
                                  {
                                      Exp<QueryType, int>(q => q.Integer),
                                      Exp<QueryType, string>(q => q.Double.ToString())
                                  };

            //Act
            Type rowType;
            Func<object, QueryType> rowGetter;
            var output = ReportQueryBuilder.Build(_testData, expressions, out rowType, out rowGetter);

            //Assert
            var properties = GetPropertyDesc(rowType);
            Assert.Equal("Int32 exp1; String exp2; QueryType row;", properties);
        }

        [Fact]
        public void ExpectedValuesAreExtracted()
        {
            //Arrange
            var expressions = new Expression[]
                                  {
                                      Exp<QueryType, int>(q => q.Integer),
                                      Exp<QueryType, string>(q => q.String.PadLeft(8, '*'))
                                  };

            //Act
            Type rowType;
            Func<object, QueryType> rowGetter;
            var output = ReportQueryBuilder.Build(_testData, expressions, out rowType, out rowGetter);

            //Assert
            var sb = new StringBuilder();
            foreach (var item in output)
            {
                sb.AppendFormat("[{0}]", 
                                item.GetType().GetProperties()
                                .Select(p => string.Format("{0} = {1}", p.Name, p.GetValue(item, null)))
                                .JoinWith(" "));
            }
            var actual = sb.ToString();
            Assert.Equal("[exp1 = 100 exp2 = ***alpha row = QueryType][exp1 = 200 exp2 = ****beta row = QueryType]", actual);
        }

        [Fact]
        public void RowItemRetrieverReturnsTheOriginalRowObject()
        {
            //Arrange
            var expressions = new Expression[]
                                  {
                                      Exp<QueryType, int>(q => q.Integer),
                                      Exp<QueryType, string>(q => q.String.PadLeft(8, '*'))
                                  };

            //Act
            Type rowType;
            Func<object, QueryType> rowGetter;
            var output = ReportQueryBuilder.Build(_testData, expressions, out rowType, out rowGetter);

            //Assert
            rowGetter(output.Cast<object>().First()).Should().BeSameAs(_testData.First());
        }
    }
}