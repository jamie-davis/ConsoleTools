using System.Reflection.Metadata.Ecma335;
using ApprovalUtil.Scanning;
using ConsoleToolkit.ConsoleIO;

namespace ApprovalUtil.Approving;

internal static class InteractiveApprover
{
    public static bool Execute(IConsoleAdapter console, IErrorAdapter error, List<ApprovalTestResult> tests)
    {
        var stats = tests.GroupBy(t => t.Passed).ToList();
        var passed = stats.FirstOrDefault(g => g.Key)?.ToList() ?? new();
        var failed = stats.FirstOrDefault(g => !g.Key)?.ToList() ?? new();
        
        console.WrapLine($"Located {tests.Count.ToString().White()} test{PluralOf(tests.Count)}. {failed.Count.ToString().Red()} failure{PluralOf(failed.Count)}, {passed.Count.ToString().Green()} pass{PluralOf(passed.Count, "es")}".Cyan());
        console.WriteLine();

        return DisplayInteractiveMenu(console, error, failed);
    }

    private static bool DisplayInteractiveMenu(IConsoleAdapter console, IErrorAdapter error, List<ApprovalTestResult> failed)
    {
        do
        {
            var menu = Read.String().Prompt("Select an option".Cyan());

            AddTestsToMenu(failed, menu);

            if (failed.Any())
                menu.Option("A", "A", "Approve all and exit");

            menu.Option("X", "X", "Exit")
                .AsMenu("Interactive functions");

            var option = console.ReadInput(menu).Value;
            if (option == "X") return true;

            if (option == "A")
            {
                ApproveAll(console, failed);
                return true;
            }

            console.WriteLine();
            var chosenFailure = failed[int.Parse(option) - 1];

            InteractiveDifferenceDisplay.ShowDifferencesForFailure(console, error, chosenFailure.Test);
            if (TestResultScanner.IsMatch(chosenFailure.Test))
                failed.Remove(chosenFailure);
            console.WriteLine();
        } while (true);
    }

    private static void AddTestsToMenu(List<ApprovalTestResult> failed, Read<string> menu)
    {
        var index = 1;
        foreach (var failedTest in failed)
        {
            var selectedValue = $"{index++}";
            menu.Option(selectedValue, selectedValue,
                $"View {failedTest.Test.TestTypeName}.{failedTest.Test.TestName} failure");
        }
    }

    private static void ApproveAll(IConsoleAdapter console, List<ApprovalTestResult> failed)
    {
        if (!console.Confirm("Are you sure you want to approve all?")) return;
        
        var approvedCount = 0;
        foreach (var failedTest in failed)
        {
            if (File.Exists(failedTest.Test.ReceivedFile))
            {
                console.WrapLine($"Approving {failedTest.Test.TestTypeName}.{failedTest.Test.TestName}...");
                File.Move(failedTest.Test.ReceivedFile, failedTest.Test.ApprovedFile, true);
                ++approvedCount;
            }
        }

        console.WrapLine($"{approvedCount} test{PluralOf(approvedCount)} approved.");
    }

    private static string PluralOf(int count, string plural = "s")
    {
        return count == 1 ? string.Empty : plural;
    }
}