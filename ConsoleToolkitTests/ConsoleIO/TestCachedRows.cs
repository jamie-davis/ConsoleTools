using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;
using Approvals = ApprovalTests.Approvals;

namespace ConsoleToolkitTests.ConsoleIO
{
    [UseReporter(typeof(CustomReporter))]
    public class TestCachedRows
    {
        private static readonly DateTime TestDate = DateTime.Parse("2014-06-11 20:36");
        private List<Simple> _dataSet;

        #region Types for test
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local

        class Simple
        {
            public int Number { get; set; }
            public string StringData { get; set; }
            public DateTime Date { get; set; }

            public Simple(int number, string stringData = null, DateTime date = default(DateTime))
            {
                Number = number;
                StringData = stringData ?? string.Format("string {0}", number);
                Date = date == default(DateTime) ? TestDate : date;
            }

            public static implicit operator Simple(int n)
            {
                return new Simple(n);
            }
        }
        #endregion

        public TestCachedRows()
        {
            SetUpTests.OverrideCulture();
            _dataSet = new List<Simple>
            {
                10, 20, 30, 40, 50, 60, 70, 80, 90, 100
            };
        }

        [Fact]
        public void CachedDataCanBeRetrieved()
        {
            var data = new CachedRows<Simple>(_dataSet);
            Approvals.Verify(ReportData(data));
        }

        private string ReportData<T>(CachedRows<T> data)
        {
            return data.GetRows().Select(FormatRow).JoinWith(Environment.NewLine);
        }

        private string FormatRow<T>(CachedRow<T> row)
        {
            return row.Columns.Select(c => string.Format("{0} = {1}", c.Property.Name, c.Value).TrimEnd()).JoinWith(",");
        }

    }
}