using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.Internal.ReportDefinitions;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.ReportDefinitions
{
    [TestFixture]
    public class TestReportQueryBuilder
    {
        private QueryType[] _testData;

        #region Types for test
        
        class QueryType
        {
            public int Integer { get; set; }
            public double Double { get; set; }
            public string String { get; set; }
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

        [Test]
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
            var output = ReportQueryBuilder.Build(_testData, expressions, out rowType);

            //Assert
            var properties = GetPropertyDesc(rowType);
            Assert.That(properties, Is.EqualTo("Int32 exp1; String exp2;"));
        }

        [Test]
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
            var output = ReportQueryBuilder.Build(_testData, expressions, out rowType);

            //Assert
            var sb = new StringBuilder();
            foreach (var item in output)
            {
                sb.AppendFormat("[{0}]", 
                                item.GetType().GetProperties()
                                .Select(p => string.Format("{0} = {1}", p.Name, p.GetValue(item, null)))
                                .JoinWith(" "));
            }
            Assert.That(sb.ToString(), Is.EqualTo("[exp1 = 100 exp2 = ***alpha][exp1 = 200 exp2 = ****beta]"));
        }
    }
}