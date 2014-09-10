using System.Collections.Generic;
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
        public void Handle(TableDataCommand command, IConsoleOperations console)
        {
            var data = Enumerable.Range(0, 20)
                .Select(i => new { Text = string.Format("item {0}", i), Index = i })
                .Report();
            console.FormatTable(data);
        }

    }

    public class Report<T>
    {
        public Report(IEnumerable<T> items)
        {
            throw new System.NotImplementedException();
        }
    }

    public static class ReportExtensions
    {
        public static Report<T> Report<T>(this IEnumerable<T> items)
        {
            return new Report<T>(items);
        }

    }
}