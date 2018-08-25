using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.CommandLineInterpretation;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.Properties;
using ConsoleToolkit.Testing;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ConsoleToolkitTests.ConsoleIO
{
    [TestFixture]
    [UseReporter(typeof(CustomReporter))]
    public class ConsoleOutputAcceptanceTests
    {
        private ConsoleInterfaceForTesting _console;

        #region Test programs

        [UsedImplicitly]
        public class Program1 : ConsoleApplication
        {
            private static string[] _data =
            {
                "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta",
                "Iota", "Kappa", "Lambda", "Mu", "Nu", "Xi", "Omicron", "Pi", "Rho",
                "Sigma", "Tau", "Upsilon", "Phi", "Chi", "Psi", "Omega"
            };

            [Command]
            public class Options
            {
                [Option]
                public int Test { get; set; }

                [CommandHandler]
                public void Handle(IConsoleAdapter adapter)
                {
                    switch (Test)
                    {
                        case 60:
                            FixedColumnsThatRequiredSixtyChars(adapter);
                            break;

                        case 61:
                            ColumnsHaveMinWidthToFillSixtyChars(adapter);
                            break;
                    }
                }

                private void FixedColumnsThatRequiredSixtyChars(IConsoleAdapter adapter)
                {
                    var report = _data.AsReport(p => p.AddColumn(a => a,
                                                          col => col.Heading("Character").Width(10))
                                                      .AddColumn(a => a.Length,
                                                          col => col.Heading("Name Length").Width(10))
                                                      .AddColumn(a => new string(a.Reverse().ToArray()),
                                                          col => col.Heading("Backwards").Width(10))
                                                      .AddColumn(a => string.Join(" ", Enumerable.Repeat(a, a.Length)),
                                                          col => col.Heading("Repeated").Width(27)));
                    adapter.FormatTable(report);
                }

                private void ColumnsHaveMinWidthToFillSixtyChars(IConsoleAdapter adapter)
                {
                    var report = _data.AsReport(p => p.AddColumn(a => a,
                                                          col => col.Heading("Character").MinWidth(10))
                                                      .AddColumn(a => a.Length,
                                                          col => col.Heading("Name Length").MinWidth(10))
                                                      .AddColumn(a => new string(a.Reverse().ToArray()),
                                                          col => col.Heading("Backwards").MinWidth(10))
                                                      .AddColumn(a => string.Join(" ", Enumerable.Repeat(a, a.Length)),
                                                          col => col.Heading("Repeated").MinWidth(27)));
                    adapter.FormatTable(report);
                }
            }

            public Program1()
            {
                SetConfigTypeFilter(t => t.DeclaringType == GetType());
            }

            public static void XMain(string[] args)
            {
                Toolkit.Execute<ConsoleInputAcceptanceTests.Program1>(args);
            }
        }

        #endregion

        [SetUp]
        public void SetUp()
        {
            _console = new ConsoleInterfaceForTesting();
        }

        private string[] Args(string argsString)
        {
            return CommandLineTokeniser.Tokenise(argsString);
        }

        private void SetConsoleWidth(int width)
        {
            _console.BufferWidth = width;
            _console.WindowWidth = width;
            _console.Write(RulerFormatter.MakeRuler(width));
        }

        [Test]
        public void SimpleReportIsFormatted()
        {
            //Arrange
            SetConsoleWidth(60);

            //Act
            UnitTestAppRunner.Run<Program1>(Args("-Test 60"), _console);

            //Assert
            Approvals.Verify(_console.GetBuffer());
        }

        [Test]
        public void MinWidthDoesNotFitColumnsAreStacked()
        {
            //Arrange
            SetConsoleWidth(59);

            //Act
            UnitTestAppRunner.Run<Program1>(Args("-Test 61"), _console);

            //Assert
            Approvals.Verify(_console.GetBuffer());
        }

        [Test]
        public void MinWidthThatFillsWholeLineIsFormatted()
        {
            //Arrange
            SetConsoleWidth(60);

            //Act
            UnitTestAppRunner.Run<Program1>(Args("-Test 61"), _console);

            //Assert
            Approvals.Verify(_console.GetBuffer());
        }

        [Test]
        public void FixedColumnsThatDoNotFitAreStacked()
        {
            //Arrange
            SetConsoleWidth(59);

            //Act
            UnitTestAppRunner.Run<Program1>(Args("-Test 60"), _console);

            //Assert
            Approvals.Verify(_console.GetBuffer());
        }
    }
}
