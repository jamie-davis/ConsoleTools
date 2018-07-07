using System;
using System.Linq;
using System.Linq.Expressions;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions;
using ConsoleToolkit.Exceptions;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    [TestFixture]
    public class TestReportQueryRowFunctionBuilder
    {
        private QueryType[] _testData;

        #region Types for test

        class QueryType
        {
            public int Integer { get; set; }
            public double Double { get; set; }
            public string String { get; set; }
        }

        class ResultType
        {
            public ResultType()
            {
                
            }

            public ResultType(int intResult, string stringResult)
            {
                IntResult = intResult;
                StringResult = stringResult;
            }

            public int IntResult { get; set; }
            public string StringResult { get; set; }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            _testData = new[]
                            {
                                new QueryType {Double = 1.5, Integer = 100, String = "alpha"},
                                new QueryType {Double = 3.5, Integer = 200, String = "beta"}
                            };
        }

        private static ReportQueryExpression MakeQueryExpression<T, T1>(Expression<Func<T, T1>> arg)
        {
            var lamda = arg as LambdaExpression;
            if (lamda == null)
                throw new BadExpressionType(arg);
            return new ReportQueryExpression
            {
                Expression = lamda.Body,
                ReturnType = lamda.ReturnType,
                ParameterVariable = lamda.Parameters.First()
            };
        }

        [Test]
        public void QueryFunctionIsGenerated()
        {
            //Arrange
            var expressions = new[]
                                  {
                                      MakeQueryExpression<QueryType, int>(t => t.Integer),
                                      MakeQueryExpression<QueryType, string>(t => t.String)
                                  };

            //Act
            var func = ReportQueryRowFunctionBuilder.MakeQueryFunction<ResultType>(expressions);

            //Assert
            var result = _testData
                .Select(i => func(i)).Cast<ResultType>()
                .Select(r => string.Format("[{0}, {1}]", r.IntResult, r.StringResult))
                .JoinWith(" ");
            Assert.That(result, Is.EqualTo("[100, alpha] [200, beta]"));
        }

        [Test]
        public void CalculatedFieldExpressionsWork()
        {
            //Arrange
            var expressions = new[]
                                  {
                                      MakeQueryExpression<QueryType, int>(t => t.Integer / 10),
                                      MakeQueryExpression<QueryType, string>(t => t.Double.ToString())
                                  };

            //Act
            var func = ReportQueryRowFunctionBuilder.MakeQueryFunction<ResultType>(expressions);

            //Assert
            var result = _testData
                .Select(i => func(i)).Cast<ResultType>()
                .Select(r => string.Format("[{0}, {1}]", r.IntResult, r.StringResult))
                .JoinWith(" ");
            Assert.That(result, Is.EqualTo("[10, 1.5] [20, 3.5]"));
        }

        [Test]
        public void MoreExpressionsThanPropertiesThrows()
        {
            //Arrange
            var expressions = new[]
                                  {
                                      MakeQueryExpression<QueryType, int>(t => t.Integer),
                                      MakeQueryExpression<QueryType, int>(t => t.Integer),
                                  };

            //Act
            Assert.Throws<ResultTypeCannotAcceptQuery>(() => ReportQueryRowFunctionBuilder.MakeQueryFunction<ResultType>(expressions));
        }

        [Test]
        public void MixedTargetTypeThrows()
        {
            //Arrange
            var expressions = new[]
                                  {
                                      MakeQueryExpression<QueryType, int>(t => t.Integer),
                                      MakeQueryExpression<string, string>(t => t),
                                  };

            //Act
            Assert.Throws<MixedInputTypesInQueryExpressions>(() => ReportQueryRowFunctionBuilder.MakeQueryFunction<ResultType>(expressions));
        }
    }
}