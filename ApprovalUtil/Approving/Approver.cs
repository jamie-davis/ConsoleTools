using ApprovalUtil.Commands;
using ApprovalUtil.Scanning;
using ConsoleToolkit.ConsoleIO;

namespace ApprovalUtil.Approving;

/// <summary>
/// This class implements the application functionality.
/// </summary>
public class Approver
{
    public static bool Execute(IConsoleAdapter console, IErrorAdapter error, IApproverParams approverParams)
    {
        if (approverParams == null) throw new ArgumentNullException(nameof(approverParams));
        try
        {
            if (string.IsNullOrWhiteSpace(approverParams.PathToCode))
            {
                error.WrapLine("No source path provided, unable to scan source code".Red());
                return false;
            }

            if (!Directory.Exists(approverParams.PathToCode))
            {
                error.WrapLine("Source code location does not exist".Red());
                return false;
            }

            console.WrapLine("Scanning...".Cyan());
            
            var tests = TestResultScanner.Scan(approverParams.PathToCode);
            if (approverParams.Interactive)
            {
                return InteractiveApprover.Execute(console, error, tests, approverParams);
            }

            console.FormatTable(tests.Select(t => new {t.Test.TestName, t.Test.ReceivedFile, t.Passed}));
            console.WrapLine("Done.".Cyan());

            return true;
        }
        catch (Exception e)
        {
            error.WrapLine("Approval processing halted due to error:".Red());
            error.WrapLine(e.Message.Red());
            return false;
        }
    }
}