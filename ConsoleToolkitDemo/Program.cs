using System;
using System.Linq;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;
using ConsoleToolkit.ConsoleIO;

namespace ConsoleToolkitDemo
{
    class Program : CommandDrivenApplication
    {
        static void Main(string[] args)
        {
            Toolkit.Execute(args);
        }

        public void Handle(TableDataCommand command, IConsoleOperations console)
        {
            var data = Enumerable.Range(0, 20)
                .Select(i => new {Text = string.Format("item {0}", i), Index = i});
            console.FormatTable(data);
        }

protected override void Initialise()
        {
            HelpCommand<HelpCommand>(h => h.Topic);
        }
    }

}
