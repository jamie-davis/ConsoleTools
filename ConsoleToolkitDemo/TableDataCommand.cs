using System.Linq;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitDemo
{
    [Command("tables")]
    [Description("Displays tabulated test data.")]
    class TableDataCommand
    {
        [CommandHandler]
        public void Handle(IConsoleAdapter console, IErrorAdapter error)
        {
            console.WrapLine("Default table format");
            console.WriteLine();
            var data1 = Enumerable.Range(0, 5)
                                  .Select(i => new {Text = string.Format("item {0}", i), Index = i});
            console.FormatTable(data1);
            console.WriteLine();

            var report = Enumerable.Range(0, 5)
                                 .Select(i => new {Text = string.Format("item {0}", i), Index = i})
                                 .AsReport(x => x.AddColumn(c => c.Index, d => d.Heading("Just The Index"))
                                                 .AddColumn(c => string.Format("{0} miles", c.Index*2),
                                                            d => d.Heading("Index in miles")));
            console.WriteLine();
            console.WrapLine("Report with custom headings");
            console.WriteLine();
            console.FormatTable(report);
            console.WriteLine();

            var report2 = Enumerable.Range(0, 5)
                                 .Select(i => new {Text = string.Format("item {0}", i), Index = i})
                                 .AsReport(x => x.AddColumn(c => c.Index, d => d.Heading("Fixed Width Index (12 wide)")
                                                                                .Width(12))
                                                 .AddColumn(c => string.Format("{0} miles", c.Index*2),
                                                            d => d.Heading("Index in miles")));
            console.WriteLine();
            console.WrapLine("Report with fixed width column");
            console.WriteLine();
            console.FormatTable(report2);
        }
    }
}