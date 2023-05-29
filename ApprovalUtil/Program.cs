// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;
using ApprovalUtil;
using ApprovalUtil.Commands;
using ConsoleToolkit;
using ConsoleToolkit.ApplicationStyles;

[assembly: InternalsVisibleTo("ApprovalUtilTests")] //allow the test suite to access our implementation code

public class Program : CommandDrivenApplication
{
    public static void Main(string[] args)
    {
        Toolkit.Execute<Program>(args);
    }

    #region Overrides of ConsoleApplicationBase

    protected override void Initialise()
    {
        HelpCommand<HelpCommand>(o => o.Topic);
        base.Initialise();
    }

    #endregion
}