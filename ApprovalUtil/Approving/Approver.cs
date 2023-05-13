using ApprovalUtil.Scanning;
using ConsoleToolkit.ConsoleIO;

namespace ApprovalUtil.Approving;

/// <summary>
/// This class implements the application functionality.
/// </summary>
public class Approver
{
    public static bool Execute(IConsoleAdapter console, IErrorAdapter error, Options options)
    {
        try
        {
            console.WrapLine("Scanning...".Cyan());
            var tests = TestScanner.Scan(options.PathToCode);
            console.WrapLine("Done.".Cyan());
            console.FormatTable(tests);
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