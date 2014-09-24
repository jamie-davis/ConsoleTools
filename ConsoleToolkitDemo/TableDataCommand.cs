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
            error.WriteLine("This is bad.");
            console.WrapLine("Default table format");
            var data1 = Enumerable.Range(0, 5)
                                  .Select(i => new {Text = string.Format("item {0}", i), Index = i});
            console.FormatTable(data1);

            console.WriteLine();
            var data = Enumerable.Range(0, 5)
                                 .Select(i => new {Text = string.Format("item {0}", i), Index = i})
                                 .AsReport(x => x.AddColumn(c => c.Index, d => d.Heading("Just The Index"))
                                                 .AddColumn(c => string.Format("{0} miles", c.Index*2),
                                                            d => d.Heading("Index in miles")));
            console.FormatTable(data);
        }
    }
}