using System.Text.RegularExpressions;

namespace ApprovalUtil.Scanning;

public static class ApprovalFileNameDeconstructor
{
    public static (string? TestTypeName, string? TestName) Deconstruct(string? approvalFile)
    {
        var match = Regex.Match(approvalFile, @"([^.]*)\.([^.]*)\.(approved|received)\.txt");
        if (!match.Success) return (null, null);
        
        return (match.Result("$1"), match.Result("$2"));
    }
}