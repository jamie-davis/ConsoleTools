using ApprovalUtil.Scanning;
using ConsoleToolkit.ConsoleIO;

namespace ApprovalUtil.Approving;

internal static class InteractiveDifferenceDisplay
{
    public static void ShowDifferencesForFailure(IConsoleAdapter console, IErrorAdapter error, ApprovalTestOutput test)
    {
        var (received, approved) = TestOutputLoader.LoadText(test, error);
        DifferenceFormatter.DisplayDiffs(received, approved, console, error);

        var compareMenu = Read.String().Prompt("Select an option".Cyan());
        if (File.Exists(test.ReceivedFile))
            compareMenu.Option("A", "A", "Approve");
        compareMenu.Option("X", "X", "Done")
            .AsMenu("Approval functions");

        var compareOption = console.ReadInput(compareMenu).Value;
        if (compareOption == "A")
        {
            File.Move(test.ReceivedFile!, test.ApprovedFile!, true);
            console.WrapLine("Test approved.".Cyan());
        }
    }
}