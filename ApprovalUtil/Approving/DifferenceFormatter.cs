using ConsoleToolkit.ConsoleIO;
using DiffMatchPatch;

namespace ApprovalUtil.Approving;

internal static class DifferenceFormatter
{
    public static void DisplayDiffs(string received, string approved, IConsoleAdapter console, IErrorAdapter error)
    {
        console.WrapLine("=================");
        var diffs = Diff.Compute(approved, received, checklines:true);
        console.WrapLine(diffs.ToReadableText());
        console.WrapLine("=================");
    }
}