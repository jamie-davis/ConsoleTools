using ApprovalUtil.Scanning;
using ConsoleToolkit.ConsoleIO;

namespace ApprovalUtil.Approving;

internal static class InteractiveApprover
{
    public static bool Execute(IConsoleAdapter console, IErrorAdapter error, List<ApprovalTestResult> tests, IApproverParams approverParams)
    {
        var stats = tests.GroupBy(t => t.Passed).ToList();
        var passed = stats.FirstOrDefault(g => g.Key)?.ToList() ?? new();
        var failed = stats.FirstOrDefault(g => !g.Key)?.ToList() ?? new();
        
        console.WrapLine($"Located {tests.Count.ToString().White()} test{PluralOf(tests.Count)}. {failed.Count.ToString().Red()} failure{PluralOf(failed.Count)}, {passed.Count.ToString().Green()} pass{PluralOf(passed.Count, "es")}".Cyan());
        console.WriteLine();

        string PluralOf(int count, string plural = "s")
        {
            return count == 1 ? string.Empty : plural;
        }

        var menu = Read.String().Prompt("Select an option".Cyan());

        var index = 1;
        foreach (var failedTest in failed)
        {
            var selectedValue = $"{index++}";
            menu.Option(selectedValue, selectedValue, $"View {failedTest.Test.TestTypeName}.{failedTest.Test.TestName} failure");
        }

        if (failed.Any())
            menu.Option("A", "A", "Approve all and exit");    
        
        menu.Option("X", "X", "Exit")
            .AsMenu("Interactive functions");

        do
        {
            var option = console.ReadInput(menu).Value;
            if (option == "X") return true;
            
            if (option == "A" && console.Confirm("Are you sure you want to approve all?"))
            {
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
                return true;
            }

            console.WriteLine();
            var chosenFailure = failed[int.Parse(option) - 1];
            var (received, approved) = TestOutputLoader.LoadText(chosenFailure.Test, error);
            DifferenceFormatter.DisplayDiffs(received, approved, console, error);

            var compareMenu = Read.String().Prompt("Select an option".Cyan());
            if (File.Exists(chosenFailure.Test.ReceivedFile))
                compareMenu.Option("A", "A", "Approve");
            compareMenu.Option("X", "X", "Done")
                .AsMenu("Approval functions");

            var compareOption = console.ReadInput(compareMenu).Value;
            if (compareOption == "A") 
                File.Move(chosenFailure.Test.ReceivedFile, chosenFailure.Test.ApprovedFile, true);

        } while (true);
    }

}