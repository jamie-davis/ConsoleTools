using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests.Core;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;
using Approvals = ApprovalTests.Approvals;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class TestFormatTableCommandFactory
    {
        private List<TestType> _data;

        #region Types for test

        class TestType
        {
            public string S1 { get; set; }
            public int N1 { get; set; }
            public double D1 { get; set; }

            public string S2 { get; set; }
            public int N2 { get; set; }
            public double D2 { get; set; }

            public TestType(int n)
            {
                S1 = string.Format("String value {0}", n);
                N1 = n;
                D1 = n*1.1;

                S2 = new string('N', n);
                N2 = n*n;
                D2 = N2*1.1;
            }
        }

        #endregion

        private string Describe(IRecordedCommand cmd)
        {
            var replay = new ReplayBuffer(50);

            replay.Wrap(string.Format("Command type: {0}", cmd.GetType().Name));
            replay.NewLine();
            replay.NewLine();
            cmd.Replay(replay);

            var output = replay.ToLines().JoinWith(Environment.NewLine);
            Console.WriteLine(output);
            return output;
        }

        [SetUp]
        public void SetUp()
        {
            _data = Enumerable.Range(0, 10)
                .Select(n => new TestType(n))
                .ToList();
        }

        [Test]
        public void FormatTableCommandIsCreatedFromReport()
        {
            //Arrange
            var rep = _data.AsReport(r => r.AddColumn(c => c.S1, c => c.Heading("String One"))
                                           .AddColumn(c => c.S2, c => c.Heading("String Two"))
                                           .StretchColumns()
                                           .AddChild(t => t.S2, rp => rp.AddColumn(c => c, c => c.Heading("Char"))));

            //Act
            var cmd = FormatTableCommandFactory.Make(rep);

            //Assert
            Approvals.Verify(Describe(cmd));
        }
    }
}
