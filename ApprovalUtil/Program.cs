// See https://aka.ms/new-console-template for more information

using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;

public class Program : ConsoleApplication
{
    public static void Main(string[] args)
    {
        Toolkit.Execute<Program>(args);
    }

    #region Overrides of ConsoleApplicationBase

    protected override void Initialise()
    {
        HelpOption<Options>(o => o.Help);
        base.Initialise();
    }

    #endregion
}