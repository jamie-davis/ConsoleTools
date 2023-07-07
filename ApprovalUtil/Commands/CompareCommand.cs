using System.Security.AccessControl;
using ApprovalUtil.Approving;
using ApprovalUtil.Scanning;
using ConsoleToolkit.CommandLineInterpretation.ConfigurationAttributes;
using ConsoleToolkit.ConsoleIO;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace ApprovalUtil.Commands;

[Command]
[Description("Compares two files and shows differences. The files are assumed to be approval test results but this is not validated. By default a menu is presented allowing the test to be approved")]
public class CompareCommand
{
    [Positional]
    [Description("The received file")]
    public string Received { get; set; }

    [Positional]
    [Description("The approved file")]
    public string Approved { get; set; }
    
    [Option("nomenu", "n")]
    [Description("Show the differences and exit without showing the interactive menu")]
    public bool SkipMenu { get; set; }

    [CommandHandler]
    public void Handle(IConsoleAdapter console, IErrorAdapter error)
    {
        bool IsMissing(string path)
        {
            return string.IsNullOrWhiteSpace(path) || !File.Exists(path);
        }

        if (IsMissing(Approved) && IsMissing(Received))
        {
            error.WrapLine("At least one file must be specified and readable.");
            Environment.ExitCode = -100;
            return;
        }
       
        var output = new ApprovalTestOutput(null, Approved, Received, null, null);
        if (!SkipMenu)
        {
            InteractiveDifferenceDisplay.ShowDifferencesForFailure(console, error, output);
            return;
        }
        
        var (received, approved) = TestOutputLoader.LoadText(output, error);
        DifferenceFormatter.DisplayDiffs(received, approved, console, error);
    }
}