using System.Diagnostics;
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
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Toolkit.Execute<Program>(args);
            stopwatch.Stop();
            System.Console.WriteLine("Runtime: {0}", stopwatch.Elapsed);
        }

        protected override void Initialise()
        {
            HelpCommand<HelpCommand>(h => h.Topic);
        }
    }

}
